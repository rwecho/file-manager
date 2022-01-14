<template>
  <q-table
    class="my-sticky-header-table"
    title="文件管理"
    :rows="rows"
    :columns="columns"
    row-key="name"
    flat
    bordered
    v-model:pagination.sync="pagination"
    hide-bottom
  >
    <template v-slot:top>
      <div class="text-h5 h5">文件管理</div>
      <q-space />
      <q-btn
        class="q-ml-sm"
        color="primary"
        :disable="false"
        label="新建文件夹"
        @click="createFolder"
      />
      <q-btn
        class="q-ml-sm"
        color="primary"
        :disable="false"
        label="上传文件"
        @click="uploadFileModal = true"
      />
    </template>
    <template v-slot:body-cell-icon="props">
      <q-td :props="props">
        <div v-if="props.row.fileType == 0">
          <a
            :href="`/FileManager/DownloadFile?path=${encodeURIComponent(
              props.row.path
            )}`"
          >
            <q-icon name="fas fa-file" />
          </a>
        </div>
        <div v-else-if="props.row.fileType == 1">
          <router-link
            v-bind:to="`?path=${props.row.path}`"
            tag="button"
            class="primary"
          >
            <q-icon name="fas fa-folder" />
          </router-link>
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
    <template v-slot:body-cell-name="props">
      <q-td :props="props">
        <div v-if="props.row.fileType == 0">
          <a
            :href="`/FileManager/DownloadFile?path=${encodeURIComponent(
              props.row.path
            )}`"
          >
            {{ props.value }}
          </a>
        </div>
        <div v-else-if="props.row.fileType == 1">
          <router-link
            v-bind:to="`?path=${props.row.path}`"
            tag="button"
            class="primary"
          >
            {{ props.value }}
          </router-link>
        </div>
      </q-td>
    </template>
  </q-table>

  <q-dialog v-model="uploadFileModal" persistent>
    <q-card class="full-width">
      <q-toolbar>
        <q-toolbar-title
          ><span class="text-weight-bold"
            >上传到路径 【{{ getCurrentFolder() }}】</span
          >
        </q-toolbar-title>

        <q-btn flat round dense icon="close" v-close-popup />
      </q-toolbar>

      <q-card-section class="q-pt-none">
        <q-form @submit="onSubmitUpload" class="q-gutter-md">
          <q-file
            clearable
            name="poster_file"
            v-model="fileToUpload"
            filled
            label="待上传的文件"
          />
          <q-banner class="{1:bg-primary text-white}">
            curl -X 'POST' \
            'http://localhost:5115/FileManager/UploadFile?path=%2F' \ -H
            'accept: */*' \ -H 'Content-Type: multipart/form-data' \ -F
            'file=@settings.json;type=application/json'
          </q-banner>
          <div class="row">
            <q-space />
            <q-btn
              label="上传"
              type="submit"
              color="primary"
              :disabled="fileToUpload === null"
            />
          </div>
        </q-form>
      </q-card-section>
    </q-card>
  </q-dialog>
</template>

<script lang="ts">
import { fileManagerService } from 'src/services/file-manager';
import { defineComponent, onMounted, onBeforeMount, ref } from 'vue';
import filesize from 'filesize';
import { ensureEndWithout, normalizePath } from 'boot/string-utilities';
import { useQuasar } from 'quasar';

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
  fileType: FileType;
}

const rows = ref<Row[]>([]);
export default defineComponent({
  async updated() {
    console.log(this.$route.query.path);
    await this.refresh();
  },
  methods: {
    onSubmitUpload: async function () {
      console.log('submit files');
      if (!this.fileToUpload) {
        return;
      }

      this.quasar.loading.show({
        delay: 400,
        message: '正在上传文件中... 请等待',
      });
      const currentFolder = this.getCurrentFolder();
      const fileDto = await fileManagerService.uploadFile(
        currentFolder,
        this.fileToUpload
      );
      this.fileToUpload = undefined;
      this.uploadFileModal = !this.uploadFileModal;
      rows.value.push({
        name: fileDto.name,
        path: fileDto.path,
        length: fileDto.length,
        creationTime: fileDto.creationTimeUtc,
        lastWriteTime: fileDto.lastWriteTimeUtc,
        fileType: FileType.file,
      });
      this.quasar.loading.hide();
    },
    createFolder: function () {
      console.log('create folder.');
      const currentFolder = this.getCurrentFolder();
      this.quasar
        .dialog({
          title: '新建文件夹',
          message: `请输入文件夹名称，父文件夹为 【${currentFolder}】`,
          prompt: {
            model: '',
            isValid: (val) => val.length > 0, // << here is the magic
            type: 'text', // optional
          },
          cancel: true,
          persistent: true,
        })
        .onOk((payload) => {
          console.log(payload);
          void (async () =>
            await fileManagerService
              .createFolder(currentFolder, payload)
              .then((folder) => {
                rows.value.push({
                  name: folder.name,
                  path: folder.path,
                  length: 0,
                  creationTime: folder.creationTimeUtc,
                  lastWriteTime: folder.lastWriteTimeUtc,
                  fileType: FileType.folder,
                });
              }))();
        });
    },

    readableFileSize: (length: number): string => {
      if (length == 0) {
        return '--';
      }
      return filesize(length);
    },
    getCurrentFolder: function () {
      const path = this.$route.query.path;
      const folderPath = (Array.isArray(path) ? path[0] : path)?.toString();

      return folderPath || '/';
    },

    refresh: async function () {
      await this.openFolder(this.getCurrentFolder());
    },

    openFolder: async function (folderPath: string) {
      rows.value.length = 0;
      let folders = await fileManagerService.getFolders(folderPath);
      const files = await fileManagerService.getFiles(folderPath);

      let currentFolder = await fileManagerService.getFolder(folderPath);
      let parentFolder = await fileManagerService.getParentFolder(folderPath);

      const currentPath = ensureEndWithout(
        normalizePath(currentFolder.path),
        '/'
      );
      let parentPath = currentPath.split('/').slice(0, -1).join('/');
      parentPath = parentPath || '/';
      folders = [
        {
          name: '.',
          path: `${currentPath}`,
          creationTimeUtc: currentFolder.creationTimeUtc,
          lastWriteTimeUtc: currentFolder.lastWriteTimeUtc,
          lastAccessTimeUtc: currentFolder.lastAccessTimeUtc,
        },
        {
          name: '..',
          path: `${parentPath}`,
          creationTimeUtc: parentFolder.creationTimeUtc,
          lastWriteTimeUtc: parentFolder.lastWriteTimeUtc,
          lastAccessTimeUtc: parentFolder.lastAccessTimeUtc,
        },
        ...folders,
      ];
      folders.forEach((o) =>
        rows.value.push({
          name: o.name,
          path: o.path,
          length: 0,
          creationTime: o.creationTimeUtc,
          lastWriteTime: o.lastWriteTimeUtc,
          fileType: FileType.folder,
        })
      );
      files.forEach((o) =>
        rows.value.push({
          name: o.name,
          path: o.path,
          length: o.length,
          creationTime: o.creationTimeUtc,
          lastWriteTime: o.lastWriteTimeUtc,
          fileType: FileType.file,
        })
      );
    },
  },
  async mounted() {
    await this.refresh();
    console.log('Component is mounted! ');
  },
  setup() {
    onBeforeMount(() => {
      console.log('before mounted');
      return;
    });
    onMounted(() => {
      console.log('Component is onMounted! ');
    });

    const quasar = useQuasar();
    return {
      columns,
      rows,
      pagination: {
        page: 1,
        rowsPerPage: 0, // 0 means all rows
      },
      quasar,
      uploadFileModal: ref(false),
      fileToUpload: ref<File | undefined>(undefined),
    };
  },
});
</script>
