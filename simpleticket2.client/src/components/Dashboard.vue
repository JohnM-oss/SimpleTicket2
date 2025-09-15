<template>
  <div>
    <v-card>
      <v-toolbar flat dense>
        <v-card-title>Simple Ticketing System Dashboard</v-card-title>
        <v-spacer></v-spacer>
        <div v-if="isLoggedIn" class="user-info mr-4">
          <v-btn color="secondary" size="x-small" class="mr-4" @click="logout">Logout</v-btn>
          <v-icon>mdi-account-circle</v-icon>
          <span class="username">{{ loggedInUsername }}</span>
        </div>
      </v-toolbar>
      <v-card-text>
        <div v-if="!isLoggedIn">
          <v-form @submit.prevent="login">
            <v-text-field v-model="loginUser.username" label="Username" required />
            <v-text-field v-model="loginUser.password" label="Password" type="password" required />
            <v-btn color="primary" type="submit">Login</v-btn>
          </v-form>
          <v-alert v-if="loginError" type="error" dense>{{ loginError }}</v-alert>
        </div>
        <div v-else>
          <users></users>
          <tickets></tickets>
        </div>
      </v-card-text>
    </v-card>
  </div>
</template>

<script lang="js">
  import { defineComponent } from 'vue';
  import Tickets from './Tickets/Tickets.vue';
  import Users from './Users/Users.vue';

  export default defineComponent({
    components: { Tickets, Users },
    data() {
      return {
        isLoggedIn: false,
        loginUser: {
          username: '',
          password: ''
        },
        loginError: '',
        loggedInUsername: '',
      };
    },
    methods: {
      async login() {
        this.loginError = '';
        const response = await fetch('/api/auth/login', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(this.loginUser)
        });
        if (response.ok) {
          this.isLoggedIn = true;
          this.loggedInUsername = this.loginUser.username;
          this.loginUser = { username: '', password: '' };
        } else {
          this.loginError = 'Invalid username or password.';
          this.loggedInUsername = '';
        }
      },
      logout() {
        this.isLoggedIn = false;
      }
    }
  });
</script>
