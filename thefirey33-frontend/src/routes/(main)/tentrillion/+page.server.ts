import type { PageServerLoad } from './$types';
import { env } from '$env/dynamic/private';

interface GitData {
	sha: string;
	node_id: string;
	html_url: string;
	commit: {
		message: string;
	};
	author: {
		login: string;
		avatar_url: string;
		html_url: string;
	};
}

export const load: PageServerLoad = async ({ fetch }) => {
	const data: GitData[] = await fetch(`${env.FIREYBACKEND_API}/Git`).then((res) => {
		if (!res.ok) throw new Error("Couldn't fetch data!");
		return res.json();
	});

	return {
		gitData: data
	};
};
