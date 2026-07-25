<script lang="ts">
    import BackgroundElement from "$lib/components/BackgroundElement.svelte";
    import BackgroundImage from "$lib/assets/img/wallpapers/codingWallpaper.png"
    import ReadIcon from "$lib/assets/img/icons/read.png"
    import ProgrammingDetails from "$lib/assets/data/programmingDetails.json"

    let {data} = $props();

    /**
     * Retrieve the background color of a selected language.
     * @param text The language name.
     */
    function retrieveColorFromList(text: string) {
        switch (text){
            case "Svelte":
                return "rgb(150, 0, 0, 255)";
            case "GDScript":
            case "Lua":
                return "rgb(0 0 140)"
            case "TypeScript":
            case "C++":
                return "rgb(78 78 255)"
            case "Python":
                return "rgb(122 109 0)"

        }
        return undefined;
    }

    /**
     * This automatically generates a color for the specified language. If the color already exists in the switch case,
     * a color will not be generated and instead, the color from that switch will be chosen.
     * @param text The name of the language.
     */
    function getColor(text: string) {
        // Some of them come from a predefined list, so ignore the automatic color generation when necessary.
        const result = retrieveColorFromList(text);
        if (result !== undefined)
            return result;

        let colorValue = 0;
        for (let i = 0; i < text.length; i++) {
            colorValue += text.charCodeAt(i);
        }
        return `hsl(${colorValue}, 100%, 30%)`;
    }
</script>

<BackgroundElement urlBackground={BackgroundImage}/>

<div class="flex flex-col lg:gap-8 items-center">
    <section class="text-white md:text-xl mb-5 flex flex-col gap-4 flex-wrap">
        <h1 class="text-white text-center md:text-3xl">Programming</h1>
        <p>Being my main interest, I have learned a lot of programming languages and made a lot of projects!</p>
        <p>All of them with their unique purposes! Some projects I've been involved in are in organizations, so check out my GitHub page for a list of all my projects!</p>
        <p>Along with many other fields in CyberSecurity, Artificial Intelligence, Game Design and the Cloud that I have skills in!</p>
    </section>

    <div class="w-fit m-auto grid gap-4 xl:grid-cols-14 md:grid-cols-7 grid-cols-4 transition-all bg-black">
        {#each ProgrammingDetails as programmingData, index (index)}
            <a href={programmingData.learn_more} rel="external" class="border-2 group border-(--border-color) hover:bg-(--border-color) transition-all p-4 items-center justify-center flex">
                <img class="not-2xl:max-w-12 max-h-15 group-hover:ring-2 p-2 ring-white transition-all" src={`code/${programmingData.identifier}.png`} width="100" height="100" alt="Logo of {programmingData.name}"/>
            </a>
        {/each}
    </div>
</div>

<div class="flex flex-col gap-4 mt-10">
    {#each data.repositories as dataPortion, index (index)}
        <section class="bg-black border-4 border-(--border-color) flex flex-col gap-2 p-4">
            <div class="text-white flex truncate items-center flex-wrap gap-4 md:text-3xl not-md:font-bold">
                <h2>{dataPortion.name}</h2>
                {#if (dataPortion.language !== null)}
                    <p class="md:text-xl text-xs items-center justify-center content-center p-2" style="background-color: {getColor(dataPortion.language || "fuck you typescript")}">{dataPortion.language}</p>
                {/if}
            </div>
            <em class="text-xs text-white">{new Date(Date.parse(dataPortion.created_at)).toLocaleDateString()}</em>
            <p class="text-white">{dataPortion.description}</p>
            <a class="btn group flex flex-row gap-4 justify-center" href={dataPortion.html_url}>
                <img class="group-hover:invert transition" src={ReadIcon} alt="Read Icon">
                Visit Repository
            </a>
        </section>
    {/each}
</div>