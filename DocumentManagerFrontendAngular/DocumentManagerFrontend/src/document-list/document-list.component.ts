import {Component, computed, effect, input, signal, viewChild} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {debounceTime, distinctUntilChanged, Subject, switchMap} from "rxjs";
import {Table, TableModule} from 'primeng/table';
import {FilterMetadata, FilterService, PrimeTemplate} from 'primeng/api';
import {RouterLink, RouterLinkActive} from '@angular/router';
import {DatePipe} from '@angular/common';
import {takeUntilDestroyed} from "@angular/core/rxjs-interop";
import {TagListPipe} from "../app/tag-list.pipe";
import {DocumentMetadata, DocumentTag} from "../app/dataModel/documentMetadata";

@Component({
  selector: 'app-document-list',
  templateUrl: './document-list.component.html',
  imports: [TableModule, PrimeTemplate, RouterLinkActive, RouterLink, DatePipe, TagListPipe]
})
export class DocumentListComponent {
  readonly stateKey = input<string>();
  readonly searchString = input<string>('');
  filter = input<{value: string, field: string, matchMode: string} | null>(null);
  filters = computed<{ [field: string] : FilterMetadata[] }>(() => {
    if (this.filter() == null) {return {}}
    const filter = this.filter() as {value: string, field: string, matchMode: string};
    return {[filter.field]: [{value: filter.value, matchMode: filter.matchMode}]}
  })
  table = viewChild<Table>('documentTable');

  metadatas = signal<DocumentMetadata[]>([]);
  tagMatchModes = [{label: 'Enthält', value: 'tagFilter'}];

  private searchSubject = new Subject<string>();

  constructor(private client: HttpClient, private filterService: FilterService) {
    this.filterService.register('tagFilter', (value: DocumentTag[], filter: string): boolean => {
      if (!filter) return true;
      return value.some(t => t.value.toLowerCase().includes(filter.toLowerCase()));
    });

    effect(() => this.searchSubject.next(this.searchString()));
    effect(() => {
      if (this.table() && this.filters()){
        queueMicrotask(() => this.table()!._filter());
      }
    });

    this.searchSubject.pipe(
      takeUntilDestroyed(),
      debounceTime(250),
      distinctUntilChanged(),
      switchMap(search =>
        this.client.get<DocumentMetadata[]>('api/Document',
          search.length > 0 ? {params: {search}} : undefined)
      )
    ).subscribe(value => this.metadatas.set(this.mapDates(value)));
  }

  private mapDates(documents: DocumentMetadata[]): DocumentMetadata[] {
    return documents.map(doc => ({...doc, date: new Date(doc.date)}));
  }
}
