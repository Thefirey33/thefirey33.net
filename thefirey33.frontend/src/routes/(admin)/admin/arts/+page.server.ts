import type { Actions, PageServerLoad } from './$types';
import { type ArtResponse, AuthTokenName, getJson } from '$lib/types';
import { env } from '$env/dynamic/private';

export const load: PageServerLoad = async ({ fetch }) => {
	const result: { message: ArtResponse[] | undefined; success: boolean; errorMessage?: string } =
		await getJson(fetch, `${env.FIREYBACKEND_API}/Art`);

	return { arts: result };
};

export const actions: Actions = {
	default: async ({ request, fetch, cookies }) => {
		const formData = await request.formData();

		const result = await fetch(`${env.FIREYBACKEND_API}/Art`, {
			method: 'POST',
			headers: { Authorization: `Bearer ${cookies.get(AuthTokenName)}` },
			body: formData
		}).then(async (r) => {
			return r.ok;
		});

		return { result: result };
	}
} satisfies Actions;
