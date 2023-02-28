<template>
    <div :data-test="testCounter" hidden></div>
</template>

<script>
export default {
    props: {
        url: { type: String, required: true },
        query: { required: true },
        initialized: { default: false },
    },
    data: () => ({
        testCounter: 0,
    }),
    methods: {
        update() {
            const self = this;
            if (!self.query) return;

            const url = new URL(self.url, window.location.href);
            Object
                .entries(typeof self.query === 'string' ? JSON.parse(self.query) : self.query)
                .forEach(([name, value]) => url.searchParams.set(name, value));

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
                        self.$emit('page-count', 0);

                        return;
                    }

                    self.$emit('items', data.items);
                    self.$emit('max-page', data.pageCount - 1);
                    self.$emit('page-count', data.pageCount);
                });
        },
    },
    watch: {
        query(value, previous) { if (value !== previous) this.update(); },
    },
    mounted: function () { if (!this.initialized) this.update(); },
};
</script>
