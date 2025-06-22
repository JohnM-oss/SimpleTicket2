<template>
  <v-app>
    <v-app-bar app color="primary" dark>
      <v-app-bar-title>Simple Ticketing</v-app-bar-title>
    </v-app-bar>
    <v-main>
      <dashboard></dashboard>
    </v-main>
  </v-app>
</template>

<script lang="js">
    import Dashboard from './components/Dashboard.vue'

    export default {
        name: "VuetifyApp",
        components: {
          Dashboard,
        },
        data() {
            return {
            }
        },
    };
</script>

<style scoped>
</style>
