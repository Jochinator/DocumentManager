import { Pipe, PipeTransform } from '@angular/core';
import {DocumentTag} from "./dataModel/documentMetadata";

@Pipe({ name: 'tagList', standalone: true })
export class TagListPipe implements PipeTransform {
  transform(tags: DocumentTag[]): string {
    return tags?.map(t => t.value).join(', ') ?? '';
  }
}
