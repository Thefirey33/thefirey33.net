<script lang="ts">
	import { onMount } from 'svelte';
	import BackendWarningElement from '$lib/components/other/BackendWarningElement.svelte';
	import { setToast } from '$lib/toast-helper';
	import RefreshIcon from '$lib/assets/img/admin/refresh.png';

	let apiFailure = $state(false);
	let apiLoading = $state(false);
	let approvalData: Approval[] = $state([]);

	/**
	 * Change the state of the approval data.
	 * @param index The ID of the data.
	 * @param uuid The UUID of the player.
	 * @param state The state to set to.
	 */
	async function changeState(index: number, uuid: string, state: boolean) {
		apiLoading = true;
		const result = await fetch(`/api/approvals/${uuid}?approved=${state}`, {
			method: 'PUT'
		}).then((r) => r.ok);

		if (result) {
			const dataSection = approvalData[index];
			setToast(`Successfully changed the state of ${dataSection.username}`);

			dataSection.approved = state;

			// Set back the data to the specified.
			approvalData[index] = dataSection;
		} else setToast('Failed to change state!');

		apiLoading = false;
		return result;
	}

	/**
	 * Get all the approval data.
	 */
	async function getApprovalData() {
		apiLoading = true;

		const data: {
			message: Approval[] | undefined;
			success: boolean;
			errorMessage?: string;
		} = await fetch('/api/data/approvals').then((res) => res.json());

		apiFailure = !data.success;

		if (data.success && data.message !== undefined) {
			approvalData = data.message;
		}

		apiLoading = false;
	}

	onMount(async () => {
		await getApprovalData();
	});
</script>

{#if apiFailure}
	<BackendWarningElement errorMessage="Backend failed to fetch!" />
{/if}

<button class="btn group mb-5 flex gap-4" onclick={async () => getApprovalData()}>
	<img alt="Refresh" class="transition group-hover:invert" src={RefreshIcon} />
	Refresh
</button>

<p class="mb-5 text-white">
	This is for the entry to the Minecraft Server. When someone attempts to join the server, their
	request to join is logged here.
</p>
{#if approvalData.length <= 0 || apiLoading}
	{#if apiLoading}
		<p class="text-xl text-white">Loading</p>
	{:else}
		<p class="text-xl text-white">No pending approval requests yet.</p>
	{/if}
{:else}
	<table>
		<thead>
			<tr>
				<td>Id</td>
				<td>Uuid</td>
				<td>Username</td>
				<td>Are they allowed for entry?</td>
			</tr>
		</thead>
		<tbody>
			{#each approvalData as approval, index (index)}
				<tr>
					<td>{approval.id}</td>
					<td>{approval.username}</td>
					<td>{approval.uuid}</td>
					<td>
						<p class="mb-3">Currently, they {approval.approved ? 'can' : "can't"} enter.</p>
						<button
							onclick={() => changeState(index, approval.uuid, true)}
							class="btn mb-3 text-base {approval.approved ? 'pointer-events-none opacity-50' : ''}"
						>
							Allow
						</button>
						<button
							onclick={() => changeState(index, approval.uuid, false)}
							class="btn text-base {!approval.approved ? 'pointer-events-none opacity-50' : ''}"
						>
							Don't Allow
						</button>
					</td>
				</tr>
			{/each}
		</tbody>
	</table>
{/if}
