<script lang="ts">
    import BackgroundElement from "$lib/components/BackgroundElement.svelte";
    import BackgroundImage from "$lib/assets/img/wallpapers/aboutScreenWallpaper.png"

    import BigIconImage from "$lib/assets/img/other/frontpageIcon.png"
    import {resolve} from "$app/paths";

    import {Temporal} from "@js-temporal/polyfill";
    import SocialsData from "$lib/assets/data/json/socials.json"
    import {onMount} from "svelte";

    let temporalTime: Temporal.ZonedDateTime = $state(Temporal.Now.zonedDateTimeISO("Europe/Istanbul"));
    let clockCanvas: HTMLCanvasElement;

    function convertToPaddedTimeString(time: number) {
        return time.toString().padStart(2, "0");
    }

    onMount(() => {
        let handle = requestAnimationFrame(updateClock);
        const ctx = clockCanvas.getContext("2d");
        const widthRadii = clockCanvas.width / 2;
        const clockRadiusOffset = 5;
        const heightRadii = clockCanvas.height / 2;

        /**
         * This draws the specified hand with the value to the clock.
         * @param value THe value to draw with.
         */
        function drawHand(value: number) {
            if (ctx === null) {
                throw new Error("Canvas rendering context failure!")
            }

            ctx.beginPath();

            // Center the clock hand to the center.
            ctx.setTransform(1, 0, 0, 1, widthRadii, heightRadii)

            // Draw the hand using degrees -> radians.
            ctx.rotate(((Math.PI / 180) * (360 * value)));
            ctx.moveTo(0, 0)

            // Draw the hand.
            ctx.lineTo(0, (-heightRadii) + clockRadiusOffset)
            ctx.stroke()
            ctx.resetTransform()
        }


        /**
         * Updates the clock for viewing.
         */
        function updateClock() {
            if (ctx === null) {
                throw new Error("Canvas rendering context failure!")
            }

            temporalTime = Temporal.Now.zonedDateTimeISO("Europe/Istanbul");

            ctx.fillStyle = "black";
            ctx.strokeStyle = "white";

            // Render the background.
            ctx.resetTransform()
            ctx.clearRect(0, 0, clockCanvas.width, clockCanvas.height);

            ctx.beginPath();
            ctx.ellipse(widthRadii, heightRadii, widthRadii - clockRadiusOffset, heightRadii - clockRadiusOffset, 0, 0, Math.PI * 2);
            ctx.fill()
            ctx.stroke()

            // Render the clock hand.
            drawHand((temporalTime.hour + (temporalTime.minute / 60)) / 12)
            drawHand((temporalTime.second / 60))

            handle = requestAnimationFrame(updateClock);
        }

        // Clear the TimeOut after the component is unmounted.
        return () => {
            cancelAnimationFrame(handle);
        }
    });


</script>

<BackgroundElement urlBackground={BackgroundImage}/>
<section
        class="bg-black border-4 border-(--border-color) p-4 max-w-200 flex flex-col gap-4 h-fit m-auto lg:top-[50%] lg:left-[50%] lg:translate-x-[-50%] lg:translate-y-[-50%] lg:fixed">
    <h1 class="section-title">Thefirey33</h1>
    <div class="justify-self-center text-white m-auto md:text-xl flex 2xl:flex-row flex-col transition-all items-center gap-4">
        <img
                alt="Firey Plushie"
                class="2xl:w-30 2xl:h-30 w-20 h-20 transition-all"
                src={BigIconImage}
        />
        <div class="flex flex-col gap-4 flex-wrap">
            <p>
                Hello! I'm a full-stack developer, artist and game designer from Izmir,
                Türkiye!
            </p>

            <p>
                I know tons of languages pretty well and my dream is to make software that is fun and useful for
                everyone!
            </p>

            <p>
                My most important project is the
                <a class="underline" href={resolve("/tentrillion")}>TenTrillion Game Engine</a>
                , my own special engine written from
                scratch in
                Vulkan and OpenAL!
            </p>
        </div>
    </div>

    <div class="grid grid-cols-2  gap-2 flex-wrap">
        {#each SocialsData as socialData, index (index)}
            <a href={socialData.href} rel="external"
               class="text-white hover:bg-white hover:text-black transition border-b p-2 items-center gap-2 flex-wrap flex">
                <img src={`https://www.google.com/s2/favicons?domain=${socialData.href}&sz=${32}`}
                     alt="Favicon of {socialData.name}"/>
                {socialData.name}
            </a>
        {/each}
    </div>

    <div class="text-white m-auto flex flex-row items-center gap-4 text-center font-bold md:text-xl">
        <canvas bind:this={clockCanvas} height="50" width="50">
            Your browser doesn't support the HTML Canvas Rendering Method!
        </canvas>
        <p>
            Time for
            me: {`${convertToPaddedTimeString(temporalTime.hour)}:${convertToPaddedTimeString(temporalTime.minute)}:${convertToPaddedTimeString(temporalTime.second)}`}
        </p>
    </div>

</section>