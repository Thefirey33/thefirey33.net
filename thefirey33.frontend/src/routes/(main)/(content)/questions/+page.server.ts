import { env } from '$env/dynamic/private';
import { getJson } from '$lib/types';
import type { PageServerLoad } from './$types';

interface QuestionDbType {
	id: number;
	time: string;
	question: string;
	attachment: string;
	response: string | null;
}

export const load = (async ({ fetch }) => {
	const result: {
		message?: QuestionDbType[];
		success: boolean;
		errorMessage?: string;
	} = await getJson(fetch, `${env.FIREYBACKEND_API}/Question`);

	return { questions: result };
}) satisfies PageServerLoad;
