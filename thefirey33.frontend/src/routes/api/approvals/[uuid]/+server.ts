import type { RequestHandler } from './$types';
import { env } from '$env/dynamic/private';
import { AuthTokenName } from '$lib/types';

export const PUT: RequestHandler = async ({ fetch, params, url, cookies }) => {
	const approvedSearchQuery = url.searchParams.get('approved') ?? 'false';

	const result = await fetch(
		`${env.FIREYBACKEND_API}/Approval/${params.uuid}?approved=${approvedSearchQuery}`,
		{
			method: 'PUT',
			headers: { Authorization: `Bearer ${cookies.get(AuthTokenName)}` }
		}
	);

	return new Response(result.body, {
		status: result.status
	});
};
