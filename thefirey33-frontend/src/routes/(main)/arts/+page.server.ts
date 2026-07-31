import type { PageServerLoad } from './$types';
import { env } from '$env/dynamic/private';
import { type ArtResponse, getJson } from '$lib/types';

export const load: PageServerLoad = async ({ fetch }) => {
	// This will fetch all the related art content from the server.

	const result: {
		message: string[] | undefined;
		success: boolean;
		errorMessage?: string;
	} = await getJson<string[]>(fetch, `${env.FIREYBACKEND_API}/Art/categories`);

	if (!result.success || result.message === undefined) {
		return {
			success: false,
			data: undefined,
			errorMessage: 'Failure to fetch categories'
		};
	}

	const categorizedData: Map<string, ArtResponse[]> = new Map();

	for (const item of result.message) {
		const categoryData: {
			message: ArtResponse[] | undefined;
			success: boolean;
			errorMessage?: string;
		} = await getJson(fetch, `${env.FIREYBACKEND_API}/Art/category/${item}`);
		if (!categoryData.success || categoryData.message === undefined) {
			return {
				success: false,
				errorMessage: 'Failure to fetch art piece',
				data: undefined
			};
		}
		categorizedData.set(item, categoryData.message);
	}
	return { success: true, data: categorizedData };
};
