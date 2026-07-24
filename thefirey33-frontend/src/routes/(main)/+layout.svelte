<script lang="ts">
    import '../layout.css';
    import LogoElement from "$lib/components/navigationbar/LogoElement.svelte";
    import NavigationLinkElement from "$lib/components/navigationbar/NavigationLinkElement.svelte";
    // Icons.
    import TenTrillionIcon from "$lib/assets/img/icons/tentrillion.png";
    import GamepadIcon from "$lib/assets/img/icons/gamepad.png";
    import ArtIcon from "$lib/assets/img/icons/art.png";
    import ProgrammingIcon from "$lib/assets/img/icons/programming.png";
    import LoreIcon from "$lib/assets/img/icons/lore.png";
    import NikoDexRecoveryIcon from "$lib/assets/img/icons/nikodexbackup.png";
    import MetaTagsElement from "$lib/components/other/MetaTagsElement.svelte";
    import DropdownElement from "$lib/components/navigationbar/DropdownElement.svelte";

    let {children} = $props();

    // Is the navigation panel is open?
    let navigationPanelIsOpen = $state(false);

</script>

<MetaTagsElement
        description="the thefirey33 network!"
        image=""
        title="thefirey33"
        web="https://thefirey33.net/"
/>

<nav class="bg-black fixed z-30 flex flex-row items-center gap-4 justify-between w-screen min-h-15 border-(--border-color) border-b-4 p-2">
    <LogoElement bind:openState={navigationPanelIsOpen}/>

    <div class="not-2xl:min-w-60 not-2xl:h-screen not-2xl:fixed not-2xl:p-4 {!navigationPanelIsOpen && "not-2xl:transform-[translate(-100%)]"} flex gap-2 flex-col 2xl:flex-row bg-black left-0 top-0 border-(--border-color) not-2xl:border-r-4 transition">
        <LogoElement _class="2xl:hidden" bind:openState={navigationPanelIsOpen}/>

        <div class="2xl:ml-auto flex 2xl:flex-row flex-col gap-2">
            <NavigationLinkElement href="/tentrillion" imgSrc={TenTrillionIcon}>TenTrillion</NavigationLinkElement>
            <NavigationLinkElement href="/arts" imgSrc={ArtIcon}>Arts</NavigationLinkElement>
            <NavigationLinkElement href="/coding" imgSrc={ProgrammingIcon}>Coding</NavigationLinkElement>
            <NavigationLinkElement href="/lore" imgSrc={LoreIcon}>Lore</NavigationLinkElement>

            <!-- The other categories of the  -->
            <DropdownElement title="Others...">
                <NavigationLinkElement href="/games" imgSrc={GamepadIcon}>Games</NavigationLinkElement>
                <NavigationLinkElement href="/dex" imgSrc={NikoDexRecoveryIcon}>NikoDex Status</NavigationLinkElement>
            </DropdownElement>
        </div>
    </div>
</nav>

<!-- Actual content goes here. Maximum width is 1200px. -->
<div class="max-w-400 min-w-90 h-full pt-25 m-auto p-7">
    {@render children()}
</div>