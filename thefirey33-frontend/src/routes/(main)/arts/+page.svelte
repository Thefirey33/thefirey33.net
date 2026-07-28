<script lang="ts">
    import BackgroundElement from "$lib/components/BackgroundElement.svelte";
    import BackgroundImage from "$lib/assets/img/wallpapers/artsWallpaper.png"
    import CloseableMenu from "$lib/components/other/CloseableMenu.svelte";
    import {resolve} from "$app/paths";
    import BackendWarning from "$lib/components/other/BackendWarning.svelte";

    let {data} = $props();


</script>

<BackgroundElement urlBackground={BackgroundImage}/>

<div class="bg-black text-white md:text-xl p-4 w-full flex text-center  flex-col gap-4 border-4 border-(--border-color)">
    {#if !data.success || data.data === undefined}
        <BackendWarning errorMessage={data.errorMessage}/>
    {:else}
        <h1 class="text-3xl">Arts!</h1>
        <p>Each of these arts are made by very cool ppl and they deserve attention!! :3</p>
        <p>I appreciate all of it, i love y'all &lt;3 /p!!</p>
        <em>To submit an art piece, send it to me via DMs!</em>
        {#each data.data as artData, index (index)}
            <CloseableMenu title={`Category: ${artData[0]}`}>
                <div class="md:grid md:grid-cols-2 flex flex-col grid-flow-dense gap-4">
                    {#each artData[1] as artDataPortion, index (index)}
                        <a class="group h-full border-2 hover:bg-(--border-color) transition-all hover:text-black gap-3 border-(--border-color) items-center md:p-4 p-2 flex xl:flex-row flex-col"
                           href={resolve("/api/data/[uuid]?pr=true", {
                            uuid: artDataPortion.uuid
                        })}>
                            <img draggable="false" oncontextmenu={(e) => e.preventDefault()} width="200"
                                 class="xl:w-[60%] h-full ring-2 transition-all group-hover:ring-black ring-white p-1"
                                 src={`/api/data/${artDataPortion.uuid}?pr=true`}
                                 alt="Art!"/>
                            <div class="flex flex-col text-center m-auto">
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