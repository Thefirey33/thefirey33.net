import type { RequestHandler } from './$types';
import { env } from '$env/dynamic/private';
import { json } from '@sveltejs/kit';
import type { ArtResponse } from '$lib/types';

export const GET: RequestHandler = async ({ url, fetch, cookies }) => {
	// Get the art content from the API.
	const result: ArtResponse[] = await fetch(`${env.FIREYBACKEND_API}/Art`, {
		headers: {
			Authorization: `Bearer ${cookies.get('Token')}`
		}
	}).then((r) => r.json());

	return json(result);
};
