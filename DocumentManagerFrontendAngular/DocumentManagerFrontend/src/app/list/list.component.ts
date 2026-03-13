import {Component, OnInit, signal} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {DocumentMetadata, DocumentTag} from "../dataModel/documentMetadata";
import {debounceTime, distinctUntilChanged, Subject, switchMap} from "rxjs";
import {MatFormField, MatInput, MatLabel, MatSuffix} from '@angular/material/input';
import {FormsModule} from '@angular/forms';
import {MatIconButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';
import {TableModule} from 'primeng/table';
import {FilterService, PrimeTemplate} from 'primeng/api';
import {ActivatedRoute, RouterLink, RouterLinkActive} from '@angular/router';
import {DatePipe} from '@angular/common';
import {takeUntilDestroyed} from "@angular/core/rxjs-interop";
import {TagListPipe} from "../tag-list.pipe";

@Component({
    selector: 'app-list',
    templateUrl: './list.component.html',
    styleUrls: ['./list.component.scss'],
  imports: [MatFormField, MatLabel, MatInput, FormsModule, MatIconButton, MatSuffix, MatIcon, TableModule, PrimeTemplate, RouterLinkActive, RouterLink, DatePipe, TagListPipe]
})
export class ListComponent {
  public metadatas = signal<DocumentMetadata[]>([]);
  public searchString = signal('');
  private searchSubject = new Subject<string>();
  tagMatchModes = [{ label: 'Enthält', value: 'tagFilter' }];

  constructor(private client: HttpClient, private route: ActivatedRoute, private filterService: FilterService) {
    this.filterService.register('tagFilter', (value: DocumentTag[], filter: string): boolean => {
      if (!filter) return true;
      return value.some(t => t.value.toLowerCase().includes(filter.toLowerCase()));
    });
    this.route.params.pipe(
      takeUntilDestroyed(),
      switchMap(() => this.client.get<DocumentMetadata[]>('api/Document'))
    ).subscribe(value => this.metadatas.set(this.mapDates(value)));
    this.searchSubject.pipe(
      takeUntilDestroyed(),
      debounceTime(250),
      distinctUntilChanged(),
      switchMap(searchString => this.client.get<DocumentMetadata[]>('api/Document', searchString.length > 0 ? {params: {'search': searchString}}: undefined)))
      .subscribe(value => this.metadatas.set(this.mapDates(value)));
  }

  initiateSearch(searchString: string) {
    this.searchSubject.next(searchString);
  }

  private mapDates(documents: DocumentMetadata[]): DocumentMetadata[] {
    return documents.map(doc => ({
      ...doc,
      date: new Date(doc.date)
    }));
  }
}
