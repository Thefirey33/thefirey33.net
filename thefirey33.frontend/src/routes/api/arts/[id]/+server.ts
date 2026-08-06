import type { RequestHandler } from './$types';
import { env } from '$env/dynamic/private';
import { json } from '@sveltejs/kit';
import { AuthTokenName } from '$lib/types';

export const DELETE: RequestHandler = async ({ params, fetch, cookies }) => {
	const result = await fetch(`${env.FIREYBACKEND_API}/Art/${params.id}`, {
		method: 'DELETE',
		headers: {
			Authorization: `Bearer ${cookies.get(AuthTokenName)}`
		}
	}).then((r) => r.ok);

	return json({
		success: result
	});
};
