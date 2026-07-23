<script lang="ts">
    import {resolve} from "$app/paths";

    let {imgSrc = "", alt = "No description provided.", href, children = undefined} = $props()
</script>

<a class="btn flex flex-row gap-2 group items-center content-center text-center" href={resolve(href)}>
    {#if (imgSrc.length > 0)}
        <img alt={alt} class="group-hover:invert transition" src={imgSrc}/>
    {/if}
    {@render children?.()}
</a>