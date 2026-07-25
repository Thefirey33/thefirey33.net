import type { PageServerLoad } from './$types';
import { env } from '$env/dynamic/private';
import type { RepositoryGitData } from '$lib';

export const load: PageServerLoad = async ({ fetch }) => {
	const result: RepositoryGitData[] = await fetch(`${env.FIREYBACKEND_API}/Git/repositories`).then(
		(res) => {
			if (!res.ok) throw new Error('Failed to load repositories');
			return res.json();
		}
	);
	return { repositories: result };
};
