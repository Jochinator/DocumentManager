import {Rule} from "../dataModel/rule";
import {Contact, DocumentTag} from "../dataModel/documentMetadata";
import {HttpClient} from "@angular/common/http";
import {map, Observable} from "rxjs";

export interface Assignable {
  id?: string;
  name: string;
  rule?: Rule;
}

export interface AssignableStragegy<T> {

  assignableNamePlural: string;

  getAssignables(): Observable<Assignable[]>;

  updateAssignable(updated: Assignable): Observable<Assignable>;

  deleteAssignable(id: string): Observable<Object>;

  getListFilter(x?: string): { value: string, field: string, matchMode: string } | null;

  assignableName: string;
}

export class TagStragegy implements AssignableStragegy<DocumentTag> {
  constructor(private http: HttpClient) {
  }

  assignableName = "Tag";
  assignableNamePlural = "Tags";

  getAssignables() {
    return this.http.get<DocumentTag[]>('api/Tag').pipe(map(value => value.map(tag => this.toAssignable(tag))));
  }

  updateAssignable(updated: Assignable) {
    return this.http.put<DocumentTag>(`api/Tag/${updated.id}`, this.toTag(updated)).pipe(map(value => this.toAssignable(value)));
  }

  deleteAssignable(id: string) {
    return this.http.delete(`api/Tag/${id}`)
  }

  private toAssignable(tag: DocumentTag): Assignable {
    return {...tag, name: tag.value, value: undefined} as Assignable;
  }

  private toTag(assignable: Assignable) {
    return {...assignable, value: assignable.name, name: undefined} as DocumentTag;
  }

  getListFilter(x?: string) {
    if (!x) {
      return null;
    }
    return {value: x, field: 'tags', matchMode: 'tagFilter'}
  }
}

export class ContactStrategy implements AssignableStragegy<Contact> {
  constructor(private http: HttpClient) {
  }

  getListFilter(x: string) {
    if (!x) {
      return null;
    }
    return {value: x, field: 'contact.name', matchMode: 'startsWith'}
  }

  assignableName = "Kontakt";
  assignableNamePlural = "Kontakte";

  getAssignables(): Observable<Assignable[]> {
    return this.http.get<Contact[]>('api/Contact')
  }

  updateAssignable(updated: Assignable): Observable<Assignable> {
    return this.http.put<Contact>(`api/Contact/${updated.id}`, updated)
  }

  deleteAssignable(id: string): Observable<Object> {
    return this.http.delete(`api/Contact/${id}`)
  }

}
