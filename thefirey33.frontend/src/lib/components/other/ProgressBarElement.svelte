<script lang="ts">
    /**
     * ATTENTION!
     * This code was forked from the NikoDex v2 FrontEnd Transition.svelte file.
     * Thanks to sudoker0 for writing the original code, thanksies fren :3
     * Seriously, this code isn't owned by me.
     * See: https://github.com/Niko-Dex/nikodex2-frontend/blob/main/src/lib/components/Transition.svelte
     */

    import {afterNavigate, beforeNavigate} from "$app/navigation";

    // The appearance of the progressbar.
    let progress = $state(0);
    let opacity = $state(1);

    // Delta-timing functions.
    let lastTimestamp = $state(0);
    let nowTimestamp = $state(0);

    // The definitions for the progressbar's start and final progress displays.
    let initProgress = 0.1;
    let maxProgress = 0.98;

    // Progress bar update variables.
    let updateInterval = 0.5;
    let passedInterval = 0;

    let delta = $derived.by(() => nowTimestamp - lastTimestamp);

    let animationFrameHandle = 0;
    function timerRun(now: number) {
        if (passedInterval > updateInterval) {
            progress = Math.max(
                Math.min(
                    progFunc(initProgress, 0.12, delta / 1000),
                    maxProgress,
                ),
                initProgress,
            );
            passedInterval = 0;
        }
        passedInterval += (now - nowTimestamp) / 1000;
        nowTimestamp = now;
        animationFrameHandle = requestAnimationFrame(timerRun);
    }

    function progFunc(y0: number, k: number, x: number) {
        return y0 + (1 - y0) * (1 - Math.exp(-k * x));
    }

    beforeNavigate(() => {
        progress = initProgress;
        opacity = 1;

        animationFrameHandle = requestAnimationFrame(timerRun);
        lastTimestamp = performance.now();
    });

    afterNavigate(() => {
        progress = 1;

        setTimeout(() => {
            opacity = 0;
        }, 100);

        cancelAnimationFrame(animationFrameHandle);
        setTimeout(() => {
            progress = 0;
        }, 200);
    });
</script>

<div
        class="h-0.75 fixed top-0 bg-(--border-color) z-100 transition-[clip-path, opacity] duration-200 ease-out loading-bar pointer-events-none"
        style="width: 100%; opacity: {opacity}; clip-path: polygon(0 0, {progress *
        100}% 0, {progress * 100}% 100%, 0% 100%);"
></div>
