import type { PageServerLoad } from './$types';
import { env } from '$env/dynamic/private';
import { getJson, type RepositoryGitData } from '$lib/types';

export const load: PageServerLoad = async ({ fetch }) => {
	const result: {
		message: RepositoryGitData[] | undefined;
		success: boolean;
		errorMessage?: string;
	} = await getJson(fetch, `${env.FIREYBACKEND_API}/Git/repositories`);
	return { repositories: result };
};
