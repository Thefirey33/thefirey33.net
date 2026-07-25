// place files you want to import through the `$lib` alias in this folder.

export interface ArtResponse {
	id: number;
	uuid: string;
	category: string;
	author: string;
	title: string;
	description: string;
}

export interface Author {
	login: string;
	avatar_url: string;
	html_url: string;
}

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

export interface TenTrillionGitData {
	sha: string;
	node_id: string;
	html_url: string;
	commit: {
		message: string;
	};
	author: Author;
}
