<script lang="ts">
    import BackgroundElement from "$lib/components/BackgroundElement.svelte";
    import MinecraftWallpaper from "$lib/assets/img/wallpapers/mcWallpaper.png"
    import BackendWarning from "$lib/components/other/BackendWarning.svelte";

    let {data} = $props();
</script>


<BackgroundElement urlBackground={MinecraftWallpaper}/>

<h1 class="text-white text-center text-3xl font-bold">The FireServer: Minecraft Server</h1>
{#if !data.minecraftServerInformation.success}
    <BackendWarning errorMessage={data.minecraftServerInformation.errorMessage}/>
{/if}