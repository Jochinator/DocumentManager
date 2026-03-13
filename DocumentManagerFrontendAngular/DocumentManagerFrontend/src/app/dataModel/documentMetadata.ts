export interface DocumentTag {
  id?: string;
  value: string;
  isManualOnly?: boolean;
}

export interface Contact {
  id?: string;
  name: string;
}

export interface DocumentMetadata {
  id: string,
  title: string,
  date: Date,
  checked: boolean,
  filePath: string,
  contact?: Contact,
  contentType: string,
  tags: DocumentTag[]
}
