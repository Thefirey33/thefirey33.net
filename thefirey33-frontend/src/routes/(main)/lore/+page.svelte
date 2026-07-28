<script lang="ts">
    import BackgroundElement from "$lib/components/BackgroundElement.svelte";

    import LoreWallpaper from "$lib/assets/img/wallpapers/loreWallpaper.png"
    import ZoomIn from "$lib/assets/img/documentreader/zoomin.png"
    import ZoomOut from "$lib/assets/img/documentreader/zoomout.png"
    import Dock from "$lib/assets/img/documentreader/docktoleft.png"

    let {data} = $props()

    // Definitions for the document reader's general options.
    const minFontSize = 18;
    const maxFontSize = 50;
    const removalAddition = 3;


    function addRemoveZoomPortion(zoomPortion: number){
        fontSize = Math.min(maxFontSize, Math.max(minFontSize, fontSize + zoomPortion));
    }

    // The general TailwindCSS for the zoom buttons.
    const zoomButtonsCss = "text-white hover:translate-y-1 active:translate-y-1.5 cursor-pointer";
    // The CSS for when the button is disabled. (Reached limit)
    const disabledZoomButtonCss = "pointer-events-none opacity-50 cursor-not-allowed";
    let panelDock = $state(false);

    let fontSize = $state(minFontSize);

</script>

<BackgroundElement urlBackground={LoreWallpaper}/>

<div oncontextmenu={(e) => e.preventDefault()} tabindex="0" role="menubar" class="{panelDock ? "bg-black/30" : "bg-black"} left-15 {panelDock ? "-translate-x-full" : ""} border-2 select-none {!panelDock ? "border-white" : "border-white/30"} min-w-20 not-lg:bottom-5 transition-all h-20 p-4 items-center gap-x-4 flex flex-row fixed">
    <button class={`${zoomButtonsCss} ${fontSize >= maxFontSize ? disabledZoomButtonCss : ""}`} onclick={() => addRemoveZoomPortion(removalAddition)}>
        <img alt="Zoom In Icon" src={ZoomIn}/>
    </button>
    <button class={`${zoomButtonsCss} ${fontSize <= minFontSize ? disabledZoomButtonCss : ""}`} onclick={() => addRemoveZoomPortion(-removalAddition)}>
        <img alt="Zoom Out Icon" src={ZoomOut}/>
    </button>
    <p class="text-white">
        {fontSize}px
    </p>
    <button class={zoomButtonsCss} onclick={() => panelDock = !panelDock}>
        <img src={Dock} class="{panelDock ? "rotate-180" : ""} transition-all" alt="Dock To ${panelDock ? "Right": "Left"}"/>
    </button>
</div>
<section style="font-size: {fontSize}px;">
    <h1 class="text-3xl text-center text-white">Lore</h1>
    <p class="text-white md:text-xl text-xs not-xl:justify-between flex flex-row items-center gap-4 font-bold mb-5 p-2 bg-red-500/20">
        This background is made by Niko_Solar.
        <a class="animate-pulse text-nowrap bg-black p-2 border-4 border-red-500 hover:bg-red-500 transition-all hover:text-black" href="https://nikos-silly-space.webflow.io/#top-banner" rel="external">Follow them!</a>
    </p>
    {#each data.loreData as loreDataPortion, index (index)}
        {#if (loreDataPortion != 'LINEBREAK')}
            <p class="text-white wrap-anywhere">{loreDataPortion}</p>
        {:else}
            <br/>
        {/if}
    {/each}
</section>
