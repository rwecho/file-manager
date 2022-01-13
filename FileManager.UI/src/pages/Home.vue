<template>
  <q-table
    class="my-sticky-header-table"
    title="Treats"
    :rows="rows"
    :columns="columns"
    row-key="name"
    flat
    bordered
  >
    <template v-slot:body-cell-icon="props">
      <q-td :props="props">
        <div>
          <q-icon v-if="props.value == 1" name="fas fa-folder" />
          <q-icon v-else-if="props.value == 0" name="fas fa-file" />
        </div>
      </q-td>
    </template>

    <template v-slot:body-cell-size="props">
      <q-td :props="props">
        <div>
          {{ readableFileSize(props.value) }}
        </div>
      </q-td>
    </template>
  </q-table>
</template>

<script lang="ts">
import { fileManagerService } from 'src/services/file-manager';
import { defineComponent, onMounted, ref } from 'vue';
import * as filesize from 'filesize';

const columns = [
  {
    name: 'icon',
    required: true,
    label: 'Type',
    align: 'left' as const,
    field: 'type',

    sortable: true,
  },
  {
    name: 'name',
    required: true,
    label: 'File name',
    align: 'left' as const,
    field: 'name',
    sortable: true,
  },
  {
    name: 'path',
    required: true,
    label: 'File path',
    align: 'left' as const,
    field: 'path',
    sortable: true,
  },
  {
    name: 'size',
    required: true,
    label: 'File size',
    align: 'left' as const,
    field: 'length',
    sortable: true,
  },
  {
    name: 'creationTime',
    label: 'Creation time',
    align: 'left' as const,
    field: 'creationTime',
    sortable: true,
    format: (val: Date) => `${new Date(val).toLocaleString()}`,
  },
  {
    name: 'lastWriteTime',
    label: 'Last write time',
    align: 'left' as const,
    field: 'lastWriteTime',
    sortable: true,
    format: (val: Date) => `${new Date(val).toLocaleString()}`,
  },
];

enum FileType {
  file,
  folder,
}

interface Row {
  name: string;
  path: string;
  length: number;
  creationTime: Date;
  lastWriteTime: Date;
  type: FileType;
}

const rows = ref<Row[]>([]);
export default defineComponent({
  methods: {
    readableFileSize: (length: number): string => {
      if (length == 0) {
        return '--';
      }
      return filesize.default(length);
    },
  },
  setup() {
    onMounted(async () => {
      console.log('Component is mounted! ');
      const folders = await fileManagerService.getFolders('/');
      const files = await fileManagerService.getFiles('/');

      folders.forEach((o) =>
        rows.value.push({
          name: o.name,
          path: o.path,
          length: 0,
          creationTime: o.creationTimeUtc,
          lastWriteTime: o.lastWriteTimeUtc,
          type: FileType.folder,
        })
      );

      files.forEach((o) =>
        rows.value.push({
          name: o.name,
          path: o.path,
          length: o.length,
          creationTime: o.creationTimeUtc,
          lastWriteTime: o.lastWriteTimeUtc,
          type: FileType.file,
        })
      );
    });

    return {
      columns,
      rows,
    };
  },
});
</script>
