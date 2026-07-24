<script lang="ts">
    import BackgroundElement from "$lib/components/BackgroundElement.svelte";
    import BackgroundImage from "$lib/assets/img/wallpapers/artsWallpaper.png"
    import CloseableMenu from "$lib/components/other/CloseableMenu.svelte";

    let {data} = $props();
</script>

<BackgroundElement urlBackground={BackgroundImage}/>

<div class="bg-black p-4 w-full flex flex-col gap-4 border-4 border-(--border-color)">
    <h1 class="text-white text-center text-3xl">Arts!</h1>
    <p class="text-white">Each of these arts are made by very cool ppl!!</p>
    {#each data.data as artData, index (index)}
        <CloseableMenu title={`Category: ${artData[0]}`}>
            <div class="grid grid-cols-2 grid-flow-dense gap-4">
                {#each artData[1] as artDataPortion, index (index)}
                    <article
                            class="border-2 gap-3 border-(--border-color) items-center md:p-4 p-2 flex xl:flex-row flex-col">
                        <img draggable="false" oncontextmenu={(e) => e.preventDefault()} width="200"
                             class="xl:w-[60%] ring-2 ring-white p-1"
                             src={`/api/data/${artDataPortion.uuid}?pr=true`}
                             alt="Art!"/>
                        <div class="flex flex-col text-center m-auto">
                            <h1 class="text-white md:text-3xl">
                                {artDataPortion.title}
                                <em class="text-white">({artDataPortion.author})</em>
                            </h1>
                            <p class="text-white">{artDataPortion.description}</p>
                        </div>
                    </article>
                {/each}
            </div>
        </CloseableMenu>
    {/each}
</div>