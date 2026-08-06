import type { Actions, PageServerLoad } from './$types';
import { AuthTokenName, getJson, type QuestionDbType } from '$lib/types';
import { env } from '$env/dynamic/private';

export const load: PageServerLoad = async ({ cookies, fetch }) => {
	const authToken = cookies.get(AuthTokenName);

	const result: {
		message?: QuestionDbType[];
		success: boolean;
		errorMessage?: string;
	} = await getJson(fetch, `${env.FIREYBACKEND_API}/Question/all`, {
		headers: {
			Authorization: `Bearer ${authToken}`
		}
	});

	return { questions: result };
};

export const actions = {
	change: async ({ request, fetch }) => {
		const formData = await request.formData();

		const result: {
			success: boolean;
		} = await fetch(`/api/questions/${formData.get('id')}?response=${formData.get('response')}`, {
			method: 'PUT'
		}).then((r) => r.json());

		return result;
	},
	delete: async ({ request, fetch }) => {
		const formData = await request.formData();

		const result: {
			success: boolean;
		} = await fetch(`/api/questions/${formData.get('id')}`, {
			method: 'DELETE'
		}).then((r) => r.json());

		return result;
	}
} satisfies Actions;
