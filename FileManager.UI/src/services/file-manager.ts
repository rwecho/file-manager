import { api } from 'boot/axios';
import { ensureEndWith, ensureStartWithout } from 'src/boot/string-utilities';

export interface FolderDto {
  name: string;
  path: string;
  lastWriteTimeUtc: Date;
  lastAccessTimeUtc: Date;
  creationTimeUtc: Date;
}

export interface FileDto {
  name: string;
  path: string;
  length: number;
  lastWriteTimeUtc: Date;
  lastAccessTimeUtc: Date;
  creationTimeUtc: Date;
  extension: string;
}

export const fileManagerService = {
  createFolder: async (
    folder: string,
    subFolder: string
  ): Promise<FolderDto> => {
    const path = `${ensureEndWith(folder, '/')}${ensureStartWithout(
      subFolder,
      '/'
    )}`;
    const response = await api.post<FolderDto>(
      `FileManager/CreateFolder?path=${path}`
    );
    return response.data;
  },
  getFolder: async (path: string): Promise<FolderDto> => {
    const response = await api.get<FolderDto>(
      `FileManager/GetFolder?path=${path}`
    );
    return response.data;
  },
  getParentFolder: async (path: string): Promise<FolderDto> => {
    const response = await api.get<FolderDto>(
      `FileManager/GetParentFolder?path=${path}`
    );
    return response.data;
  },
  getFolders: async (path: string): Promise<FolderDto[]> => {
    const response = await api.get<FolderDto[]>(
      `FileManager/GetFolders?path=${path}`
    );

    return response.data;
  },
  getFiles: async (path: string): Promise<FileDto[]> => {
    const response = await api.get<FileDto[]>(
      `FileManager/GetFiles?path=${path}`
    );
    return response.data;
  },
  getFile: async (path: string): Promise<FileDto> => {
    const response = await api.get<FileDto>(`FileManager/GetFile?path=${path}`);
    return response.data;
  },

  uploadFile: async (path: string, file: File): Promise<FileDto> => {
    const formData = new FormData();
    formData.append('file', file);
    const response = await api.post<FileDto>(
      `FileManager/UploadFile?path=${path}`,
      formData,
      {
        headers: {
          'Content-Type': 'multipart/form-data',
        },
      }
    );
    return response.data;
  },
};
