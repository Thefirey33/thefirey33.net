import { env } from '$env/dynamic/private';
import { type DiscordReply, DiscordTokenName, getJson, type QuestionDbType } from '$lib/types';
import type { Actions, PageServerLoad } from './$types';

interface AuthLinkResponse {
	url: string;
}

export const load = (async ({ fetch, cookies, url }) => {
	// The current questions.
	const result: {
		message?: QuestionDbType[];
		success: boolean;
		errorMessage?: string;
	} = await getJson(fetch, `${env.FIREYBACKEND_API}/Question`);

	const discordToken = cookies.get(DiscordTokenName);
	const linkResponse: {
		message?: AuthLinkResponse;
		success: boolean;
		errorMessage?: string;
	} = await getJson(
		fetch,
		`${env.FIREYFILTERINGSERVICE_HTTP}/auth/login?redirect_uri=${url.origin}/auth`
	);

	if (discordToken == undefined) {
		// If the specified Discord Token is undefined, then send the Discord Authorization Link.

		return { questions: result, link: linkResponse };
	} else {
		const authenticatedCheck: {
			message?: boolean;
			success: boolean;
			errorMessage?: string;
		} = await getJson(fetch, `${env.FIREYFILTERINGSERVICE_HTTP}/auth/authenticated`, {
			headers: {
				Authorization: `Bearer ${discordToken}`
			}
		});

		// If the authorization failed, then force a reauthorization.
		if (!authenticatedCheck.message) {
			return { questions: result, link: linkResponse };
		}
	}

	const userRequest: {
		message?: DiscordReply;
		success: boolean;
		errorMessage?: string;
	} = await getJson(fetch, `${env.FIREYFILTERINGSERVICE_HTTP}/auth/user`, {
		headers: {
			Authorization: `Bearer ${discordToken}`
		}
	});

	return { questions: result, user: userRequest };
}) satisfies PageServerLoad;

interface QuestionPostResponse {
	success: boolean;
	message: string;
}

export const actions = {
	default: async ({ request, cookies }) => {
		const discordToken = cookies.get(DiscordTokenName);
		const authenticatedCheck: boolean = await fetch(
			`${env.FIREYFILTERINGSERVICE_HTTP}/auth/authenticated`,
			{
				headers: {
					Authorization: `Bearer ${discordToken}`
				}
			}
		).then((r) => {
			// Submit if submit.
			if (!r.ok) return false;

			return r.json();
		});

		// If the authorization failed, then force a reauthorization.
		if (!authenticatedCheck) {
			return { success: false, message: 'Unauthorized!' };
		}

		const userRequest: DiscordReply = await fetch(
			`${env.FIREYFILTERINGSERVICE_HTTP}/auth/user?token=${discordToken}`,
			{
				headers: {
					Authorization: `Bearer ${discordToken}`
				}
			}
		).then((r) => r.json());

		// Upload the specified data for the database.
		// Which also links the discord avatar url.
		const formData = await request.formData();
		formData.append('AuthorName', userRequest.username);
		formData.append('UserId', userRequest.id);

		try {
			const postForm: QuestionPostResponse = await fetch(`${env.FIREYBACKEND_API}/Question`, {
				method: 'POST',
				body: formData
			}).then((r) => r.json());

			return postForm;
		} catch {
			return {
				success: false,
				message: `Failure to send your requested file to the backend!`
			};
		}
	}
} satisfies Actions;
