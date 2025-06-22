<template>
  <div>
    <v-card>
      <v-card-text>
        <h2>Simple Ticketing System Dashboard</h2>

        <h3>Open Tickets: 0</h3>

        <tickets></tickets>
      </v-card-text>
    </v-card>
  </div>
</template>

<script lang="js">
    import { defineComponent } from 'vue';
    import Tickets from './Tickets/Tickets.vue';

    export default defineComponent({
        components: {Tickets},
        props: {
        },
        data() {
            return {
            }
        },
        computed: {
        },
        methods: {
        },
    });
</script>

<style scoped>
</style>
