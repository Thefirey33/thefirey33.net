import type { RequestHandler } from './$types';
import { env } from '$env/dynamic/private';
import { json } from '@sveltejs/kit';

export const PUT: RequestHandler = async ({ params, url }) => {
	const result = await fetch(
		`${env.FIREYBACKEND_API}/Question/${params.id}?response=${url.searchParams.get('response')}`,
		{
			method: 'PUT'
		}
	).then((r) => r.ok);

	return json({ success: result });
};

export const DELETE: RequestHandler = async ({ params }) => {
	const result = await fetch(`${env.FIREYBACKEND_API}/Question/${params.id}`, {
		method: 'DELETE'
	}).then((r) => r.ok);

	return json({ success: result });
};
