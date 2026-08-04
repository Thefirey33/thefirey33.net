<script lang="ts">
    import currentToastState, {toastMaximumTime} from "$lib/toast-helper";
    import Accepted from "$lib/assets/img/icons/accepted.png"

    let currentState = $state({
        message: "",
        show: false,
        time: 0
    });

    currentToastState.subscribe((message) => {
        currentState = message;
    });

    function getPercentage() {
        const diff = (toastMaximumTime - currentState.time) / toastMaximumTime;
        return diff * 100.0;
    }

</script>


<div class="bg-black fixed z-100 h-20 min-w-20 gap-x-3 duration-400 origin-top p-4 flex md:text-xl transition {!currentState.show ? "opacity-0 pointer-events-none" : "opacity-100"} items-center justify-center min-w-80 left-[50%] translate-x-[-50%] border-4 border-(--border-color)"
     role="alert">
    <div class="h-2 top-0 left-0 absolute bg-green-500" style="width: {getPercentage()}%;"></div>
    <p class="text-white">{currentState.message}</p>
</div>