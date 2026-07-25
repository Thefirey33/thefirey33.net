import type { PageServerLoad } from './$types';
import { env } from '$env/dynamic/private';
import type { TenTrillionGitData } from '$lib';

export const load: PageServerLoad = async ({ fetch }) => {
	const data: TenTrillionGitData[] = await fetch(`${env.FIREYBACKEND_API}/Git`).then((res) => {
		if (!res.ok) throw new Error("Couldn't fetch data!");
		return res.json();
	});

	return {
		gitData: data
	};
};
