<template>
    <div class="container">
        <div class="row justify-content-center mb-3">
            <div class="col-md-6 col-sm">
                <div class="card">
                    <div class="card-body">
                        <h5 v-if="!loading && !error">Scan code</h5>
                        <qrcode-stream @decode="onDecode" @init="onInit" :track="paintOutline">
                            <loading-indicator v-if="loading" />
                        </qrcode-stream>
                    </div>
                </div>
            </div>
        </div>
        <div class="row justify-content-center mb-3" v-if="error">
            <div class="col-md-6 col-sm">
                <p class="error">{{ error }}</p>
            </div>
        </div>
        <div class="row justify-content-center mb-3" v-if="!error && !loading">
            <div class="col-md-6 col-sm">
                <business-card :card-id="cardId" :api-url="apiUrl" />
            </div>
        </div>
    </div>
</template>

<script>
import { QrcodeStream } from '../../../node_modules/vue-qrcode-reader';
import LoadingIndicator from './loading-indicator.vue';
import BusinessCard from './business-card.vue';

export default {
    components: {
        QrcodeStream,
        LoadingIndicator,
        BusinessCard,
    },
    props: {
        errorMessages: {
            type: Object,
            default: function (raw) {
                const messages = {
                    NotAllowedError: 'You need to grant camera access permission.',
                    NotFoundError: 'No camera found.',
                    NotSupportedError: 'Secure context required (HTTPS, localhost).',
                    NotReadableError: 'The camera already in use.',
                    OverconstrainedError: 'Installed cameras are not suitable.',
                    StreamApiNotSupportedError: 'The stream API is not supported in this browser.',
                    InsecureContextError: 'Camera access is only permitted in secure context. Use HTTPS or localhost rather than HTTP.',
                    Unknown: 'Camera error.',
                };

                return {
                    ...messages,
                    ...(raw || {}),
                };
            },
        },
        apiUrl: {
            type: String,
            required: true,
        },
    },
    data: function () {
        return {
            cardId: null,
            error: '',
            loading: false,
        };
    },
    methods: {
        onDecode(result) {
            try {
                const content = JSON.parse(result);

                if (content.cardId) {
                    this.cardId = content.cardId;
                }
            }
            catch {
                // Nothing to do here, just continue scanning.
            }
        },
        async onInit(promise) {
            this.loading = true;

            try {
                await promise;
            }
            catch (error) {
                this.error = this.errorMessages[error.name] || this.errorMessages.Unknown;
            }
            finally {
                this.loading = false;
            }
        },
        paintOutline(detectedCodes, ctx) {
            for (const detectedCode of detectedCodes) {
                const [firstPoint, ...otherPoints] = detectedCode.cornerPoints;

                ctx.strokeStyle = 'red';

                ctx.beginPath();
                ctx.moveTo(firstPoint.x, firstPoint.y);
                for (const { x, y } of otherPoints) {
                    ctx.lineTo(x, y);
                }

                ctx.lineTo(firstPoint.x, firstPoint.y);
                ctx.closePath();
                ctx.stroke();
            }
        },
    },
};
</script>

<style scoped>
    /* The style tag is completely ignored. Please keep using themes as usual. */
</style>