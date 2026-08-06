<script lang="ts">
    let fileInput: HTMLInputElement;
    let files: FileList | undefined = $state();

    let {required = false, disabled = false} = $props()
</script>

<label class="gap-4 flex items-center" for="file">
    Image:
    <button class="btn {disabled ? "opacity-50" : ""} text-base" disabled={disabled} onclick={() => fileInput.click()}
            type="button">Select File..
        {#if (files !== undefined && files.length > 0)}
            ({files?.item(0)?.name})
        {/if}
    </button>
    <input accept="image/*" bind:files={files} bind:this={fileInput}
           class="hidden {disabled ? "opacity-50" : ""}"
           disabled={disabled}
           name="file" required={required}
           type="file">
</label>