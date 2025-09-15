<template>
  <v-form @submit.prevent="register">
    <v-text-field v-model="username" label="Username" required />
    <v-text-field v-model="email" label="Email" required />
    <v-text-field v-model="password" label="Password" type="password" required />
    <v-btn type="submit" color="primary">Register</v-btn>
  </v-form>
</template>

<script>
export default {
  data() {
    return { username: '', email: '', password: '' };
  },
  methods: {
    async register() {
      await fetch('user/register', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          username: this.username,
          email: this.email,
          password: this.password
        })
      });
    }
  }
};
</script>
