import {Routes} from '@angular/router';
import {FileUploadComponent} from "./file-upload/file-upload.component";
import {DocumentDetailsComponent} from "./document-details/document-details.component";
import {RedirectComponent} from "./shared/redirect/redirect.component";
import {DocumentOverviewComponent} from "../document-overview/document-overview.component";


export const routes: Routes = [
  {path: '', redirectTo: 'default/list', pathMatch: 'full'},
  {
    path: ':scope', children: [
      {path: 'redirect', component: RedirectComponent},
      {path: 'single-file-import', component: FileUploadComponent},
      {path: 'list', component: DocumentOverviewComponent},
      {path: 'document/:id', component: DocumentDetailsComponent},
      {
        path: 'tags',
        loadComponent: () => import('./assignable-management/assignable-management.component')
          .then(m => m.AssignableManagementComponent),
        data: {type: 'tag'},
        children: [
          {
            path: ':id',
            loadComponent: () => import('./assignable-management/assignable-detail/assignable-detail.component')
              .then(m => m.AssignableDetailComponent)
          }
        ]
      },
      {
        path: 'contacts',
        loadComponent: () => import('./assignable-management/assignable-management.component')
          .then(m => m.AssignableManagementComponent),
        data: {type: 'contact'},
        children: [
          {
            path: ':id',
            loadComponent: () => import('./assignable-management/assignable-detail/assignable-detail.component')
              .then(m => m.AssignableDetailComponent)
          }
        ]
      },
    ]
  }
];
