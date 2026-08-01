<script lang="ts">
    let {openState = $bindable(), _class = ""} = $props();
    import {resolve} from "$app/paths";
    import Logo from "$lib/assets/favicon.webp"
</script>

<div class="flex flex-wrap items-center gap-2 {_class}">
    <button aria-label="Open Menu" class="cursor-pointer" onclick={() => openState = !openState}>
        <svg class="2xl:hidden" fill="#e3e3e3" height="32px" viewBox="0 -960 960 960"
             width="32px" xmlns="http://www.w3.org/2000/svg">
            <path d="M120-240v-80h720v80H120Zm0-200v-80h720v80H120Zm0-200v-80h720v80H120Z"></path>
        </svg>
    </button>
    <!-- Back to root section -->
    <a class="flex flex-row gap-4 p-4 group text-white text-2xl cursor-pointer hover:bg-white hover:text-black transition-all"
       href={resolve("/")}>
        <img alt="Logo" class="w-8 h-8 group-hover:ring-2 ring-black transition" height="32" src={Logo} width="32"/>

        Thefirey33
    </a>
</div>