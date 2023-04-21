<template>
    <blockquote class="blockquote text-center">
        {{ message || '[[ Loading... ]]' }}
    </blockquote>
</template>

<script>
export default {
    props: {
        message: {
            type: String,
            default: '',
        },
    },
};
</script>

<style scoped>
    /* The style tag is completely ignored. Please keep using themes as usual. */
</style>
