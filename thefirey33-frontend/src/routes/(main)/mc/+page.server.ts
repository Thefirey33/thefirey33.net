import type { PageServerLoad } from './$types';
import { getJson } from '$lib';
import { env } from '$env/dynamic/private';

interface ServerInformation {
	currentPlayers: string[];
	serverUptime: number;
}

export const load: PageServerLoad = async ({ fetch }) => {
	const result: {
		message: ServerInformation | undefined;
		success: boolean;
		errorMessage?: string;
	} = await getJson(fetch, `${env.FIREYMINECRAFTSERVER_API}`);
	return { minecraftServerInformation: result };
};
