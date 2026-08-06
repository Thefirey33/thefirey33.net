<script lang="ts">
    import MetaTagsElement from "$lib/components/other/MetaTagsElement.svelte";
    import BackgroundElement from "$lib/components/BackgroundElement.svelte";
    import AdminWallpaper from "$lib/assets/img/wallpapers/adminWallpaper.png"
    import Thefirey33AdminLogo from "$lib/assets/img/icons/tadmin.png"
    import "../layout.css"
    import {resolve} from "$app/paths";
    import ToastNotificationElement from "$lib/components/other/ToastNotificationElement.svelte";
    import ProgressBarElement from "$lib/components/other/ProgressBarElement.svelte";

    let {data, children} = $props();
</script>

<MetaTagsElement
        description="Admin Page for Thefirey33's Portfolio."
        image="https://thefirey33.net/front.png"
        title="thefirey33 - admin"
        web="https://thefirey33.net/"
/>
<BackgroundElement urlBackground={AdminWallpaper}/>

<nav class="bg-black mb-5 z-30 flex flex-row items-center gap-4 w-screen min-h-15 border-(--border-color) border-b-4 p-2">
    <a class="text-white flex-row flex not-md:justify-center items-center gap-x-4 flex-wrap text-xl group hover:bg-white hover:text-black transition p-4"
       href={resolve("/admin")}>
        <img alt="Thefirey33 Admin Logo" class="group-hover:invert transition" src={Thefirey33AdminLogo}/>
        Thefirey33 Administrator
    </a>
    <a class="text-white"
       href={resolve("/")}>
        Back to Main
    </a>
</nav>
<ToastNotificationElement/>
<ProgressBarElement/>
<div class="max-w-400 min-w-90 justify-center h-full pt-25 not-md:justify-items-center overflow-hidden m-auto p-7">
    {@render children()}
    <footer class="text-white mt-5">
        {#if (data.info.success && data.info.message !== undefined)}
            <p>Uptime: {data.info.message.uptime}</p>
            <p>Operating System: {data.info.message.os_name}</p>
            <p>Machine: {data.info.message.machine_name}</p>
        {/if}
    </footer>
</div>

<style>
    :root {
        background-color: black;
    }
</style>