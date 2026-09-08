<script lang="ts">
	import BackgroundElement from '$lib/components/BackgroundElement.svelte';
	import BackgroundImage from '$lib/assets/img/wallpapers/artsWallpaper.png';
	import CloseableMenu from '$lib/components/other/CloseableMenu.svelte';
	import { resolve } from '$app/paths';
	import BackendWarningElement from '$lib/components/other/BackendWarningElement.svelte';
	import ReadDisclaimerFirst from '$lib/components/other/ReadDisclaimerFirst.svelte';

	let { data } = $props();
</script>

<BackgroundElement urlBackground={BackgroundImage} />

<div
	class="flex w-full flex-col gap-4 border-4 border-(--border-color) bg-black p-4 text-center text-white md:text-xl"
>
	{#if !data.success || data.data === undefined}
		<BackendWarningElement errorMessage={data.errorMessage} />
	{:else}
		<h1 class="text-3xl">Arts!</h1>
		<ReadDisclaimerFirst />
		<p>Each of these arts are made by very cool ppl and they deserve attention!! :3</p>
		<p>I appreciate all of it, i love y'all &lt;3 /p!!</p>
		<em>To submit an art piece, send it to me via DMs!</em>
		{#each data.data as artData, index (index)}
			<CloseableMenu title={`Category: ${artData[0]}`}>
				<div class="flex grid-flow-dense flex-col gap-4 md:grid md:grid-cols-2">
					{#each artData[1] as artDataPortion, index (index)}
						<a
							class="group flex h-full flex-col items-center gap-3 border-2 border-(--border-color) p-2 transition-all hover:bg-(--border-color) hover:text-black md:p-4"
							href={resolve('/api/data/[uuid]?pr=true', {
								uuid: artDataPortion.uuid
							})}
						>
							<img
								draggable="false"
								oncontextmenu={(e) => e.preventDefault()}
								width="200"
								class="h-full p-1 ring-2 ring-white transition-all group-hover:ring-black xl:w-[60%]"
								src={`/api/data/${artDataPortion.uuid}?pr=true`}
								alt="Art!"
							/>
							<div class="m-auto flex flex-col text-center">
								<h1 class="md:text-3xl">
									{artDataPortion.title}
									<em>({artDataPortion.author})</em>
								</h1>
								<p>{artDataPortion.description}</p>
							</div>
						</a>
					{/each}
				</div>
			</CloseableMenu>
		{/each}
	{/if}
</div>
