/**
 * Response for the art data.
 */
export interface ArtResponse {
	id: number;
	uuid: string;
	category: string;
	author: string;
	title: string;
	description: string;
}

export interface QuestionDbType {
	id: number;
	time: string;
	question: string;
	attachment: string;
	author_id: number;
	author: string;
	response: string | null;
}

/**
 * The reply from discord.
 */
export interface DiscordReply {
	id: string;
	username: string;
	discriminator: string;
	avatar: string;
	avatar_url: string;
	locale: string;
	email: string;
	bot: boolean;
	mfa_enabled: boolean;
	flags: number;
	premium_type: number;
	public_flags: number;
}

/**
 * The Discord Token's Name.
 */
export const DiscordTokenName = 'D-Token';

/**
 * The name of the Auth Token of the website.
 */
export const AuthTokenName = 'Token';

/**
 * The author of the specified GitHub commit.
 */
export interface Author {
	login: string;
	avatar_url: string;
	html_url: string;
}

/**
 * All the repositories that Thefirey33 owns, will be portioned in this request.
 */
export interface RepositoryGitData {
	id: number;
	name: string;
	owner: Author;
	description?: string;
	html_url: string;
	created_at: string;
	language?: string;
	archived: boolean;
}

/**
 * The TenTrillion GitHub Tracker Data.
 */
export interface TenTrillionGitData {
	sha: string;
	node_id: string;
	html_url: string;
	commit: {
		message: string;
	};
	author: Author;
}

/**
 * The minimum amount of zoom.
 */
export const minFontSize = 18;

/**
 * The maximum amount of zoom.
 */
export const maxFontSize = 50;

export async function getJson<T>(
	fetch: {
		(input: RequestInfo | URL, init?: RequestInit): Promise<Response>;
		(input: string | URL | Request, init?: RequestInit): Promise<Response>;
	},
	urlLink: string,
	init?: RequestInit
): Promise<{ message: T | undefined; success: boolean; errorMessage?: string }> {
	try {
		return await fetch(urlLink, init).then(async (r) => {
			return {
				message: await r.json(),
				success: true
			};
		});
	} catch (error) {
		return {
			// OH, FOR FUCK's SAKE SHUT UP ESLINT
			// eslint-disable-next-line @typescript-eslint/ban-ts-comment
			// @ts-expect-error
			message: error.message,
			errorMessage: 'Failed to communicate with the API',
			success: false
		};
	}
}
