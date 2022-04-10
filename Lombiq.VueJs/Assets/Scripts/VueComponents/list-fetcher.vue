<template>
    <div :data-test="testCounter" hidden></div>
</template>

<script>
export default {
    props: {
        url: { type: String, required: true },
        query: { required: true },
    },
    data: () => ({
        testCounter: 0,
    }),
    methods: {
        update() {
            const self = this;
            if (!self.query) return;

            const url = new URL(self.url, window.location.href);
            Object.entries(self.query)
                .forEach((pair) => url.searchParams.set(pair[0], pair[1]));

            fetch(url.toString(), {
                headers: {
                    Accept: 'application/json; charset=utf-8',
                    'Content-Type': 'application/json;charset=utf-8',
                },
            })
                .then((response) => response.json())
                .then((data) => {
                    self.testCounter += 1;

                    if (data.error) {
                        // The data.data only exists in local dev mode.
                        alert(data.data ? data.data : data.error);

                        self.$emit('items', []);
                        self.$emit('max-page', 0);

                        return;
                    }

                    self.$emit('items', data.items);
                    self.$emit('max-page', data.pageCount - 1);
                });
        },
    },
    watch: {
        query() { this.update(); },
    },
    mounted: function () { this.update(); },
};
</script>
