<template>
  <div class="mt-4">
    <h2>Tickets</h2>
    <v-data-table-server :headers="headers"
                         :items="tickets"
                         :loading="loading"
                         :items-length="totalTickets"
                         :options="options"
                         @update:options="onOptionsUpdate"
                         class="elevation-1">
      <template #item.id="{ item }">
        {{ item.id }}
      </template>
      <template #item.title="{ item }">
        {{ item.title }}
      </template>
      <template #item.description="{ item }">
        {{ item.description }}
      </template>
    </v-data-table-server>

    <!-- Floating Action Button moved below the table, aligned right -->
    <div style="display: flex; justify-content: flex-end; margin-top: 16px;">
      <v-btn color="primary"
             class="fab"
             icon="mdi-plus"
             @click="dialog = true"></v-btn>
    </div>
  </div>

  <v-snackbar-queue v-model="snackbarQueue" location="top right" />

  <!-- Add Ticket Dialog -->
  <v-dialog v-model="dialog" max-width="500">
    <v-card>
      <v-card-title>Add Ticket</v-card-title>
      <v-card-text>
        <v-form @submit.prevent="addTicket">
          <v-text-field v-model="newTicket.title" label="Title" required></v-text-field>
          <v-text-field v-model="newTicket.description" label="Description" required></v-text-field>
        </v-form>
      </v-card-text>
      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn text @click="closeDialog">Cancel</v-btn>
        <v-btn color="primary" @click="addTicket">Add Ticket</v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="js">
import { defineComponent } from 'vue';
//import { ref } from 'vue';

  export default defineComponent({
    data() {
      return {
        dialog: false,
        loading: false,
        tickets: [],
        totalTickets: 0,
        options: {
          page: 1,
          itemsPerPage: 10,
          sortBy: [],
          sortDesc: [],
        },
        headers: [
          { title: 'ID', key: 'id' },
          { title: 'Title', key: 'title' },
          { title: 'Description', key: 'description' },
        ],
        newTicket: {
          title: '',
          description: ''
        },
        snackbarQueue: [],
      };
    },
    watch: {
      options: {
        handler: 'fetchData',
        deep: true,
      },
      '$route': 'fetchData',
    },
    async created() {
      await this.fetchData();
    },
    methods: {
      async fetchData() {
        this.loading = true;
        // Build query params for pagination/sorting
        const params = new URLSearchParams();
        params.append('page', this.options.page);
        params.append('pageSize', this.options.itemsPerPage);
        if (this.options.sortBy.length) {
          params.append('sortBy', this.options.sortBy[0]);
          params.append('sortDesc', this.options.sortDesc[0]);
        }
        const response = await fetch(`ticket?${params.toString()}`);
        if (response.ok) {
          const result = await response.json();
          //this.tickets = result
          this.tickets = result.items || [];
          this.totalTickets = result.totalCount || this.tickets.length;
        }
        this.loading = false;
      },
      onOptionsUpdate(newOptions) {
        this.options = { ...this.options, ...newOptions };
      },
      async addTicket() {
        const response = await fetch('ticket', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(this.newTicket)
        });
        if (response.ok) {
          this.newTicket.title = '';
          this.newTicket.description = '';
          this.closeDialog();
          this.snackbarQueue.push({
            text: 'Ticket added successfully!',
            color: 'success',
            timeout: 3000,
          });
          await this.fetchData();
        } else {
          this.snackbarQueue.push({
            text: 'Failed to add ticket',
            color: 'error',
            timeout: 3000,
          });
        }
      },
      closeDialog() {
        this.dialog = false;
        this.newTicket.title = '';
        this.newTicket.description = '';
      },
    },
  });
</script>

<style scoped>
</style>
