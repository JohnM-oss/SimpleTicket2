import '@mdi/font/css/materialdesignicons.css'
import { createApp } from 'vue'
import VuetifyApp from './VuetifyApp.vue'
import vuetify from './plugins/vuetify'

createApp(VuetifyApp)
  .use(vuetify)
  .mount('#app')
