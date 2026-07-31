<script lang="ts">
    import {enhance} from "$app/forms";
    import Thefirey33AdminLogo from "$lib/assets/img/icons/tadmin.png"

    let {form} = $props();
</script>


<form class="fixed text-white left-[50%] flex items-center flex-col gap-4 top-[50%] translate-x-[-50%] translate-y-[-50%]"
      method="POST" use:enhance>

    <p>ATTENTION! HIDE THIS FROM YOUR STREAM FEED IMMEDIATELY!</p>

    {#if !form?.success}
        <p class="bg-black border-2 text-center border-red-500 w-full p-4">Invalid Credentials or Authorization
            Code!</p>
    {/if}

    <img alt="Thefirey33 Admin Logo" class="max-w-32 max-h-32 w-full no-anti-alias" src={Thefirey33AdminLogo}/>

    <label class="gap-x-10 w-full flex flex-row items-center" for="user">
        Username:
        <input class="login-field w-full text-center" name="name" required type="text">
    </label>

    <label class="gap-x-10 w-full flex flex-row items-center" for="password">
        Password:
        <input class="login-field w-full text-center" name="password" required type="password">
    </label>

    <label class="gap-x-10 w-full flex flex-row items-center" for="password">
        Authorization Code:
        <input class="login-field w-full text-center" name="code" required type="password">
    </label>
    <em>Check your backend's logs for the authorization code!</em>
    <strong class="bg-black text-red-500 p-2">
        Any unauthorized entrances, their IP Addresses will be logged, DO NOT
        ATTEMPT!!!
    </strong>

    <button class="btn w-full" type="submit">Login</button>
</form>