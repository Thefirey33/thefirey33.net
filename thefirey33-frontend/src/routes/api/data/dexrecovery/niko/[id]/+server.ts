import type { RequestHandler } from './$types';
import { env } from '$env/dynamic/private';
import { json } from '@sveltejs/kit';
import { getJson } from '$lib/types';
import type { Niko } from '$lib/types/dexrecovery';

export const GET: RequestHandler = async ({ fetch, params }) => {
	const result: {
		message: Niko | undefined;
		success: boolean;
		errorMessage?: string;
	} = await getJson(fetch, `${env.FIREYBACKEND_API}/Dex/niko/${params.id}`);

	if (!result.success || result.message === undefined) {
		return new Response(null, {
			status: 404
		});
	}

	return json(result, {
		headers: {
			'content-disposition': `attachment; filename=niko-${params.id}.json`
		}
	});
};
