<script>
    import Favicon from "$lib/assets/favicon.webp";

    let {title, description, web, image} = $props();
</script>

<svelte:head>

    <!-- Primary -->
    <title>{title}</title>
    <link href={Favicon} rel="icon"/>
    <meta content={title} name="title"/>
    <meta content={description} name="description"/>

    <!-- Open Graph -->
    <meta content="website" property="og:type"/>
    <meta content={web} property="og:url"/>
    <meta content={title} property="og:title"/>
    <meta content={description} property="og:description"/>
    <meta content={image} property="og:image"/>

    <!-- X (Twitter) -->
    <meta content="summary_large_image" property="twitter:card"/>
    <meta content={web} property="twitter:url"/>
    <meta content={title} property="twitter:title"/>
    <meta content={description} property="twitter:description"/>
    <meta content={web} property="twitter:image"/>

</svelte:head>