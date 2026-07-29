<!-- Kindness always doesn't lead to good results doesn't it? -->
<!-- Sometimes you have to be a dictator just for people to understand. -->
<!-- That's just how the world goes around, most people understand with "please". -->
<!-- Other people understand with action and with a judge's gavel. -->

<script lang="ts">
    import {minFontSize} from "$lib";
    import DocumentViewerElement from "$lib/components/other/DocumentViewerElement.svelte";

    let fontSize = $state(minFontSize);
</script>

<DocumentViewerElement bind:fontSize={fontSize}/>
<article class="gap-10 flex flex-col flex-wrap text-white" style="font-size: {fontSize}px;">
    <h1 class="text-center text-3xl">
        Disclaimers
    </h1>

    <p class="text-center italic text-xl">
        This section was added due to events that occurred starting from 08.07.2026.
    </p>

    <p>
        This section declares that every instance of characters I have made, their origins trace
        back to "Thefirey33". Any impersonator must mark
        that they are impersonating. Otherwise due to impersonation,
        any action will be taken against unauthorized use
        of my identity online. If any person wants to use my main profile picture,
        they must declare that they are a "fan",
        "impersonator" or anything else that matches these words of context. Otherwise, due to unauthorized
        impersonation, the specified cause of the problem will be taken action against. If any person spots another
        person using my profile picture without authorization,
        that person should report it to me immediately. If the specified cause is
        any sort of a close relative to the person, the person should encourage their close person to stop holding the
        identity. If the specified impersonation is for a joke, there is no action that will be taken against if it
        doesn't hurt the reputation of Thefirey33, or any action that
        might affect the mental health of Thefirey33. If it is any way
        hurtful towards Thefirey33, action will be taken in any way.

        <strong class="italic underline">
            In short, DO NOT use Thefirey33's identity online without proper authorization.
        </strong>
    </p>

    <p>
        Along with that, If you want to involve Thefirey33 in any sort of drama, you are not recommended to do so. Any
        sort of drama will be ignored regardless of the importance. Your personal conflicts or tendencies belong to your
        person and you are subject to solve it out on your own case. Thefirey33 will not have any involvement in the
        specified conflict that you are in and that conflict is merely locked towards you. Your personal conflicts are
        your personal matters and Thefirey33 doesn't have any involvement in it. If you attempt to get Thefirey33 to
        send a message to the person that you are in-conflict with, your request will be ignored. Instead of giving that
        request towards Thefirey33, give it to any other person that you know of.

        <strong class="italic underline">
            This section doesn't include the term called "Venting", which is totally excused.
        </strong>
    </p>

    <p class="font-bold" style="font-size: {fontSize + 8}px;">
        - Thefirey33
    </p>
</article>

<style>
    :root {
        background-color: black;
    }
</style>