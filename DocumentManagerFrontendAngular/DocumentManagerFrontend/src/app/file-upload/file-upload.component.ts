import { Component, OnInit } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {catchError, forkJoin, Observable, of, tap, timer} from "rxjs";
import {Router} from "@angular/router";

@Component({
  selector: 'app-file-upload',
  templateUrl: './file-upload.component.html',
  styleUrls: ['./file-upload.component.scss']
})
export class FileUploadComponent implements OnInit {
  uploadInProgress = false;
  fileName = '';

  constructor(private http: HttpClient, private router: Router) {}

  onFileSelected(event: any) {
    this.uploadInProgress = true;
    let uploads$: Observable<string>[] = [];
    const files: File[] = event.target.files;

    for (const file of files) {
      this.fileName = file.name;

      const formData = new FormData();

      formData.append("file", file);
      formData.append("lastChanged", new Date(file.lastModified).toISOString());

      const upload$ = this.http.post<string>("api/Document", formData).pipe(catchError(_ => {
        alert(file.name + ' konnte nicht importiert werden')
        return of('');
      }));
      uploads$.push(upload$);
    }
    if (uploads$.length === 1){
      uploads$[0].subscribe(id => {
        this.uploadInProgress = false;
        this.router.navigate(["/document", id]);
      })
      return
    }

    forkJoin(uploads$).subscribe(() => {
      this.uploadInProgress = false;
      this.router.navigate(["/list"]);
    });
  }

  ngOnInit(): void {
  }

}
