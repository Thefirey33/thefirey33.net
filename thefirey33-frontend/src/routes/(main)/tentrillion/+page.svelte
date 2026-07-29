<script lang="ts">
    import BackgroundElement from "$lib/components/BackgroundElement.svelte";
    import TenTrillionWallpaper from "$lib/assets/img/wallpapers/tentrillionWallpaper.png"
    import ReadCommitIcon from "$lib/assets/img/icons/read.png"
    import TenTrillionIcon from "$lib/assets/img/icons/tentrillion.png"
    import BackendWarningElement from "$lib/components/other/BackendWarningElement.svelte";

    let {data} = $props();

    // The CSS style of how the lists are displayed.
    const listCssStyle = "ml-10";
    // The CSS style of the descriptions.
    const descriptionCssStyle = "text-left md:text-xl w-full";

</script>

<BackgroundElement urlBackground={TenTrillionWallpaper}/>

<section class="min-w-70 m-auto flex flex-col gap-4 items-center">
    <h1 class="text-white text-3xl">
        TenTrillion Game Engine
        <a class="btn w-full text-center flex group flex-row justify-center gap-3 items-center animate-pulse"
           href="https://github.com/tentrillion-game-engine">
            <img alt="TenTrillion Icon" class="group-hover:invert transition" src={TenTrillionIcon}/>
            Check it out!
        </a>
    </h1>
    <p class={`${descriptionCssStyle} text-white`}>
        The TenTrillion is a highly performant game engine, written entirely from scratch in
        Vulkan
        and C++. It's main
        goal is to provide the optimizations needed so developers <em>don't have to!</em>
    </p>
    <p class={`${descriptionCssStyle} bg-white p-2 text-black`}>
        Along with that, making it easier to make a optimized Vulkan game, without needing to write Vulkan code!
    </p>
    <div class="text-white  md:text-xl w-full">
        The game engine's architecture is:
        <ul class="list-decimal">
            <li class={listCssStyle}>
                Vulkan, OpenGL, OpenAL and OpenCL for the backend.
            </li>
            <li class={listCssStyle}>
                Qt, linked with the backend for the frontend.
            </li>
            <li class={listCssStyle}>
                <strong>Easy Mode:</strong> Allowing the users to use the frontend (Editor) to quickly make games!
            </li>
            <li class={listCssStyle}>
                <strong>Library Mode:</strong> Allowing the advanced users to have more control by directly calling
                TenTrillion
                Functions!
            </li>
        </ul>
    </div>

    <span class="max-h-120 w-full overflow-auto border-2 border-(--border-color) scrollbar-gutter-stable">

        {#if (!data.gitData.success)}
            <BackendWarningElement errorMessage={data.gitData.errorMessage}/>
        {:else}
            {#each data.gitData.message as gitCommit, index (index)}
                <div class="bg-black w-full border-2 flex flex-col gap-4 p-4 border-(--border-color)">
                    <p class="text-white flex flex-row items-center gap-4">
                        <a href={gitCommit.html_url} class="text-white">
                        {gitCommit.sha.substring(0, 10)}
                        </a>
                        <a href={gitCommit.author.html_url}
                           class="flex flex-row items-center gap-4 hover:bg-white hover:text-black p-2 transition">
                            <img width="50" height="50" src={gitCommit.author.avatar_url}
                                 alt="Profile Picture of {gitCommit.author.login}">
                            {gitCommit.author.login}
                        </a>
                    </p>
                <p class="text-white">{gitCommit.commit.message}</p>
                    <a href={gitCommit.html_url}
                       class="text-white text-xl group items-center btn flex-wrap flex justify-center gap-4">
                    <img src={ReadCommitIcon} class="group-hover:invert transition" alt="Hand Pointing to Right Icon"/>
                    <p class="group-hover:invert transition">Read Commit {gitCommit.sha.substring(0, 10)}</p>
                    </a>
                </div>
            {/each}
        {/if}
    </span>
</section>