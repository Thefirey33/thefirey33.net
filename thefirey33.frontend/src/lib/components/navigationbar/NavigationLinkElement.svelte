<script lang="ts">
    import {resolve} from "$app/paths";
    import {page} from "$app/state";

    let {
        imgSrc = "",
        disabled = false,
        external = false,
        alt = "No description provided.",
        href,
        children = undefined
    } = $props()

    let isEqual = $derived(page.url.pathname === href);
</script>

<a class="btn {disabled ? "pointer-events-none opacity-50": ""} flex flex-row gap-2 group {isEqual && "bg-(--border-color) text-black"} items-center content-center text-center "
   href={external || disabled ? href: resolve(href)} rel="{external ? "external": ""}">
    {#if (imgSrc.length > 0)}
        <img alt={alt} class="group-hover:invert transition {isEqual && "invert"}" src={imgSrc}/>
    {/if}
    {@render children?.()}
</a>