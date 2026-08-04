<script lang="ts">
    import {resolve} from "$app/paths";
    import {page} from "$app/state";

    let {imgSrc = "", external = false, alt = "No description provided.", href, children = undefined} = $props()

    let isEqual = $derived(page.url.pathname === href);
</script>

<a class="btn flex flex-row gap-2 group {isEqual && "bg-(--border-color) text-black"} items-center content-center text-center "
   href={external ? href: resolve(href)} rel="{external ? "external": ""}">
    {#if (imgSrc.length > 0)}
        <img alt={alt} class="group-hover:invert transition {isEqual && "invert"}" src={imgSrc}/>
    {/if}
    {@render children?.()}
</a>