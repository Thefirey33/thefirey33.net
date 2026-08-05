import type { PageServerLoad } from './$types';
import { env } from '$env/dynamic/private';
import { getJson, type TenTrillionGitData } from '$lib/types';

export const load: PageServerLoad = async ({ fetch }) => {
	const data: {
		message: TenTrillionGitData[] | undefined;
		success: boolean;
		errorMessage?: string;
	} = await getJson(fetch, `${env.FIREYBACKEND_API}/Git`);
	return {
		gitData: data
	};
};
