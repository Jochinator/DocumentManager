import {Component, OnDestroy, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {DocumentMetadata} from "../dataModel/documentMetadata";
import {debounceTime, distinctUntilChanged, Subject, switchMap, takeUntil} from "rxjs";

@Component({
  selector: 'app-list',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss']
})
export class ListComponent implements OnInit, OnDestroy {
  public metadatas: DocumentMetadata[] = [];
  public searchString: string = '';
  private searchSubject = new Subject<string>();
  private ngUnsubscribe = new Subject<void>();

  constructor(private client: HttpClient) { }

  ngOnDestroy(): void {
        this.ngUnsubscribe.next();
        this.ngUnsubscribe.complete();
    }

  ngOnInit(): void {
    this.client.get<DocumentMetadata[]>('api/Document').subscribe(value => this.metadatas = value);
    this.searchSubject.pipe(
      takeUntil(this.ngUnsubscribe),
      debounceTime(250),
      distinctUntilChanged(),
      switchMap(searchString => this.client.get<DocumentMetadata[]>('api/Document', searchString.length > 0 ? {params: {'search': searchString}}: undefined)))
        .subscribe(value => this.metadatas = value);
  }

  initiateSearch(searchString: string) {
    this.searchSubject.next(searchString);
  }
}
