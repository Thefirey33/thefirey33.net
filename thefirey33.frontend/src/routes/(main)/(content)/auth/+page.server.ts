import type { PageServerLoad } from './$types';
import { redirect } from '@sveltejs/kit';
import { DiscordTokenName, getJson } from '$lib/types';
import { env } from '$env/dynamic/private';

interface DiscordApiResponse {
	access_token: string;
	refresh_token: string;
}

export const load: PageServerLoad = async ({ url, cookies, fetch }) => {
	// This is for the Discord API authorization passthrough.
	// It calls the Discord API to get the specified Authorization token, then redirects back to the questions page when authorized.

	const code = url.searchParams.get('code');

	if (code != null) {
		const authResult: {
			message?: DiscordApiResponse;
			success: boolean;
			errorMessage?: string;
		} = await getJson(fetch, `${env.FIREYFILTERINGSERVICE_HTTP}/auth/callback?code=${code}`);

		if (!authResult.success || authResult.message == undefined)
			throw redirect(308, `/questions?error=${authResult.errorMessage}`);

		cookies.set(DiscordTokenName, authResult.message.access_token, {
			path: '/',
			secure: true,
			maxAge: 3600 * 5
		});
	}

	return redirect(308, '/questions');
};
