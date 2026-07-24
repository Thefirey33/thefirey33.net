import type { PageServerLoad } from './$types';
import { env } from '$env/dynamic/private';
import type { ArtResponse } from '$lib';

export const load: PageServerLoad = async () => {
	// This will fetch all the related art content from the server.

	const result: string[] = await fetch(`${env.FIREYBACKEND_API}/Art/categories`).then((res) =>
		res.json()
	);
	const categorizedData: Map<string, ArtResponse[]> = new Map();

	for (const item of result) {
		const categoryData = await fetch(`${env.FIREYBACKEND_API}/Art/category/${item}`).then((res) =>
			res.json()
		);
		categorizedData.set(item, categoryData);
	}
	return { data: categorizedData };
};
