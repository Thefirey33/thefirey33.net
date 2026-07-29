<script lang="ts">
    import DexStatusWallpaper from "$lib/assets/img/wallpapers/dexStatusWallpaper.png";
    import BackgroundElement from "$lib/components/BackgroundElement.svelte";
    import BackIcon from "$lib/assets/img/icons/back.png"

    let {data} = $props();
</script>

<BackgroundElement urlBackground={DexStatusWallpaper}/>

<button class="btn mb-5 flex flex-row gap-4 group" onclick={() => history.back()}>
    <img alt="Back" class="group-hover:invert transition" src={BackIcon}/>
    Go back
</button>
{#if (data.results.success && data.results.message !== undefined)}
    <table class="text-white md:text-xl w-full">
        <thead>
        <tr>
            <th>Place</th>
            <th>Name</th>
            <th>Count</th>
        </tr>
        </thead>
        <tbody>
        {#each data.results.message as result, index (index)}
            <tr class="group cursor-cell">
                <td class="text-center group-hover:bg-white group-hover:text-black transition">{index + 1}</td>
                <td class="group-hover:bg-white group-hover:text-black transition">{result.author === "" ? "(Not claimed by anyone!)" : result.author}</td>
                <td class="text-center group-hover:bg-white group-hover:text-black transition">{result.count}</td>
            </tr>
        {/each}
        </tbody>
    </table>
{/if}