<!-- Kindness always doesn't lead to good results doesn't it? -->
<!-- Sometimes you have to be a dictator just for people to understand. -->
<!-- That's just how the world goes around, most people understand with "please". -->
<!-- Other people understand with action and with a judge's gavel. -->

<script lang="ts">
	import { minFontSize } from '$lib/types';
	import DocumentViewerElement from '$lib/components/other/DocumentViewerElement.svelte';

	let fontSize = $state(minFontSize);
</script>

<DocumentViewerElement bind:fontSize />
<article class="flex flex-col flex-wrap gap-10 text-white" style="font-size: {fontSize}px;">
	<h1 class="text-center text-3xl">Disclaimers</h1>

	<!-- ONESHOT content copyright -->
	<p>
		OneShot and the game franchise, belong to FutureCat and Komodo. OneShot is a game franchise,
		owned by it's respective owner, Nightmargin. All assets belonging to the game "OneShot" belong
		to Nightmargin, FutureCat and Komodo Corporation.
	</p>

	<!-- DELTARUNE and UNDERTALE content copyright -->
	<p>
		"DELTARUNE" and "UNDERTALE" is a game franchise owned by Toby Fox. All assets from the game
		"DELTARUNE" and "UNDERTALE" are owned by Toby Fox respectively.
	</p>

	<p>All of the assets that were not produced by Thefirey33, belong to their respective owners.</p>

	<!-- .NET Aspire copyright -->
	<p>.NET Aspire and the "Aspire" logo is owned by Microsoft Corporation.</p>

	<!-- NIKODEX BACKUP SERVICE usage information -->
	<p>
		The NikoDex Backup Service is an extension of the NikoDex that is for backup and recovery
		purposes only. Do not attempt to copy someone else's Nikosona as your own. Any attempt that is
		noticed by Thefirey33 from anyone with this behaviour, will be questioned. You are forbidden
		from using the NikoDex Backup section to claim any sort of Nikosona that you don't own as your
		own, That is against the TOS of the NikoDex and the TOS of the Thefirey33 Network. You are
		allowed to use the Backup Service to make your own local version of the NikoDex, for
		experimentation purposes.
		<strong class="text-white italic underline"
			>You are forbidden from doing these specified actions with the Backup Service:</strong
		>
	</p>

	<ul class="ml-5 list-disc md:ml-20">
		<li style="font-size: {fontSize + 5}px">
			Training any sort of Artificial Intelligence on this data.
		</li>
		<li>Attempting to claim any Niko as your own, that doesn't belong to you.</li>
		<li>
			Attempting to claim with the JSON data that you can change a specified Niko however you
			please.
		</li>
		<li>Attempting to change the image files "Change By" data to claim a Niko as yours.</li>
		<li>Attempting to change the JSON files "Change By" data to claim a Niko as yours.</li>
		<li>Trying to make a false NikoDex Support request with this data.</li>
		<li>The NikoDex's TOS also applies to the NikoDex Tracker & Backup Service.</li>
	</ul>

	<p>
		If you attempt to do any of these specified actions, especially the first action that is
		mentioned; you will be questioned.
	</p>

	<!-- Arts section disclaimers -->
	<p>
		You are forbidden from also copying any sort of image from the "Arts" section as your own (if
		they aren't created by you), they belong to their respective artists. The respective artist owns
		that specified content, if you attempt to get around the API protection, your request will be
		flagged as Unauthorized Request, and you will be questioned for your behaviour. Do not attempt
		to copy anyone else's work, or attempt to mitigate the watermark that is placed by the API. The
		art piece belongs to the artist, and if you're not said artist, it doesn't belong to you.
	</p>

	<!-- Impersonation disclaimers -->
	<p class="text-center text-xl italic">
		This section was added due to events that occurred starting from 08.07.2026.
	</p>

	<p>
		This section declares that every instance of characters I have made, their origins trace back to
		"Thefirey33". Any impersonator must mark that they are impersonating. Otherwise due to
		impersonation, any action will be taken against unauthorized use of my identity online. If any
		person wants to use my main profile picture, they must declare that they are a "fan",
		"impersonator" or anything else that matches these words of context. Otherwise, due to
		unauthorized impersonation, the specified cause of the problem will be taken action against. If
		any person spots another person using my profile picture without authorization, that person
		should report it to me immediately. If the specified cause is any sort of a close relative to
		the person, the person should encourage their close person to stop holding the identity. If the
		specified impersonation is for a joke, there is no action that will be taken against if it
		doesn't hurt the reputation of Thefirey33, or any action that might affect the mental health of
		Thefirey33. If it is any way hurtful towards Thefirey33, action will be taken in any way.

		<strong class="italic underline">
			In short, DO NOT use Thefirey33's identity online without proper authorization.
		</strong>
	</p>

	<p>
		Along with that, If you want to involve Thefirey33 in any sort of drama, you are not recommended
		to do so. Any sort of drama will be ignored regardless of the importance. Your personal
		conflicts or tendencies belong to your person and you are subject to solve it out on your own
		case. Thefirey33 will not have any involvement in the specified conflict that you are in and
		that conflict is merely locked towards you. Your personal conflicts are your personal matters
		and Thefirey33 doesn't have any involvement in it. If you attempt to get Thefirey33 to send a
		message to the person that you are in-conflict with, your request will be ignored. Instead of
		giving that request towards Thefirey33, give it to any other person that you know of.

		<strong class="italic underline">
			This section doesn't include the term called "Venting", which is totally excused.
		</strong>
	</p>

	<p class="font-bold" style="font-size: {fontSize + 8}px;">- Thefirey33</p>
</article>

<style>
	:root {
		background-color: black;
	}
</style>
