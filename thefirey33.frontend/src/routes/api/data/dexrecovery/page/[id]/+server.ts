import type { RequestHandler } from './$types';
import { getJson } from '$lib/types';
import type { Niko } from '$lib/types/dexrecovery';
import { env } from '$env/dynamic/private';
import { json } from '@sveltejs/kit';

export const GET: RequestHandler = async ({ fetch, params }) => {
	const nikoResult: {
		message: Niko | undefined;
		success: boolean;
		errorMessage?: string;
	} = await getJson(fetch, `${env.FIREYBACKEND_API}/Dex/page/${params.id}`);

	return json(nikoResult);
};
