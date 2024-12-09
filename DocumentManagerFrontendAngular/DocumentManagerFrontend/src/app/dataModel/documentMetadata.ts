export interface DocumentTag {
  id?: string;
  value: string;
  isManualOnly?: boolean;
}

export interface DocumentMetadata {
  id: string,
  title: string,
  date: Date,
  checked: boolean,
  filePath: string,
  senderName: string,
  contentType: string,
  tags: DocumentTag[]
}
