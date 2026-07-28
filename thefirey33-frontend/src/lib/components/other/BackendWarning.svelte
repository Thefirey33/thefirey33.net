<script>
    let {errorMessage = "No error message received from error handler!"} = $props()

    import ApiFailureAnimation from "$lib/assets/img/icons/apifailure.gif"
</script>

<div class="bg-black/90 w-screen h-screen fixed left-0 top-0"></div>
<div class="text-white bg-black p-4 border-4 border-red-500 text-center 2xl:text-xl min-w-80 fixed left-[50%] top-[50%] translate-x-[-50%] translate-y-[-50%]">
    <img alt="Api Failure Icon"
         class="justify-center [image-rendering:pixelated] justify-self-center w-16 h-16 mb-5 m-auto"
         src={ApiFailureAnimation}/>
    <p>Failed to communicate with server!</p>
    <p class="animate-pulse">If the issue persists, Contact Thefirey33 immediately!</p>
    <p class="bg-white text-black">{errorMessage}</p>
</div>