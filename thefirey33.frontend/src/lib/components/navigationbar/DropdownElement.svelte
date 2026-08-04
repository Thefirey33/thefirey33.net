<script lang="ts">

    import {onMount} from "svelte";
    import {onNavigate} from "$app/navigation";

    import DropdownOpen from "$lib/assets/img/dropdown/dropdownopen.png"
    import DropdownClose from "$lib/assets/img/dropdown/dropdownclose.png"

    let {children, title} = $props()


    // Is the dropdown menu open?
    let isDropDownOpen = $state(false)

    onMount(() => {
        onNavigate(() => {
            isDropDownOpen = false;
        })
    })
</script>

<div class="flex flex-row-reverse relative">
    <div class="bg-black absolute {isDropDownOpen ? "scale-100": "scale-0"} w-70 top-15 not-2xl:origin-top-left origin-top-right z-50 not-2xl:left-0 transition-all p-4 border-2 border-(--border-color) flex flex-col gap-2">
        {@render children()}
    </div>
    <button class="btn z-10 group w-full flex flex-row gap-2" onclick={() => isDropDownOpen = !isDropDownOpen}>
        <img alt="Not opened dropdown." class={`${isDropDownOpen ? "hidden" : ""} group-hover:invert transition`}
             src={DropdownOpen}/>
        <img alt="Not closed dropdown." class={`${!isDropDownOpen ? "hidden" : ""}  group-hover:invert transition`}
             src={DropdownClose}/>
        {title}
    </button>
</div>