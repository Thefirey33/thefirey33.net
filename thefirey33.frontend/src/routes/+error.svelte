<script lang="ts">
    import './layout.css';
    import MetaTagsElement from "$lib/components/other/MetaTagsElement.svelte";
    import BackgroundElement from "$lib/components/BackgroundElement.svelte";
    import BackgroundImage from "$lib/assets/img/wallpapers/notFoundWallpaper.png"

    import {page} from '$app/state';
</script>

<MetaTagsElement
        description="the thefirey33 network!"
        image=""
        title="thefirey33 - not found!"
        web="https://thefirey33.net/"
/>
<BackgroundElement urlBackground={BackgroundImage}/>
<div class="m-auto items-center fixed left-[50%] top-[50%] translate-x-[-50%] flex flex-col translate-y-[-50%]">
    <h1 class="text-white md:text-6xl text-3xl font-bold">{page.status}: {page.error?.message}</h1>
    <p class="text-white md:text-xl">Unfortunately, the page you requested has thrown this error!</p>
    {#if page.status != 404}
        <p class="text-white md:text-2xl">Contact the developer if possible!</p>
    {/if}
</div>