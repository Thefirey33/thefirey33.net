import type { RequestHandler } from './$types';
import { getJson } from '$lib/types';
import { env } from '$env/dynamic/private';
import { json } from '@sveltejs/kit';

export const GET: RequestHandler = async ({ fetch, cookies }) => {
	const result: {
		message: Approval[] | undefined;
		success: boolean;
		errorMessage?: string;
	} = await getJson(fetch, `${env.FIREYBACKEND_API}/Approval`, {
		headers: {
			Authorization: `Bearer ${cookies.get('Token')}`
		}
	});

	return json(result);
};
