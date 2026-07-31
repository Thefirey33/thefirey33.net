<script lang="ts">
    import DexStatusWallpaper from "$lib/assets/img/wallpapers/dexStatusWallpaper.png"
    import BackgroundElement from "$lib/components/BackgroundElement.svelte";
    import type {Niko} from "$lib/types/dexrecovery";
    import {onMount} from "svelte";
    import {setToast} from "$lib/toast-helper";
    import DownloadBackupIcon from "$lib/assets/img/dex/backup.png"
    import BackendWarningElement from "$lib/components/other/BackendWarningElement.svelte";
    import LeaderboardIcon from "$lib/assets/img/dex/leaderboard.png"

    import {resolve} from "$app/paths";
    import NikoCard from "$lib/components/dex/NikoCard.svelte";
    import ReadDisclaimerFirst from "$lib/components/other/ReadDisclaimerFirst.svelte";
    let {data} = $props();

    /**
     * The different grades of responses.
     * This is based on how quick the dex can respond to the specified request.
     */
    const responseGrades: { grade: string, color: string, rangeEnd?: number }[] = [
        {
            grade: "Fast",
            color: "rgb(0,255,0)",
            rangeEnd: 100
        },
        {
            grade: "Normal",
            color: "rgb(255,169,80)",
            rangeEnd: 500
        },
        {
            grade: "Delayed",
            color: "rgb(206 89 13)",
            rangeEnd: 1000
        },
        {
            grade: "Extremely Delayed!",
            color: "rgb(255,0,0)"
        }
    ]


    let currentPage = $state(0);
    let currentNikos: Niko[] = $state([]);

    /**
     * Fetch the nikos from the backend.
     * @param page The page to fetch.
     */
    async function fetchNikos(page: number){
        currentNikos.length = 0;
        isOverScrollBoundary = false;

        const result: {
            message: Niko[] | undefined,
            success: boolean,
            errorMessage?: string
        } = await fetch(`/api/data/dexrecovery/page/${page}`)
            .then(response => response.json());

        if (!result.success || result.message === undefined) {
            setToast("Failed to fetch Nikos!");
            return;
        }

        currentNikos = result.message;
        isOverScrollBoundary = false;
    }

    /**
     * Change the specified page.
      * @param page The amount of times to change it for.
     */
    function changePage(page: number){
        currentPage += page;
        localStorage.setItem(pageStateKey, currentPage.toString())
        fetchNikos(currentPage);
    }

    /**
     * Get the grade of the response via the response time.
     * @param responseTime Response time that will be graded.
     */
    function getResponseGrade(responseTime: number) {
        return responseGrades.find(value => {
            if (value.rangeEnd === undefined) {
                return value;
            } else if (responseTime < value.rangeEnd)
                return value
        });
    }

    function getScrollBoundary() {
        return Math.max(
            document.body.scrollHeight,
            document.documentElement.scrollHeight
        ) - window.innerHeight
    }

    const scrollStateKey = "scroll";
    const pageStateKey = "page";
    const scrollBoundaryOffset = 800

    let isOverScrollBoundary = $state(false)
    let boundary = $state(0)

    onMount(async () => {

        // When I was navigating around with the NikoDex external links, constantly being shoved back into the top,
        // Kinda made me annoyed not going to lie.
        // So this exists, to combat that.

        const scrollListener = () => {
            boundary = getScrollBoundary();
            isOverScrollBoundary = window.scrollY >= boundary - scrollBoundaryOffset
            localStorage.setItem(scrollStateKey, String(window.scrollY))
        };

        // Attempt to get the page that was switched to in the context.
        const page = localStorage.getItem(pageStateKey)
        if (page !== null) {
            currentPage = Number.parseInt(page);
        } else {
            localStorage.setItem(pageStateKey, currentPage.toString());
        }
        await fetchNikos(currentPage);

        requestAnimationFrame(() => {
            const scrollState = localStorage.getItem(scrollStateKey);

            if (scrollState !== null) {
                window.scrollTo(0, Number.parseInt(scrollState))
            }
        })

        addEventListener("scroll", scrollListener)
    })

    const responseGrade = $derived(getResponseGrade(data.totalResponseMs))
