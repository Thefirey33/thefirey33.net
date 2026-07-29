/**
 * Information about this backup of the dex.
 */
export interface DexRecoveryInformation {
	pages: number;
	date: string;
}

/**
 * Data for the Nikos in the database.
 */
export interface Niko {
	id: number;
	name: string;
	abilities: {
		id: number;
		name: string;
	}[];
	author_name: string;
	full_desc: string;
	description: string;
	is_blacklisted: boolean;
}
