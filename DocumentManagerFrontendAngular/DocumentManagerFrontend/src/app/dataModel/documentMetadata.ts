import {Rule} from "./rule";

export interface DocumentTag {
  id?: string;
  value: string;
  rule?: Rule;
}

export interface Contact {
  id?: string;
  name: string;
  rule?: Rule;
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
