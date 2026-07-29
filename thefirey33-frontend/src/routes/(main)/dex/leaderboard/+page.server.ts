import type { PageServerLoad } from './$types';
import { getJson } from '$lib';
import { env } from '$env/dynamic/private';

interface LeaderboardStats {
	author: string;
	count: number;
}

export const load: PageServerLoad = async ({ fetch }) => {
	const results: {
		message: LeaderboardStats[] | undefined;
		success: boolean;
		errorMessage?: string;
	} = await getJson(fetch, `${env.FIREYBACKEND_API}/Dex/leaderboard`);

	return { results: results };
};
