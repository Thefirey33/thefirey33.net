<script lang="ts">
    import {resolve} from "$app/paths";
    import ManageIcon from "$lib/assets/img/admin/manage.png"
</script>

<div class="flex flex-col gap-y-5 w-fit">
    <p class="text-white text-xl">Navigate to the sections below.</p>

    <a class="btn flex flex-row items-center gap-4 group" href={resolve("/admin/arts")}>
        <img alt="Manage" class="group-hover:invert transition h-8 w-8" src={ManageIcon}/>
        Manage Arts
    </a>

    <a class="btn flex flex-row items-center gap-4 group" href={resolve("/admin/questions")}>
        <img alt="Manage" class="group-hover:invert transition h-8 w-8" src={ManageIcon}/>
        Manage Questions
    </a>
</div>