<script lang="ts">
    import BackgroundElement from "$lib/components/BackgroundElement.svelte";
    import MinecraftWallpaper from "$lib/assets/img/wallpapers/mcWallpaper.png"
    import BackendWarningElement from "$lib/components/other/BackendWarningElement.svelte";
    import {setToast} from '$lib/toast-helper';

    let {data} = $props();

    const minecraftServerLink = "mc.thefirey33.net";

    /**
     * Return the specified uptime stamp to a formatted time string.
     * @param totalSeconds The total uptime seconds.
     */
    function formatUptime(totalSeconds: number): string {
        const days = Math.floor(totalSeconds / 86400);
        const hours = Math.floor((totalSeconds % 86400) / 3600);
        const minutes = Math.floor((totalSeconds % 3600) / 60);
        const seconds = Math.floor(totalSeconds % 60);

        const parts: string[] = [];
        if (days > 0) parts.push(`${days}d`);
        if (hours > 0) parts.push(`${hours}h`);
        if (minutes > 0) parts.push(`${minutes}m`);
        if (seconds > 0 || parts.length === 0) parts.push(`${seconds}s`);

        return parts.join(' ');
    }

</script>


<BackgroundElement urlBackground={MinecraftWallpaper}/>

<h1 class="text-white text-center text-3xl font-bold">My Minecraft Server!</h1>
{#if !data.minecraftServerInformation.success}
    <BackendWarningElement errorMessage={data.minecraftServerInformation.errorMessage}/>
{:else if (data.minecraftServerInformation.message !== undefined)}
    <div class="fixed bg-black border-4 border-(--border-color) min-w-80 p-4 left-[50%] top-[50%] items-center flex flex-col translate-x-[-50%] translate-y-[-50%]">
        <p class="text-white text-xl">Server
            Uptime: {formatUptime(data.minecraftServerInformation.message.serverUptime / 1000)}</p>
        {#if (data.minecraftServerInformation.message.currentPlayers.length <= 0)}
            <p class="text-white md:text-2xl font-bold text-center">No players online.. Maybe you can join?</p>
        {:else}
            <p class="text-white md:text-2xl">Online Players:</p>
            <ul class="list-disc">
                {#each data.minecraftServerInformation.message.currentPlayers as player, index (index)}
                    <li class="text-white ml-20">{player}</li>
                {/each}
            </ul>
        {/if}

        <p class="bg-white text-black w-full text-center md:text-xl mt-5">
            Join with:
            <button class="underline cursor-pointer" title="Copy to clipboard" onclick={() => {
                setToast("Copied to clipboard");
                navigator.clipboard.writeText(minecraftServerLink);
            }}>{minecraftServerLink}</button>
        </p>

        <p class="text-white mt-5">
            After you join, the system will automatically kick you. That's okay! You will get approved
            for entry later, where you can retry, and if you are approved for entry, you will be able to join!
        </p>
    </div>
{/if}