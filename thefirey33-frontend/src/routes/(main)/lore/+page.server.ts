import type { PageServerLoad } from './$types';
import LoreData from '$lib/assets/data/txt/lore.txt';

export const load: PageServerLoad = async ({ fetch }) => {
	const loreData: string[] = await fetch(LoreData).then(async (r) => {
		const response = await r.text();
		return response.split('\n');
	});
	return { loreData: loreData };
};
