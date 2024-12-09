import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {FileUploadComponent} from "./file-upload/file-upload.component";
import {ListComponent} from "./list/list.component";
import {DocumentDetailsComponent} from "./document-details/document-details.component";
import {PlaygroundComponent} from "./playground/playground.component";
import {APP_BASE_HREF, LocationStrategy, PathLocationStrategy} from "@angular/common";

const routes: Routes =  [
  { path: 'single-file-import', component: FileUploadComponent },
  { path: 'list', component: ListComponent },
  { path: 'document/:id', component: DocumentDetailsComponent },
  { path: 'playground', component: PlaygroundComponent},
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
  providers: [
    { provide: LocationStrategy, useClass: PathLocationStrategy },
    { provide: APP_BASE_HREF, useValue: '/' },
  ],
})
export class AppRoutingModule { }
