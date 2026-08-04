import type { RequestHandler } from './$types';
import { env } from '$env/dynamic/private';

export const GET: RequestHandler = async ({ fetch, params, url, cookies }) => {
	const result = await fetch(
		`${env.FIREYBACKEND_API}/Data/${params.uuid}?pr=${(url.searchParams.get('pr') ?? 'false') === 'true'}`,
		{
			headers: {
				Authorization: `Bearer ${cookies.get('Token')}`
			}
		}
	);
	const resultBytes = await result.bytes();
	// Send the specified file with the bytes.

	return new Response(resultBytes, {
		headers: {
			'content-type': result.headers.get('content-type') ?? 'application/json'
		}
	});
};
