<script lang="ts">
	import type { PageProps } from './$types';

	let { data }: PageProps = $props();

	import QuestionWallpaper from '$lib/assets/img/wallpapers/questionsWallpaper.png';
	import BackgroundElement from '$lib/components/BackgroundElement.svelte';

	import AspireLogo from '$lib/assets/img/other/websiteinfo/aspirelogo.jpg';
</script>

<BackgroundElement urlBackground={QuestionWallpaper} />

<h1 class="text-center text-3xl text-white">Questions and Answers!</h1>

<section
	class="m-auto mt-5 flex max-w-200 flex-col flex-wrap gap-4 border-2 border-(--border-color) bg-black p-4 text-white"
>
	<h2 class="text-center text-2xl">About this website?</h2>

	<div class="flex flex-col items-center gap-4 md:flex-row">
		<img class="h-30 w-30" width="200" height="200" src={AspireLogo} alt=".NET Aspire Logo" />
		<div class="m-auto flex flex-col text-center">
			<p>
				This website was brought together with .NET Aspire. A website stack middleware that I really
				enjoy using! Along with that, the specified technologies were used in the making:
			</p>
			<ul class="mt-5 list-inside list-decimal justify-center self-center text-left">
				<li>ASP.NET MVC Web API for the Backend.</li>
				<li>SvelteKit for the Frontend.</li>
				<li>PaperMC and Javalin for the Minecraft Server.</li>
				<li>
					Python with the Huggingface Transformers Library, discord.py and FastAPI Web API for the Content
					Detection and Discord Authenticator Service (CDDAS).
				</li>
				<li>PostgreSQL and Microsoft EF Core for the Database.</li>
				<li>Docker Compose for the Deployment.</li>
				<li>Cloudflare Tunneling for the Hosting.</li>
			</ul>
		</div>
	</div>
	<em class="text-center">This website is very complicated innit?</em>
</section>
