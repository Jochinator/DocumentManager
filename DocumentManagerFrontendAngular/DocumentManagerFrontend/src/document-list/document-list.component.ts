import {Component, effect, input, signal, ViewChild} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {debounceTime, distinctUntilChanged, Subject, switchMap} from "rxjs";
import {Table, TableModule} from 'primeng/table';
import {FilterService, PrimeTemplate} from 'primeng/api';
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
  filters = input<{value: string, field: string, matchMode: string} | null>(null);
  @ViewChild('documentTable') table!: Table;

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
      if (this.table){
        const filter = this.filters();
        if (!filter) {
          this.table.clear();
        }
        this.table.filter(filter!.value, filter!.field, filter!.matchMode);
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
