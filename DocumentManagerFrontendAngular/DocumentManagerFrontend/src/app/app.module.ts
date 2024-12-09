import {NgModule} from '@angular/core';
import {BrowserModule} from '@angular/platform-browser';

import {AppRoutingModule} from './app-routing.module';
import {AppComponent} from './app.component';
import {FileUploadComponent} from './file-upload/file-upload.component';
import {HttpClientModule} from "@angular/common/http";
import {ListComponent} from './list/list.component';
import {DocumentDetailsComponent} from './document-details/document-details.component';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {SpinnerComponent} from './spinner/spinner.component';
import {TableModule} from "primeng/table";
import {InputComponent} from './input/input.component';
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {MatInputModule} from "@angular/material/input";
import {MatCheckboxModule} from "@angular/material/checkbox";
import {MatButtonModule} from "@angular/material/button";
import {MatDatepickerModule} from "@angular/material/datepicker";
import {DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE} from "@angular/material/core";
import {MatProgressSpinnerModule} from "@angular/material/progress-spinner";
import {GERMAN_DATE_FORMATS, GermanDateAdapter} from "./germanDateAdapter";
import { TagSelectionComponent } from './tag-selection/tag-selection.component';
import {MatChipsModule} from "@angular/material/chips";
import { PlaygroundComponent } from './playground/playground.component';
import {MatIconModule} from "@angular/material/icon";
import {MatAutocompleteModule} from "@angular/material/autocomplete";


@NgModule({
  declarations: [
    AppComponent,
    FileUploadComponent,
    ListComponent,
    DocumentDetailsComponent,
    SpinnerComponent,
    InputComponent,
    TagSelectionComponent,
    PlaygroundComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    MatProgressSpinnerModule,
    TableModule,
    MatInputModule,
    FormsModule,
    MatCheckboxModule,
    MatButtonModule,
    MatDatepickerModule,
    MatAutocompleteModule,
    MatChipsModule,
    MatIconModule,
    ReactiveFormsModule
  ],
  providers: [
    { provide: MAT_DATE_LOCALE, useValue: 'de-DE' },
    { provide: MAT_DATE_FORMATS, useValue: GERMAN_DATE_FORMATS},
    { provide: DateAdapter, useClass: GermanDateAdapter}
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
}
