<script lang="ts">
    import {enhance} from "$app/forms";
    import BackendWarningElement from "$lib/components/other/BackendWarningElement.svelte";
    import {setToast} from "$lib/toast-helper";
    import RefreshIcon from "$lib/assets/img/admin/refresh.png";
    import {invalidateAll} from "$app/navigation";

    let {data} = $props();
</script>

<p class="mb-5 text-white">Each question asked by the users will be listed here.</p>

{#if (!data.questions.success || data.questions.message === undefined)}
    <BackendWarningElement errorMessage="Failed to fetch questions!"/>
{:else if data.questions.message.length <= 0}
    <p class="text-white">No questions yet...</p>
{:else}
    <button class="btn group mb-5 flex gap-4" onclick={async () => await invalidateAll()}>
        <img alt="Refresh" class="transition group-hover:invert" src={RefreshIcon}/>
        Refresh
    </button>
    
    <table>
        <thead>
        <tr>
            <th>Id</th>
            <th>Time</th>
            <th>Author</th>
            <th>Image</th>
            <th>Question</th>
            <th>Response</th>
            <th>Other Actions</th>
        </tr>
        </thead>
        <tbody>
        {#each data.questions.message as question (question.id)}
            <tr>
                <th>{question.id}</th>
                <th>{new Date(question.time).toUTCString()}</th>
                <th>{question.author}</th>
                <th>
                    {#if (question.attachment === null)}
                        <p>No Image!</p>
                    {:else}
                        <img class="w-20 h-20 m-auto" src={`/api/data/${question.attachment}`}
                             alt="Question Attachment"/>
                    {/if}
                </th>
                <th>{question.question}</th>
                <th>
                    <!-- Each element requires their editing. -->
                    {let baseText = $state(question.response)}
                    {let currentText = $derived(question.response)}
                    {let isDisabled = $derived(baseText === currentText)}

                    <form action="?/change" method="POST" use:enhance={() => {
                        return async ({update, result}) => {
                            // Update the state of everything in there.
                            await update({
                                invalidateAll: true
                            });

                            baseText = question.response;
                            if (result.status)
                                setToast("Updated question entry!")
                        }
                    }}>
                        <input type="hidden" name="id" value={question.id}/>
                        <input bind:value={currentText} required class="login-field" placeholder="Response here..."
                               type="text" name="response">
                        <button class="btn text-xs {isDisabled ? "opacity-50 pointer-events-none" : ""}"
                                disabled={isDisabled}>Change
                            Response
                        </button>
                    </form>
                </th>
                <th>
                    <form action="?/delete" method="POST" use:enhance>
                        <input type="hidden" name="id" value={question.id}/>
                        <button class="btn text-xs">Delete</button>
                    </form>
                </th>
            </tr>
        {/each}
        </tbody>
    </table>
{/if}