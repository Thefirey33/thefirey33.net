import type { LayoutServerLoad } from './$types';
import { env } from '$env/dynamic/private';
import { redirect } from '@sveltejs/kit';
import { getJson } from '$lib/types';

export const load: LayoutServerLoad = async ({ url, fetch, cookies }) => {
	if (url.pathname !== '/admin/login') {
		const authorizationState = await fetch(`${env.FIREYBACKEND_API}/Auth/check`).then((r) => r.ok);
		// Check the auth state of the current user.
		if (!authorizationState) throw redirect(307, '/admin/login');
	}

	const info: {
		message: Information | undefined;
		success: boolean;
		errorMessage?: string;
	} = await getJson(fetch, `${env.FIREYBACKEND_API}/Information`, {
		headers: {
			Authorization: `Bearer ${cookies.get('Token')}`
		}
	});

	return { info: info };
};
