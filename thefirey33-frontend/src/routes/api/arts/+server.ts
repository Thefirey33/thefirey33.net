import type { RequestHandler } from './$types';
import { env } from '$env/dynamic/private';
import { json } from '@sveltejs/kit';
import type { ArtResponse } from '$lib';

export const GET: RequestHandler = async ({ url, fetch }) => {
	// Get the art content from the API.
	const result: ArtResponse[] = await fetch(`${env.FIREYBACKEND_API}/Art`).then((r) => r.json());
	return json(result);
};
