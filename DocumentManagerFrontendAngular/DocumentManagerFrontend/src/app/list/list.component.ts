import {Component, OnInit, signal} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {DocumentMetadata} from "../dataModel/documentMetadata";
import {debounceTime, distinctUntilChanged, Subject, switchMap} from "rxjs";
import {MatFormField, MatInput, MatLabel, MatSuffix} from '@angular/material/input';
import {FormsModule} from '@angular/forms';
import {MatIconButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';
import {TableModule} from 'primeng/table';
import {PrimeTemplate} from 'primeng/api';
import {ActivatedRoute, RouterLink, RouterLinkActive} from '@angular/router';
import {DatePipe} from '@angular/common';
import {takeUntilDestroyed} from "@angular/core/rxjs-interop";

@Component({
    selector: 'app-list',
    templateUrl: './list.component.html',
    styleUrls: ['./list.component.scss'],
  imports: [MatFormField, MatLabel, MatInput, FormsModule, MatIconButton, MatSuffix, MatIcon, TableModule, PrimeTemplate, RouterLinkActive, RouterLink, DatePipe]
})
export class ListComponent implements OnInit {
  public metadatas = signal<DocumentMetadata[]>([]);
  public searchString = signal('');
  private searchSubject = new Subject<string>();

  constructor(private client: HttpClient, private route: ActivatedRoute) {
    this.route.params.pipe(
      takeUntilDestroyed(),
      switchMap(() => this.client.get<DocumentMetadata[]>('api/Document'))
    ).subscribe(value => this.metadatas.set(value));
    this.searchSubject.pipe(
      takeUntilDestroyed(),
      debounceTime(250),
      distinctUntilChanged(),
      switchMap(searchString => this.client.get<DocumentMetadata[]>('api/Document', searchString.length > 0 ? {params: {'search': searchString}}: undefined)))
      .subscribe(value => this.metadatas.set(value));
  }

  ngOnInit(): void {
    this.client.get<DocumentMetadata[]>('api/Document').subscribe(value => this.metadatas.set(value));
  }

  initiateSearch(searchString: string) {
    this.searchSubject.next(searchString);
  }
}
