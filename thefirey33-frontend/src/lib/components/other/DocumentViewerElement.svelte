<script lang="ts">
    import ZoomIn from "$lib/assets/img/documentreader/zoomin.png"
    import ZoomOut from "$lib/assets/img/documentreader/zoomout.png"
    import Dock from "$lib/assets/img/documentreader/docktoleft.png"
    import {maxFontSize, minFontSize} from "$lib";

    let {fontSize = $bindable()} = $props()

    // Definitions for the document reader's general options.
    const removalAddition = 3;

    function addRemoveZoomPortion(zoomPortion: number) {
        fontSize = Math.min(maxFontSize, Math.max(minFontSize, fontSize + zoomPortion));
    }

    // The general TailwindCSS for the zoom buttons.
    const zoomButtonsCss = "text-white hover:translate-y-1 active:translate-y-1.5 cursor-pointer";
    // The CSS for when the button is disabled. (Reached limit)
    const disabledZoomButtonCss = "pointer-events-none opacity-50 cursor-not-allowed";
    let panelDock = $state(false);

</script>

<div class="{panelDock ? "bg-black/30" : "bg-black"} left-15 {panelDock ? "-translate-x-full" : ""} border-2 select-none {!panelDock ? "border-white" : "border-white/30"} min-w-20 not-lg:bottom-5 transition-all h-20 p-4 items-center gap-x-4 flex flex-row fixed"
     oncontextmenu={(e) => e.preventDefault()} role="menubar"
     tabindex="0">
    <button class={`${zoomButtonsCss} ${fontSize >= maxFontSize ? disabledZoomButtonCss : ""}`}
            onclick={() => addRemoveZoomPortion(removalAddition)}>
        <img alt="Zoom In Icon" src={ZoomIn}/>
    </button>
    <button class={`${zoomButtonsCss} ${fontSize <= minFontSize ? disabledZoomButtonCss : ""}`}
            onclick={() => addRemoveZoomPortion(-removalAddition)}>
        <img alt="Zoom Out Icon" src={ZoomOut}/>
    </button>
    <p class="text-white">
        {fontSize}px
    </p>
    <button class={zoomButtonsCss} onclick={() => panelDock = !panelDock}>
        <img alt="Dock To ${panelDock ? "Right": "Left"}" class="{panelDock ? "rotate-180" : ""} transition-all"
             src={Dock}/>
    </button>
</div>