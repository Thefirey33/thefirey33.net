<script lang="ts">

    import type {PageProps} from './$types';
    import QuestionWallpaper from '$lib/assets/img/wallpapers/questionsWallpaper.png';
    import BackgroundElement from '$lib/components/BackgroundElement.svelte';
    import DiscordAuth from "$lib/assets/img/icons/discordauth.png"
    import FileInputElement from "$lib/components/other/FileInputElement.svelte";
    import BackendWarningElement from "$lib/components/other/BackendWarningElement.svelte";
    import ModerationWarning from "$lib/assets/img/icons/moderationwarning.png"
    import QuestionAsker from "$lib/assets/img/icons/questionasker.png"
    import {enhance} from '$app/forms';

    let {data, form}: PageProps = $props();
    let uploading = $state(false)
</script>

<BackgroundElement urlBackground={QuestionWallpaper}/>

<h1 class="text-center text-3xl text-white">Questions and Answers!</h1>


{#if (data.link !== undefined && data.link.message !== undefined && data.link.success)}
    <!-- This is kinda on purpose to hide the gross link you get when you hover it. -->
    <div class="flex flex-col mb-10 items-center gap-4">
        <em class="text-white md:text-xl text-center">
            To post a question, you must authorize with Discord. Only your
            name and user id is collected for the question you post as you being the "Author" of said question.</em>

        <button role="link" class="btn flex gap-x-4 items-center md:flex-row flex-col group w-fit m-auto" onclick={() => {
		// To make the language server stop bitching
		if (data.link.message === undefined)
			return;

		window.location.replace(data.link.message.url)
	}}>
            <img src={DiscordAuth} class="w-8 h-8 group-hover:invert transition" alt="Discord Auth"/>
            Authorize with Discord
        </button>
    </div>
{:else}
    <em class="text-white md:text-xl mb-10 text-center">
        After you post a question,
        it will be checked for any harmful or NSFW content. If it is detected, the question will not go through.
        Each question you send might take some time to be approved.
    </em>

    {#if (data.user !== undefined && data.user.message !== undefined && data.user.success)}
        <form method="POST" enctype="multipart/form-data"
              class="flex text-white gap-4 mt-10 mb-5 flex-col max-w-200 m-auto" use:enhance={() => {
                  uploading = true;

                  return async ({ update }) => {
                      await update();
                      uploading = false;
                  };
              }}>
            {#if (!form?.success && form?.message !== undefined)}
                <p class="text-center text-xl p-4 flex justify-center gap-x-4 animate-pulse text-red-500 font-extrabold">
                    <img src={ModerationWarning} alt="Moderation Warning"/>
                    {form?.message ?? "Your question wasn't able to be posted!"}
                </p>
            {/if}
            <p class="text-xl flex justify-center m-auto flex-row gap-4 flex-wrap items-center">
                Authenticated as:
                <em class="flex flex-row gap-4 items-center">
                    <img class="w-8 h-8" src={data.user.message.avatar_url}
                         alt="Avatar of {data.user.message.username}"/>
                    {data.user.message.username}
                </em>
            </p>
            <label class="gap-4 flex items-center" for="question">
                Question:
                <input disabled={uploading} maxlength="1024" required
                       class="login-field {uploading ? "opacity-50" : ""} w-full" type="text"
                       name="question"/>
            </label>
            <FileInputElement disabled={uploading}/>
            {#if uploading}
                <p class="text-white text-center text-xl">Posting...</p>
            {:else}
                <button disabled={uploading} class="btn {uploading ? "opacity-50" : ""}">Post</button>
            {/if}
        </form>
    {:else}
        <BackendWarningElement
                errorMessage="In a weird state where user is authenticated, but discord/filteringservice API is not returning data!"/>
    {/if}
{/if}

{#if data.questions.message !== undefined && data.questions.success}
    <div class="flex flex-col gap-4 flex-wrap">
        {#each data.questions.message as question (question.id)}
            <div class="bg-black border-2 max-w-200 m-auto border-(--border-color) w-full min-h-20 p-4">
                <div class="border-b flex flex-col gap-y-5 border-white p-4">
                    <h2 class="text-white flex gap-x-3 text-xl">
                        <img src={QuestionAsker} class="[image-rendering:pixelated]" alt="Question Asker"/>
                        {question.author} says:
                        <em>"{question.question}"</em>
                    </h2>
                    {#if (question.attachment !== null)}
                        <img src={`/api/data/${question.attachment}`} alt="Question Attachment"/>
                    {/if}
                </div>
                <p class="text-white mt-5 p-2">{question.response}</p>
            </div>
        {:else}
            <p class="text-white text-center mt-5">No questions, No Answers.</p>
        {/each}
    </div>
{:else}
    <BackendWarningElement errorMessage="Failed to fetch questions!"/>
{/if}