import {Routes} from '@angular/router';
import {FileUploadComponent} from "./file-upload/file-upload.component";
import {ListComponent} from "./list/list.component";
import {DocumentDetailsComponent} from "./document-details/document-details.component";


export const routes: Routes = [
  {path: '', redirectTo: 'default/list', pathMatch: 'full'},
  {
    path: ':scope', children: [
      {path: 'single-file-import', component: FileUploadComponent},
      {path: 'list', component: ListComponent},
      {path: 'document/:id', component: DocumentDetailsComponent},
      {path: 'tags', loadComponent: () => import('./assignable-management/assignable-management.component').then(m => m.AssignableManagementComponent), data: { type: 'tag' }},
      {path: 'contacts', loadComponent: () => import('./assignable-management/assignable-management.component').then(m => m.AssignableManagementComponent), data: { type: 'contact' }},
    ]
  }
];
