//import './assets/main.css'

import '@mdi/font/css/materialdesignicons.css'
import { createApp } from 'vue'
import VuetifyApp from './VuetifyApp.vue'
import vuetify from './plugins/vuetify'

createApp(VuetifyApp)
  .use(vuetify)
  .mount('#app')

//import { createApp } from 'vue'
//import VuetifyApp from './VuetifyApp.vue'
//import { createVuetify } from 'vuetify'
//import 'vuetify/styles'

//import * as components from 'vuetify/components'
//import * as directives from 'vuetify/directives'

//const vuetify = createVuetify({
//  components,
//  directives,
//})

//createApp(VuetifyApp)
//  .use(vuetify)
//  .mount('#app')
