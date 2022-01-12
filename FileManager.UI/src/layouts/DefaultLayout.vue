<template>
  <q-layout view="lHr Lpr lFf" class="doc-page">
    <q-toolbar class="bg-primary text-white">
      <q-btn flat round dense icon="home" />
      <q-toolbar-title> FileManager </q-toolbar-title>
      <q-tabs>
        <q-route-tab label="Home" icon="home" to="/" exact />
        <q-route-tab label="Settings" icon="settings" to="/settings" exact />
      </q-tabs>
    </q-toolbar>
    <q-page-container>
      <q-page>
        <router-view />
      </q-page>
    </q-page-container>
  </q-layout>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
export default defineComponent({
  name: 'DefaultLayout',
});
</script>
