import type { RequestHandler } from './$types';
import { env } from '$env/dynamic/private';

export const GET: RequestHandler = async ({ fetch }) => {
	const result = await fetch(`${env.FIREYBACKEND_API}/Dex/zip`).then((r) => r.bytes());

	return new Response(result, {
		headers: {
			'content-type': 'application/zip',
			'content-disposition': `attachment; filename=nikodex-backup-${Date.now()}.zip`
		}
	});
};
