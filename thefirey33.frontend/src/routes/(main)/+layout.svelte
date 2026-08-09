<script lang="ts">
    import '../layout.css';
    import LogoElement from '$lib/components/navigationbar/LogoElement.svelte';
    import NavigationLinkElement from '$lib/components/navigationbar/NavigationLinkElement.svelte';
    // Icons.
    import TenTrillionIcon from '$lib/assets/img/icons/tentrillion.png';
    import ArtIcon from '$lib/assets/img/icons/art.png';
    import ProgrammingIcon from '$lib/assets/img/icons/programming.png';
    import LoreIcon from '$lib/assets/img/icons/lore.png';
    import QuestionsIcon from '$lib/assets/img/icons/question.png';
    import NikoDexRecoveryIcon from '$lib/assets/img/icons/nikodexbackup.png';
    import MetaTagsElement from '$lib/components/other/MetaTagsElement.svelte';
    import DropdownElement from '$lib/components/navigationbar/DropdownElement.svelte';
    import ProgressBarElement from '$lib/components/other/ProgressBarElement.svelte';
    import AuthenticityIcon from '$lib/assets/img/icons/authenticity.png';
    import ToastNotificationElement from '$lib/components/other/ToastNotificationElement.svelte';
    import {page} from '$app/state';

    let {children} = $props();

    // Is the navigation panel is open?
    let navigationPanelIsOpen = $state(false);

    let pageTitle = $derived(page.url.pathname.replaceAll('/', ''));
</script>

<ProgressBarElement/>
<MetaTagsElement
        description="the thefirey33 network!"
        image="https://thefirey33.net/front.png"
        title="thefirey33 {pageTitle.length > 0 ? `- ${pageTitle}` : ''}"
        web="https://thefirey33.net/"
/>

<nav
        class="fixed z-30 flex min-h-15 w-screen flex-row items-center justify-between gap-4 border-b-4 border-(--border-color) bg-black p-2"
>
    <LogoElement bind:openState={navigationPanelIsOpen}/>

    <div
            class="pointer-events-auto not-2xl:fixed not-2xl:h-screen not-2xl:min-w-60 not-2xl:p-4 {!navigationPanelIsOpen &&
			'not-2xl:transform-[translate(-100%)]'} top-0 left-0 flex flex-col gap-2 border-(--border-color) bg-black transition not-2xl:border-r-4 2xl:flex-row"
    >
        <LogoElement _class="2xl:hidden" bind:openState={navigationPanelIsOpen}/>

        <div class="flex flex-col gap-2 2xl:ml-auto 2xl:flex-row">
            <NavigationLinkElement href="/tentrillion" imgSrc={TenTrillionIcon}
            >TenTrillion Game Engine
            </NavigationLinkElement
            >
            <DropdownElement title="Content!">
                <NavigationLinkElement href="/arts" imgSrc={ArtIcon}>Arts</NavigationLinkElement>
                <NavigationLinkElement href="/questions" imgSrc={QuestionsIcon}
                >Questions
                </NavigationLinkElement>
            </DropdownElement>
            <NavigationLinkElement href="/coding" imgSrc={ProgrammingIcon}>Coding</NavigationLinkElement>
            <NavigationLinkElement href="/dex" imgSrc={NikoDexRecoveryIcon}>
                NikoDex Status
            </NavigationLinkElement>
            <DropdownElement title="Other...">
                <NavigationLinkElement href="/lore" imgSrc={LoreIcon}>Lore</NavigationLinkElement>
                <NavigationLinkElement href="/disclaimer" imgSrc={AuthenticityIcon}
                >Disclaimers
                </NavigationLinkElement
                >
            </DropdownElement>
        </div>
    </div>
</nav>

<!-- Actual content goes here. Maximum width is 1200px. -->
<div class="m-auto h-full max-w-400 min-w-90 p-7 pt-25">
    <ToastNotificationElement/>
    {@render children()}
</div>
