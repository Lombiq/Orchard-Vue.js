<template>
    <blockquote class="blockquote text-center">
        {{ message }}
    </blockquote>
</template>

<script>
export default {
    props: {
        message: {
            type: String,
            default: 'Loading...',
        },
    },
};
</script>

<style scoped>
    /* The style tag is completely ignored. Please keep using themes as usual. */
</style>
