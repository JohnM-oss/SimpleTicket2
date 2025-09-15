<template>
  <div class="mt-4">
    <h2>Tickets</h2>
    <v-select v-model="statusFilter"
              :items="statusSelectItems"
              label="Filter by Status"
              item-title="text"
              item-value="value"
              clearable
              style="max-width: 250px; margin-bottom: 16px;" />
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
      <template #item.status="{ item }">
        {{ item.status }}
      </template>
      <template #item.createdBy="{ item }">
        {{ item.createdBy }}
      </template>
      <template #item.actions="{ item }">
        <v-btn icon="mdi-pencil" size="small" @click="openEditDialog(item)" />
        <v-btn icon="mdi-delete" size="small" color="error" @click="confirmDelete(item)" />
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
          <v-select v-model="newTicket.status"
                    :items="statusSelectItems"
                    label="Status"
                    item-title="text"
                    item-value="value"
                    required></v-select>
        </v-form>
      </v-card-text>
      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn text @click="closeDialog">Cancel</v-btn>
        <v-btn color="primary" @click="addTicket">Add Ticket</v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>

  <!-- Edit Ticket Dialog -->
  <v-dialog v-model="editDialog" max-width="500">
    <v-card>
      <v-card-title>Edit Ticket</v-card-title>
      <v-card-text>
        <v-form @submit.prevent="updateTicket">
          <v-text-field v-model="editTicket.title" label="Title" required></v-text-field>
          <v-text-field v-model="editTicket.description" label="Description" required></v-text-field>
          <v-select v-model="editTicket.status"
                    :items="statusSelectItems"
                    label="Status"
                    item-title="text"
                    item-value="value"
                    required></v-select>
        </v-form>
      </v-card-text>
      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn text @click="closeEditDialog">Cancel</v-btn>
        <v-btn color="primary" @click="updateTicket">Save</v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>

  <v-dialog v-model="deleteDialog" max-width="400">
    <v-card>
      <v-card-title>Delete Ticket</v-card-title>
      <v-card-text>
        Are you sure you want to delete this ticket?
      </v-card-text>
      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn text @click="closeDeleteDialog">Cancel</v-btn>
        <v-btn color="error" @click="deleteTicket">Delete</v-btn>
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
        editDialog: false,
        deleteDialog: false,
        ticketToDelete: null,
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
          { title: 'Status', key: 'status' },
          { title: 'Created By', key: 'createdBy' },
          { title: 'Actions', key: 'actions', sortable: false },
        ],
        statusFilter: null,
        // Enum mapping for TicketStatus
        statusOptions: {
          0: 'Open',
          1: 'InProgress',
          2: 'Resolved',
          3: 'Closed'
        },
        statusSelectItems: [
          { text: 'Open', value: 0 },
          { text: 'InProgress', value: 1 },
          { text: 'Resolved', value: 2 },
          { text: 'Closed', value: 3 }
        ],
        newTicket: {
          title: '',
          description: '',
          status: 0,
        },
        editTicket: {
          id: null,
          title: '',
          description: '',
          status: 0,
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
      statusFilter(newVal, oldVal) {
        this.options.page = 1; // Reset to first page on filter change
        this.fetchData();
      },
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
        if (this.statusFilter !== null && this.statusFilter !== undefined && this.statusFilter !== '') {
          params.append('status', this.statusFilter);
        }
        const response = await fetch(`api/ticket?${params.toString()}`);
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
        const response = await fetch('api/ticket', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(this.newTicket)
        });
        if (response.ok) {
          this.newTicket.title = '';
          this.newTicket.description = '';
          this.newTicket.status = 0;
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
        this.newTicket.status = 0;
      },
      openEditDialog(ticket) {
        this.editTicket = { ...ticket };
        // Ensure status is a number for v-select
        if (typeof this.editTicket.status === 'string') {
          this.editTicket.status = this.statusSelectItems.find(x => x.text == this.editTicket.status).value;
          //this.editTicket.status = parseInt(this.editTicket.status, 10);
        }
        this.editDialog = true;
      },
      async updateTicket() {
        const response = await fetch(`api/ticket/${this.editTicket.id}`, {
          method: 'PUT',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(this.editTicket)
        });
        if (response.ok) {
          this.closeEditDialog();
          this.snackbarQueue.push({
            text: 'Ticket updated successfully!',
            color: 'success',
            timeout: 3000,
          });
          await this.fetchData();
        } else {
          this.snackbarQueue.push({
            text: 'Failed to update ticket',
            color: 'error',
            timeout: 3000,
          });
        }
      },
      closeEditDialog() {
        this.editDialog = false;
        this.editTicket = { id: null, title: '', description: '' };
      },
      confirmDelete(ticket) {
        this.ticketToDelete = ticket;
        this.deleteDialog = true;
      },
      closeDeleteDialog() {
        this.deleteDialog = false;
        this.ticketToDelete = null;
      },
      async deleteTicket() {
        if (!this.ticketToDelete) return;
        const response = await fetch(`api/ticket/${this.ticketToDelete.id}`, {
          method: 'DELETE'
        });
        if (response.ok) {
          this.snackbarQueue.push({
            text: 'Ticket deleted successfully!',
            color: 'success',
            timeout: 3000,
          });
          await this.fetchData();
        } else {
          this.snackbarQueue.push({
            text: 'Failed to delete ticket',
            color: 'error',
            timeout: 3000,
          });
        }
        this.closeDeleteDialog();
      }
    },
  });
</script>

<style scoped>
</style>
