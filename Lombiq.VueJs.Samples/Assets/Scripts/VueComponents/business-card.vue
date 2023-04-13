<template>
    <div class="card" v-if="!loading">
        <div class="card-body" v-if="!error && currentCard">
            <h5>{{ currentCard.BusinessCard.FirstName.Text }}, {{ currentCard.BusinessCard.LastName.Text }}</h5>
            <p v-if="currentCard.BusinessCard.Phone.Text">Phone: {{ currentCard.BusinessCard.Phone.Text }}</p>
            <p v-if="currentCard.BusinessCard.Email.Text">Email: {{ currentCard.BusinessCard.Email.Text }}</p>
        </div>
        <div class="card-body" v-else-if="error">
            <h5>Something went wrong :(</h5>
            <p>{{ error }}</p>
        </div>
    </div>
    <loading-indicator v-else-if="loading" />
</template>

<script>
import LoadingIndicator from './loading-indicator.vue';

export default {
    components: {
        LoadingIndicator,
    },
    props: {
        cardId: {
            type: String,
        },
        apiUrl: {
            type: String,
            required: true,
        },
    },
    data: function () {
        return {
            currentCard: null,
            loading: false,
            error: null,
        };
    },
    watch: {
        async cardId(newId) {
            this.error = null;
            if (!newId) {
                this.currentCard = null;
                return;
            }

            this.loading = true;

            try {
                const response = await fetch(`${this.apiUrl}/${newId}`);

                if (!response.ok) {
                    const message = `An error has occured: ${response.status} - ${response.statusText}`;

                    throw new Error(message);
                }

                this.currentCard = await response.json();
            }
            catch (error) {
                this.currentCard = null;
                this.error = error;
            }
            finally {
                this.loading = false;
            }
        },
    },
};
</script>

<style scoped>
    /* The style tag is completely ignored. Please keep using themes as usual. */
</style>
