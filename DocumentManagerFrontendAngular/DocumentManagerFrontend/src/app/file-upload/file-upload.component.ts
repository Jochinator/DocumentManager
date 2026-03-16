import {Component, inject, signal} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {catchError, forkJoin, Observable, of} from "rxjs";
import {ActivatedRoute, Router} from "@angular/router";
import {SpinnerComponent} from '../spinner/spinner.component';

@Component({
    selector: 'app-file-upload',
    templateUrl: './file-upload.component.html',
    styleUrls: ['./file-upload.component.scss'],
    imports: [SpinnerComponent]
})
export class FileUploadComponent {
  uploadInProgress = signal(false);
  fileName = signal('');

  private http = inject(HttpClient);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  onFileSelected(event: any) {
    this.uploadInProgress.set(true);
    let uploads$: Observable<string>[] = [];
    const files: File[] = event.target.files;

    for (const file of files) {
      this.fileName.set(file.name);

      const formData = new FormData();

      formData.append("file", file);
      formData.append("lastChanged", new Date(file.lastModified).toISOString());

      formData.append("file", file);
      formData.append("lastChanged", new Date(file.lastModified).toISOString());

      if (file.webkitRelativePath) {
        formData.append("relativePath", file.webkitRelativePath);
      }

      const upload$ = this.http.post<string>("api/Document", formData).pipe(catchError(_ => {
        alert(file.name + ' konnte nicht importiert werden')
        return of('');
      }));
      uploads$.push(upload$);
    }
    if (uploads$.length === 1){
      uploads$[0].subscribe(id => {
        this.uploadInProgress.set(false);
        this.router.navigate(["document", id], {relativeTo: this.route.parent});
      })
      return
    }

    forkJoin(uploads$).subscribe(() => {
      this.uploadInProgress.set(false);
      this.router.navigate(["list"], {relativeTo: this.route.parent});
    });
  }

}
