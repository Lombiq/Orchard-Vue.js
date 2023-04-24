<template>
    <div class="container">
        <div class="row justify-content-center mb-3">
            <div class="col-md-6 col-sm">
                <div class="card">
                    <div class="card-body">
                        <h5 v-if="!loading && !error">[[ Scan code ]]</h5>
                        <qrcode-stream @decode="onDecode" @init="onInit" :track="paintOutline">
                            <loading-indicator v-if="loading">
                                [[ Loading ... ]]
                            </loading-indicator>
                        </qrcode-stream>
                    </div>
                </div>
            </div>
        </div>
        <div class="row justify-content-center mb-3" v-if="error">
            <div class="col-md-6 col-sm message-error">
                <span v-if="error === 'NotAllowedError'">[[ You need to grant camera access permission. ]]</span>
                <span v-else-if="error === 'NotFoundError'">[[ No camera found. ]]</span>
                <span v-else-if="error === 'NotSupportedError'">[[ Secure context required (HTTPS, localhost). ]]</span>
                <span v-else-if="error === 'NotReadableError'">[[ The camera is already in use. ]]</span>
                <span v-else-if="error === 'OverconstrainedError'">[[ Installed cameras are not suitable. ]]</span>
                <span v-else-if="error === 'StreamApiNotSupportedError'">[[ The stream API is not supported in this browser. ]]</span>
                <span v-else-if="error === 'InsecureContextError'">[[ Camera access is only permitted in secure context. Use HTTPS or localhost rather than HTTP. ]]</span>
                <span v-else>[[ Camera error. ]]</span>
            </div>
        </div>
        <div class="row justify-content-center mb-3" v-if="scanError">
            <div class="col-md-6 col-sm message-error">
                <span v-if="scanError == 'InvalidCard'">[[ This QR code does not represent a business card. ]]</span>
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
            scanError: '',
        };
    },
    methods: {
        onDecode(result) {
            const content = ((text) => {
                try {
                    return JSON.parse(text);
                }
                catch {
                    return {};
                }
            })(result);

            this.cardId = content.cardId;
            this.scanError = content.cardId ? '' : 'InvalidCard';
        },
        async onInit(promise) {
            this.loading = true;

            try {
                await promise;
            }
            catch (error) {
                this.error = error.name;
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
