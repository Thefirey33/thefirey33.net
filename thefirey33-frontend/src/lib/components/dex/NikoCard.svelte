<script lang="ts">
    import {setToast} from "$lib/toast-helper";
    import ExpandIcon from "$lib/assets/img/dex/expand.png";
    import {resolve} from "$app/paths";
    import DownloadJsonIcon from "$lib/assets/img/dex/json.png";
    import CloseIcon from "$lib/assets/img/dex/close.png";
    import DownloadImageIcon from "$lib/assets/img/dex/image.png";
    import ViewIcon from "$lib/assets/img/dex/view.png";

    /**
     * Several Easter Egg quotes that might appear instead of the usual user.
     */
    const easterEggQuotes: Map<string, string> = new Map<string, string>([
        ["thefirey33", "the firey of thirty three"],
        ["nikodev", "don't awaken the evil spirit of the vedokin..."],
        ["niko_solar", "*insert image of gunna fire writing*"],
        ["grabbedcat", "the cat that has been grabbed"],
        ["smiling_niko", "that's a lot of nikosonas goddayum"],
        ["sudoker0", "patpat"],
        ["coodles", "the most brainrotted cuddly being ever"],
        ["karll0424", "kit or nikkit?"],
        ["snowi", "the orin"],
        ["pretzel", "unsalt the pretzel"],
        ["spooningtonIII", "v/s intercom voice"],
        ["whatsapp_niko", "*WhatsApp Sound Effect*"],
        ["sketchydoof23", "TENNA!!!!!"],
        ["devsun", "is devsun an actual developer? find out soon on 'Are They A Developer?'"],
        ["rainwater", "the rain of water"],
        ["nightmargin", "thanks for creating oneshot nightmargin."],
        ["_adamgamer2370", "sea salt"],
        ["[restricted]", ""]
    ]);

    /**
     * The maximum size of the name.
     */
    const maximumSizeOfName = 10;

    let {isBlacklisted, id, name, fullDesc, abilities, description, authorName, websiteStatus}: {
        isBlacklisted: boolean,
        name: string,
        id: number,
        fullDesc: string,
        abilities: { name: string }[],
        description: string,
        authorName: string,
        websiteStatus: boolean
    } = $props()

    let isExpanded = $state(false)
</script>

{#if isExpanded}
    <div class="bg-black/90 fixed left-0 top-0 w-screen h-screen z-30"></div>
{/if}
<div
        class="border-4 {isExpanded ? "fixed z-60 left-[50%] top-[50%] translate-y-[-50%] translate-x-[-50%] max-w-200 min-w-90" : "w-full max-w-full"} text-white border-(--border-color) p-4 bg-black flex gap-4 flex-col {!isExpanded ? "md:flex-row": ""}"
>
    <div class="flex w-full min-w-50 min-h-50 flex-col items-center gap-2 bg-black">
        <button class="{!isBlacklisted ? "cursor-grab" : "cursor-not-allowed"} min-h-50 min-w-50" onclick={() => {
                            if (isBlacklisted){
                                setToast(`${name} refuses to be patted!`)
                            }
                        }}>
            <img alt="Niko" class="w-50 h-50 [image-rendering:pixelated]" height="50"
                 src={`/api/data/dexrecovery/image/${id}`} width="50"/>
        </button>
        <button class="btn text-xl flex flex-row md:justify-center {isExpanded ? "justify-center": ""} text-center gap-x-4 w-full items-center group"
                onclick={() => isExpanded = !isExpanded}>
            <img alt="Expand Icon" class="group-hover:invert transition {isExpanded ? "hidden": ""}" src={ExpandIcon}>
            <img alt="Close Icon" class="group-hover:invert transition {!isExpanded ? "hidden": ""}" src={CloseIcon}>
            Description
        </button>
        {#if (!isExpanded)}
            <a class="btn text-xl flex flex-row md:justify-center gap-x-4 w-full items-center group" download href={resolve("/api/data/dexrecovery/niko/[id]", {
                            id: id.toString(),
                        })}>
                <img alt="Download Icon" class="group-hover:invert transition" src={DownloadJsonIcon}/>
                Download JSON
            </a>
            <a class="btn text-xl flex gap-x-4 w-full md:justify-center items-center group" download href={resolve("/api/data/dexrecovery/image/[id]", {
                            id: id.toString(),
                        })}>
                <img alt="Download Icon" class="group-hover:invert transition" src={DownloadImageIcon}/>
                Download Image
            </a>
            <a class="btn {!websiteStatus ? "pointer-events-none opacity-50": ""} md:justify-center w-full text-xl flex text-center gap-x-4 items-center group"
               href={`https://nikodex.net/noik/${id}`}
               rel="external">
                <img alt="Download Icon" class="group-hover:invert transition" src={ViewIcon}/>
                View on NikoDex
            </a>
        {/if}
    </div>
    <div class="info grow w-full relative text-xl">
        <p class="absolute right-0 bottom-0 text-gray-500 {isExpanded ? "hidden": ""}">
            <em>#{id}</em>
        </p>

        {#if name.length > maximumSizeOfName}
            <h2 class="text-3xl font-bold w-fit" title={name}>{name.substring(0, maximumSizeOfName)}...</h2>
        {:else}
            <h2 class="text-3xl font-bold w-fit">{name}</h2>
        {/if}

        <p class="wrap-anywhere"><em>"{description}"</em></p>
        <p>
            By:
            <button class="bg-white text-black px-1 break-all m-1 cursor-pointer" onclick={() => {
                                const author = authorName.toLowerCase();
                                if (easterEggQuotes.has(author))
                                    setToast(easterEggQuotes.get(author) ?? "typescript problems, whoopsies!")
                                else
                                    setToast("Accounts can be accessed in the main site!")
                            }}>{authorName}</button>
        </p>
        <p class="bg-white text-black w-fit px-1">Abilities:</p>
        <ul class="list-disc list-inside mb-10 max-h-40 overflow-scroll">
            {#each abilities as ability, idx (idx)}
                <li class="wrap-break-word">{ability.name}</li>
            {:else}
                <li>
                    <em>No abilities have been specified.</em>
                </li>
            {/each}
        </ul>
    </div>
    {#if (isExpanded)}
        <p class="text-white max-h-30 not-md:max-w-80 ring-1 p-2 text-xl overflow-scroll">{fullDesc}</p>
    {/if}
</div>