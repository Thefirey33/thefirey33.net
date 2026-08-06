import type { RequestHandler } from './$types';
import { type DiscordReply, DiscordTokenName, getJson } from '$lib/types';
import { env } from '$env/dynamic/private';
import { json } from '@sveltejs/kit';

export const GET: RequestHandler = async ({ fetch, cookies }) => {
	const discordToken = cookies.get(DiscordTokenName);

	const result: {
		message?: DiscordReply;
		success: boolean;
		errorMessage?: string;
	} = await getJson(fetch, `${env.FIREYFILTERINGSERVICE_HTTP}/auth/user`, {
		headers: {
			Authorization: `Bearer ${discordToken}`
		}
	});

	return json(result);
};
