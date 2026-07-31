import type { RequestHandler } from './$types';
import { env } from '$env/dynamic/private';
import { json } from '@sveltejs/kit';

export const DELETE: RequestHandler = async ({ params, fetch }) => {
	const result = await fetch(`${env.FIREYBACKEND_API}/Art/${params.id}`, {
		method: 'DELETE'
	}).then((r) => r.ok);

	return json({
		success: result
	});
};
