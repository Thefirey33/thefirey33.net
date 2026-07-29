<script lang="ts">
    import BackgroundElement from "$lib/components/BackgroundElement.svelte";

    import LoreWallpaper from "$lib/assets/img/wallpapers/loreWallpaper.png"
    import {minFontSize} from "$lib";
    import DocumentViewerElement from "$lib/components/other/DocumentViewerElement.svelte";

    let {data} = $props()
    let fontSize = $state(minFontSize);

</script>

<BackgroundElement urlBackground={LoreWallpaper}/>
<DocumentViewerElement bind:fontSize={fontSize}/>

<section style="font-size: {fontSize}px">
    <h1 class="text-3xl text-center text-white">Lore</h1>
    <p class="text-white md:text-xl text-xs not-xl:justify-between flex flex-row items-center gap-4 font-bold mb-5 p-2 bg-red-500/20">
        This background is made by Niko_Solar.
        <a class="animate-pulse text-nowrap bg-black p-2 border-4 border-red-500 hover:bg-red-500 transition-all hover:text-black"
           href="https://nikos-silly-space.webflow.io/#top-banner" rel="external">Follow them!</a>
    </p>
    {#each data.loreData as loreDataPortion, index (index)}
        {#if (loreDataPortion != 'LINEBREAK')}
            <p class="text-white wrap-anywhere">{loreDataPortion}</p>
        {:else}
            <br/>
        {/if}
    {/each}
</section>
