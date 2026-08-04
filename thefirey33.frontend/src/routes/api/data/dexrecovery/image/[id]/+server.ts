import type { RequestHandler } from './$types';
import { env } from '$env/dynamic/private';

export const GET: RequestHandler = async ({ fetch, params }) => {
	const imageResult = await fetch(`${env.FIREYBACKEND_API}/Dex/image/${params.id}`).then((r) => {
		if (!r.ok) return undefined;

		return r.bytes();
	});

	if (imageResult == undefined)
		return new Response(null, {
			status: 404
		});

	// Return the image response.
	return new Response(imageResult, {
		headers: {
			'content-type': 'image/png'
		}
	});
};
