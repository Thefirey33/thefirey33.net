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