</script>

{#if (!data.baseInformation.success)}
    <BackendWarningElement errorMessage="Failure to contact backend"/>
{/if}

<BackgroundElement urlBackground={DexStatusWallpaper}/>

<h1 class="text-white text-3xl text-center mb-5">
    Thefirey33 NikoDex Tracker
</h1>

<p class="text-white md:text-xl text-center">
    This is the NikoDex Status tracker created by me, that tracks the NikoDex. It also takes a backup of
    the website, so you can still browse Nikos if the website is down!
</p>

<ReadDisclaimerFirst/>

<div class="p-4 text-white items-center w-fit border-(--border-color) flex xl:flex-row flex-col gap-4 m-auto mt-5">
    <div class="flex flex-row gap-x-4 p-2 text-center items-center">Website
        API Status: <p
                class="{data.apiStatus ? "bg-green-500": "bg-red-500"} p-2 text-black">{data.websiteStatus ? "Working" : "Broken!"}</p>
    </div>
    <div class="flex flex-row gap-x-4 p-2 text-center items-center">Website
        Front-End Status: <p
                class="{data.websiteStatus ? "bg-green-500": "bg-red-500"} p-2 text-black">{data.websiteStatus ? "Working" : "Broken!"}</p>
    </div>
    <div class="flex flex-row gap-4 items-center text-center">
        NikoDex Response to Thefirey33 Server In: {Math.floor(data.totalResponseMs)}ms/{data.totalResponseMs / 1000}s
        {#if (responseGrade !== undefined)}
            <p class="p-2 text-black" style="background-color: {responseGrade.color};">{responseGrade.grade}</p>
        {/if}
    </div>
</div>

<div class="w-full items-center flex flex-col gap-4 justify-center md:m-4 mb-4">
    {#if (data.baseInformation.message !== undefined)}
        <p class="text-white text-xl">This backup was taken on {data.baseInformation.message.date}</p>
    {/if}
    <div class="flex flex-row gap-4">
        <a download href={resolve("/api/data/dexrecovery/zip")} class="btn flex items-center not-md:justify-center text-center flex-wrap gap-x-3 group">
            <img src={DownloadBackupIcon} class="group-hover:invert transition text-wrap" alt="Download Entire Backup"/>
                Download Backup
            </a>
        <a href={resolve("/dex/leaderboard")} class="btn items-center not-md:justify-center flex text-center gap-x-3 flex-wrap group">
            <img class="group-hover:invert transition"  src={LeaderboardIcon} alt="Leaderboard"/>
            Leaderboard
        </a>
    </div>
</div>

<!-- All of the items stored in the database goes here. -->
{#if currentNikos.length <= 0}
    <p class="text-white text-xl">Loading...</p>
{:else}
    <div class="grid 2xl:grid-cols-3 xl:grid-cols-2 w-full gap-4">
        {#each currentNikos as niko, index (index)}
            <NikoCard fullDesc={niko.full_desc} id={niko.id} description={niko.description} authorName={niko.author_name} isBlacklisted={niko.is_blacklisted} name={niko.name} abilities={niko.abilities} websiteStatus={data.websiteStatus} />
        {/each}
    </div>
{/if}

{#if data.baseInformation.message !== undefined}
    <div class="{!isOverScrollBoundary ? "fixed" : ""} pointer-events-none left-0 bottom-5 w-full mt-5 items-center">
        <div class="bg-black min-w-20 md:gap-x-10 gap-x-5 p-4 pointer-events-auto min-h-15 w-fit flex flex-row border-4 items-center justify-center text-xl border-(--border-color) m-auto">
            <button onclick={() => changePage(-1)} class="btn {currentPage <= 0 ? "pointer-events-none opacity-50": ""}">Prev</button>
            <p class="text-white text-center">Page {currentPage + 1}/{data.baseInformation.message.pages + 1}</p>
            <button onclick={() => changePage(1)} class="btn {currentPage >= data.baseInformation.message.pages ? "pointer-events-none opacity-50": ""}">Next</button>
        </div>
    </div>
{/if}