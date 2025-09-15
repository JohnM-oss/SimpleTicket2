<template>
  <div class="mt-4">
    <h2>Users</h2>
    <div class="table-actions">
      <v-btn icon="mdi-refresh" @click="fetchUsers" :loading="loading" :disabled="loading" title="Refresh"></v-btn>
    </div>
    <v-data-table :headers="headers"
                  :items="users"
                  :loading="loading"
                  class="elevation-1">
      <template #item.actions="{ item }">
        <v-btn icon="mdi-pencil" size="small" @click="openEditDialog(item)" />
        <v-btn icon="mdi-delete" size="small" color="error" @click="confirmDelete(item)" />
        <v-btn icon="mdi-shield-key" size="small" color="primary" @click="grantDeletePermission(item)" title="Grant Delete Permission" />
      </template>
    </v-data-table>

    <div style="display: flex; justify-content: flex-end; margin-top: 16px;">
      <v-btn color="primary" class="fab" icon="mdi-plus" @click="dialog = true"></v-btn>
    </div>

    <v-snackbar v-model="snackbar.show" :color="snackbar.color" :timeout="snackbar.timeout">
      {{ snackbar.text }}
    </v-snackbar>

    <!-- Add User Dialog -->
    <v-dialog v-model="dialog" max-width="500">
      <v-card>
        <v-card-title>Add User</v-card-title>
        <v-card-text>
          <v-form @submit.prevent="addUser">
            <v-text-field v-model="newUser.username" label="Username" required />
            <v-text-field v-model="newUser.email" label="Email" required />
            <v-text-field v-model="newUser.password" label="Password" type="password" required />
            <v-select v-model="newUser.role"
                      :items="roles"
                      label="Role"
                      required />
          </v-form>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn text @click="closeDialog">Cancel</v-btn>
          <v-btn color="primary" @click="addUser">Add User</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Edit User Dialog -->
    <v-dialog v-model="editDialog" max-width="500">
      <v-card>
        <v-card-title>Edit User</v-card-title>
        <v-card-text>
          <v-form @submit.prevent="updateUser">
            <v-text-field v-model="editUser.username" label="Username" required />
            <v-text-field v-model="editUser.email" label="Email" required />
            <v-select v-model="editUser.role"
                      :items="roles"
                      label="Role"
                      required />
            <!-- Password is not editable here for security -->
          </v-form>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn text @click="closeEditDialog">Cancel</v-btn>
          <v-btn color="primary" @click="updateUser">Save</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Delete Confirmation Dialog -->
    <v-dialog v-model="deleteDialog" max-width="400">
      <v-card>
        <v-card-title>Delete User</v-card-title>
        <v-card-text>
          Are you sure you want to delete this user?
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn text @click="closeDeleteDialog">Cancel</v-btn>
          <v-btn color="error" @click="deleteUser">Delete</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<script lang="js">
  import { defineComponent } from 'vue';

  export default defineComponent({
    data() {
      return {
        users: [],
        loading: false,
        dialog: false,
        editDialog: false,
        deleteDialog: false,
        userToDelete: null,
        roles: ['User', 'Admin'],
        newUser: {
          username: '',
          email: '',
          password: '',
          role: 'User',
        },
        editUser: {
          id: null,
          username: '',
          email: '',
          role: 'User',
        },
        snackbar: {
          show: false,
          text: '',
          color: '',
          timeout: 3000
        },
        headers: [
          { title: 'ID', key: 'id' },
          { title: 'Username', key: 'userName' },
          { title: 'Email', key: 'email' },
          { title: 'Roles', key: 'roles' },
          { title: 'Actions', key: 'actions', sortable: false }
        ]
      };
    },
    async created() {
      await this.fetchUsers();
    },
    methods: {
      async fetchUsers() {
        this.loading = true;
        const response = await fetch('api/auth/users');
        if (response.ok) {
          this.users = await response.json();
        }
        this.loading = false;
      },
      async addUser() {
        const response = await fetch('api/auth/register', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(this.newUser)
        });
        if (response.ok) {
          this.showSnackbar('User added successfully!', 'success');
          this.closeDialog();
          await this.fetchUsers();
        } else {
          this.showSnackbar('Failed to add user', 'error');
        }
      },
      openEditDialog(user) {
        this.editUser = {
          id: user.id,
          username: user.userName,
          email: user.email,
          role: user.roles[0],
        };
        this.editDialog = true;
      },
      async updateUser() {
        const response = await fetch(`api/auth/users/${this.editUser.id}`, {
          method: 'PUT',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({
            username: this.editUser.username,
            email: this.editUser.email,
            role: this.editUser.role,
          })
        });
        if (response.ok) {
          this.showSnackbar('User updated successfully!', 'success');
          this.closeEditDialog();
          await this.fetchUsers();
        } else {
          this.showSnackbar('Failed to update user', 'error');
        }
      },
      confirmDelete(user) {
        this.userToDelete = user;
        this.deleteDialog = true;
      },
      async deleteUser() {
        if (!this.userToDelete) return;
        const response = await fetch(`api/auth/users/${this.userToDelete.id}`, {
          method: 'DELETE'
        });
        if (response.ok) {
          this.showSnackbar('User deleted successfully!', 'success');
          await this.fetchUsers();
        } else {
          this.showSnackbar('Failed to delete user', 'error');
        }
        this.closeDeleteDialog();
      },
      async grantDeletePermission(user) {
        const response = await fetch(`api/auth/grant-delete-permission/${encodeURIComponent(user.userName)}`, {
          method: 'POST'
        });
        if (response.ok) {
          this.showSnackbar('Delete permission granted!', 'success');
        } else {
          let msg = 'Failed to grant delete permission';
          try {
            const err = await response.json();
            msg = err?.[0]?.description || err?.error || msg;
          } catch { }
          this.showSnackbar(msg, 'error');
        }
      },
      closeDialog() {
        this.dialog = false;
        this.newUser = { username: '', email: '', password: '' };
      },
      closeEditDialog() {
        this.editDialog = false;
        this.editUser = { id: null, username: '', email: '' };
      },
      closeDeleteDialog() {
        this.deleteDialog = false;
        this.userToDelete = null;
      },
      showSnackbar(text, color) {
        this.snackbar.text = text;
        this.snackbar.color = color;
        this.snackbar.show = true;
      }
    }
  });
</script>

<style scoped>
</style>
