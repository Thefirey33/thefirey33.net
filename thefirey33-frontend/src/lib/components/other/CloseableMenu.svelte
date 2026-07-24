<script lang="ts">
    let {title, children} = $props()
    import DropdownOpen from "$lib/assets/img/dropdown/dropdownopen.png"
    import DropdownClose from "$lib/assets/img/dropdown/dropdownclose.png"

    let isOpen = $state(true)
</script>

<button class="text-white flex flex-row gap-4 items-center group hover:bg-white hover:text-black text-xl cursor-pointer transition-all"
        onclick={() => isOpen = !isOpen}>
    <img alt="Unopened Dropdown Symbol." class={`${isOpen ? "hidden" : ""} group-hover:invert transition-all`}
         src={DropdownOpen}/>
    <img alt="Opened Dropdown Symbol." class={`${!isOpen ? "hidden" : ""} group-hover:invert transition-all`}
         src={DropdownClose}/>
    {title}
</button>

{#if isOpen}
    {@render children()}
{/if}