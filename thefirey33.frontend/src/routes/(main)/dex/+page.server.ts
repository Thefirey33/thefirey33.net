import type { PageServerLoad } from './$types';
import type { DexRecoveryInformation } from '$lib/types/dexrecovery';
import { getJson } from '$lib/types';
import { env } from '$env/dynamic/private';

export const load: PageServerLoad = async ({ fetch }) => {
	const startTime = performance.now();
	// The status of the NikoDex front-end itself. Basically the Website that the users will access.
	const apiStatus = await fetch('https://nikodex.net/api/ping').then((r) => r.ok);

	// The status of the NikoDex front-end API.
	const websiteStatus = await fetch('https://nikodex.net').then((r) => r.ok);
	const endTimeDifference = performance.now() - startTime;

	const response: {
		message: DexRecoveryInformation | undefined;
		success: boolean;
		errorMessage?: string;
	} = await getJson(fetch, `${env.FIREYBACKEND_API}/Dex`);

	return {
		apiStatus: apiStatus,
		websiteStatus: websiteStatus,
		baseInformation: response,
		totalResponseMs: endTimeDifference
	};
};
