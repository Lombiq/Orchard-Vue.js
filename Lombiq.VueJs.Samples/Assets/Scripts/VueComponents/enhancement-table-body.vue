<template>
    <tbody>
        <tr v-for="row in rows" :key="row.id">
            <td>{{ row.number }}</td>
            <td>{{ row.date }}</td>
            <td>{{ row.day }}</td>
            <td>{{ row.stars }}</td>
        </tr>
    </tbody>
</template>

<script>
// There is no range function in JS, but this trick gives you a 10 long array filled with undefined
// so you can use the map method's second argument for index.
const ten = Array.from({ length: 10 });

export default {
    props: {
        data: {
            type: Array,
            required: true,
        },
    },
    computed: {
        rows(self) {
            // Here we add an extra computed value to visualize the number we got from the server.
            // This isn't the most exciting enhancement, but for example it would only take a
            // little more work to turn it ont a user interface for rating what's in the row.
            return self.data.map((row, rowIndex) => ({
                ...row,
                id: 'enhancement-table-body-row-' + rowIndex,
                stars: ten
                    .map((_, index) => (index < row.random ? '★' : '✰'))
                    .join(''),
            }));
        },
    },
};

// END OF TRAINING SECTION: Progressive enhancement with SFCs
</script>
