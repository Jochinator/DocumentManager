import {Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild} from '@angular/core';
import {COMMA, ENTER} from "@angular/cdk/keycodes";
import {MatChipInputEvent} from "@angular/material/chips";
import {MatAutocompleteSelectedEvent} from "@angular/material/autocomplete";
import {FormControl} from "@angular/forms";
import {map, Observable, startWith} from "rxjs";
import {DocumentTag} from "../dataModel/documentMetadata";
import {HttpClient} from "@angular/common/http";

@Component({
  selector: 'app-tag-selection',
  templateUrl: './tag-selection.component.html',
  styleUrls: ['./tag-selection.component.scss']
})
export class TagSelectionComponent implements OnInit {
  public separatorKeysCodes: number[] = [ENTER, COMMA];
  @Input()
  public tags: DocumentTag[] = [];
  @Output()
  public tagsChange = new EventEmitter<DocumentTag[]>();

  @ViewChild('tagInput')
  tagInput!: ElementRef<HTMLInputElement>;

  private allTags: DocumentTag[] = [];

  tagCtrl = new FormControl('');
  filteredTags$: Observable<DocumentTag[]>;

  constructor(private http: HttpClient) {
    this.filteredTags$ = this.tagCtrl.valueChanges.pipe(
      startWith(null),
      map((tag: string | null) => (tag ? this.filter(tag) : this.allTags.slice())),
    );
  }

  ngOnInit(): void {
    this.http.get<DocumentTag[]>('api/Tag').subscribe(value => this.allTags = value);
  }

  remove(tag: DocumentTag) {
    this.tags = this.tags.filter(x => x !== tag);
    this.tagsChange.emit(this.tags);
  }

  addToken($event: MatChipInputEvent) {
    const value = ($event.value || '').trim();

    const existingTag = this.allTags.find(exisitingTag => exisitingTag.value === value);
    if (existingTag){
      this.tags.push(existingTag);
    }
    else {
      this.tags.push({value});
    }

    this.tagInput.nativeElement.value = '';
    this.tagCtrl.setValue(null);
    this.tagsChange.emit(this.tags);
  }

  selected($event: MatAutocompleteSelectedEvent) {
    const existingTag = this.allTags.find(exisitingTag => exisitingTag.value === $event.option.viewValue);
    if (existingTag){
      this.tags.push(existingTag);
    }
    else {
      this.tags.push({value: $event.option.viewValue});
    }
    this.tagInput.nativeElement.value = '';
    this.tagCtrl.setValue(null);
    this.tagsChange.emit(this.tags);
  }

  private filter(value: string): DocumentTag[] {
    const filterValue = value.toLowerCase();

    return this.allTags.filter(tag => tag.value.toLowerCase().includes(filterValue));
  }
}
