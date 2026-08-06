<script lang="ts">
    import Audio from "$lib/assets/audio/yes.mp3"
    import YesYouAreCorrectIAmInFactThat from "$lib/assets/img/other/yes.png"

    let enableTheTruth = $state(false)
</script>


<button class="w-screen {enableTheTruth ? "hidden": ""} h-screen fixed left-0 top-0 cursor-pointer bg-black"
        onclick={() => enableTheTruth = true}
        title="Proceed"></button>

<img alt="YesYouAreCorrectIAmInFactThat"
     class="w-screen {!enableTheTruth ? "hidden": ""} z-100 select-none h-screen fixed left-0 top-0"
     draggable="false"
     loading="eager"
     src={YesYouAreCorrectIAmInFactThat}/>
{#if (enableTheTruth)}
    <audio autoplay loop src={Audio}></audio>
{/if}
<style>
    :root {
        background-color: black;
    }
</style>