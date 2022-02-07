<template>
    <div class="DemoRepeater__list">
        <!-- Binding an array here is not strictly useful, we just want to show a different type
             of "v-for" than in demo-sfc. -->
        <div class="DemoRepeater__listItem"
             v-for="item in data"
             :key="item"
             :class="item">
            <slot></slot>
        </div>
    </div>
</template>

<script>
// Since this doesn't have any child components, its data can be very simple.
export default {
    props: {
        data: {
            type: Array,
            required: true,
        }
    }
}

// NEXT STATION: Assets/Scripts/VueComponents/demo-repeater.vue
</script>
