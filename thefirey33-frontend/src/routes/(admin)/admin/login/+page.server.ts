import type { Actions, PageServerLoad } from './$types';
import { env } from '$env/dynamic/private';
import { redirect } from '@sveltejs/kit';

interface AuthorizationPayload {
	token: string;
}

export const load: PageServerLoad = async ({ fetch }) => {
	// When the server is loaded, send a request to the server to generate the auth code.

	await fetch(`${env.FIREYBACKEND_API}/Auth/code`, {
		method: 'POST'
	});
};

export const actions = {
	default: async ({ request, fetch, cookies }) => {
		const data = await request.formData();

		const payload = JSON.stringify({
			code: data.get('code'),
			name: data.get('name'),
			password: data.get('password')
		});

		try {
			const authorizationRequest: AuthorizationPayload = await fetch(
				`${env.FIREYBACKEND_API}/Auth/login`,
				{
					method: 'POST',
					headers: {
						'content-type': 'application/json'
					},
					body: payload
				}
			).then((r) => {
				if (!r.ok) throw new Error('Failed to authorize!');
				return r.json();
			});
			cookies.set('Token', authorizationRequest.token, {
				path: '/',
				secure: true,
				maxAge: 3600 * 5
			});
		} catch {
			return { success: false };
		}

		redirect(303, '/admin');
	}
} satisfies Actions;
