import { api } from 'boot/axios';

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
};
