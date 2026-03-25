import {Routes} from '@angular/router';
import {FileUploadComponent} from "./file-upload/file-upload.component";
import {DocumentDetailsComponent} from "./document-details/document-details.component";
import {RedirectComponent} from "./shared/redirect/redirect.component";
import {DocumentOverviewComponent} from "../document-overview/document-overview.component";


export const routes: Routes = [
  {path: '', redirectTo: 'default/list', pathMatch: 'full'},
  {
    path: 'window/:scope',
    loadComponent: () => import('./layouts/window-layout/window-layout.component')
      .then(m => m.WindowLayoutComponent),
    children: [
      {
        path: 'tags/:id',
        loadComponent: () => import('./assignable-management/assignable-detail/assignable-detail.component')
          .then(m => m.AssignableDetailComponent),
        data: {type: 'tag'}
      },
      {
        path: 'contacts/:id',
        loadComponent: () => import('./assignable-management/assignable-detail/assignable-detail.component')
          .then(m => m.AssignableDetailComponent),
        data: {type: 'contact'}
      }
    ]
  },
  {
    path: '',
    loadComponent: () => import('./layouts/main-layout/main-layout.component')
      .then(m => m.MainLayoutComponent),
    children: [
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
                  .then(m => m.AssignableDetailComponent),
                data: {type: 'tag'}
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
                  .then(m => m.AssignableDetailComponent),
                data: {type: 'contact'}
              }
            ]
          },
        ]
      }]
  }
];
