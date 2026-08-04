<script>
    import {resolve} from "$app/paths";
    import DisclaimerIcon from "$lib/assets/img/icons/authenticity.png"
</script>

<div class="flex m-4 justify-self-center ring-2 p-4 justify-center flex-row text-red-500 md:text-xl items-center gap-x-4 gap-y-2">
    <p>Before using this section, read the Disclaimers first!</p>
    <a class="flex group animate-pulse btn text-center md:flex-row flex-col items-center gap-4"
       href={resolve("/disclaimer")}>
        <img alt="Disclaimers" class="group-hover:invert transition" src={DisclaimerIcon}/>
        Read Here
    </a>
</div>