<script lang="ts">
    import BackendWarningElement from "$lib/components/other/BackendWarningElement.svelte";
    import WarningIcon from "$lib/assets/img/icons/apifailure.gif"
    import {resolve} from "$app/paths";
    import {setToast} from "$lib/toast-helper";
    import {enhance} from "$app/forms";
    import {invalidateAll} from "$app/navigation";
    import FileInputElement from "$lib/components/other/FileInputElement.svelte";

    let {data} = $props();


    async function deleteEntry(index: number, id: number) {
        if (!data.arts.success || data.arts.message === undefined)
            return;

        const deleteRequest: {
            success: boolean
        } = await fetch(`/api/arts/${id}`, {
            method: "DELETE"
        }).then(r => r.json());

        if (deleteRequest.success) {
            setToast(`Deleted ID: ${id}.`)
            await invalidateAll()
        }
    }

</script>

<form class="text-white p-4 max-w-200 m-auto not-md:justify-self-center flex flex-col gap-4"
      enctype="multipart/form-data" method="POST" use:enhance>

    <h1 class="text-white text-3xl text-center">Upload Art...</h1>

    <label class="justify-between gap-4 flex items-center" for="Title">
        Title:
        <input class="login-field" maxlength="256" name="Title" placeholder="Title of post..." required type="text">
    </label>
    <label class="justify-between gap-4 flex items-center" for="Author">
        Author:
        <input class="login-field" name="Author" placeholder="Author name..." required type="text">
    </label>
    <label class="justify-between gap-4 flex items-center" for="Description">
        Description:
        <input class="login-field" maxlength="256" name="Description" placeholder="Description of post...." required
               type="text">
    </label>
    <label class="justify-between gap-4 flex items-center" for="Author">
        Category:
        <input class="login-field" name="Category" placeholder="Existing/new category..." required type="text">
    </label>
    <FileInputElement required={true}/>
    <button class="btn text-base" type="submit">Upload</button>
</form>

{#if (!data.arts.success)}
    <BackendWarningElement errorMessage="Failed to fetch arts"/>
{/if}

{#if (data.arts.message !== undefined && data.arts.message.length > 0)}
    <table class="not-md:hidden wrap-anywhere w-full">
        <thead>
        <tr>
            <td>Id</td>
            <td>File Uuid</td>
            <td>Category</td>
            <td>Author</td>
            <td>Title</td>
            <td>Description</td>
            <td>Image</td>
            <td>Actions</td>
        </tr>
        </thead>
        <tbody>
        {#each data.arts.message as artPiece, index (index)}
            <tr>
                <td>{artPiece.id}</td>
                <td>
                    <button onclick={() => {
                        setToast("UUID copied to clipboard.")
                        navigator.clipboard.writeText(artPiece.uuid);
                    }}>
                        {artPiece.uuid.substring(0, 5)}...
                    </button>
                </td>
                <td>{artPiece.category}</td>
                <td>{artPiece.author}</td>
                <td>{artPiece.title}</td>
                <td>{artPiece.description}</td>
                <td>
                    {const link = resolve("/api/data/[uuid]", {
                        uuid: artPiece.uuid,
                    })}
                    <a href={link}>
                        <img class="max-w-30 justify-center justify-self-center max-h-30" src={link} alt="">
                    </a>
                </td>
                <td>
                    <button class="btn" onclick={async () => {
                        if (!confirm("Delete entry?"))
                            return;

                        await deleteEntry(index, artPiece.id);
                    }}>Delete
                    </button>
                </td>
            </tr>
        {/each}
        </tbody>
    </table>
    <p class="md:hidden flex flex-row items-center gap-4 text-white">
        <img src={WarningIcon} alt="Warning Flasher"/>
        Your display is too small to view the database, you can try changing to landscape mode!
    </p>
{:else}
    <p class="text-white">No arts. 3:</p>
{/if}

