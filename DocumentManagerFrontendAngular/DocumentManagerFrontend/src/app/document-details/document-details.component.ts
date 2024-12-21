import { Component, OnInit } from '@angular/core';
import {ActivatedRoute, Router} from "@angular/router";
import {HttpClient} from "@angular/common/http";
import { DocumentMetadata } from '../dataModel/documentMetadata';
import {DomSanitizer, SafeResourceUrl, SafeUrl} from "@angular/platform-browser";

@Component({
  selector: 'app-document-details',
  templateUrl: './document-details.component.html',
  styleUrls: ['./document-details.component.scss']
})
export class DocumentDetailsComponent implements OnInit {
  public metadata: DocumentMetadata | undefined;
  public sanitizedDocumentLink: SafeResourceUrl | undefined;

  constructor(private route: ActivatedRoute, private client: HttpClient, private sanitizer: DomSanitizer, private router: Router) { }

  ngOnInit(): void {
    this.route.params.subscribe((value: any) => this.client.get<DocumentMetadata>("api/Document/" + value.id).subscribe(value1 => {
      this.metadata = value1;
      this.sanitizedDocumentLink = this.sanitizer.bypassSecurityTrustResourceUrl(this.metadata.filePath);
    }))
  }

  save() {
    if (this.metadata)
      this.client.put("api/Document/" + this.metadata.id, this.metadata).subscribe(value => this.router.navigate(['/list']));
  }

  delete() {
    if (this.metadata)
      if (confirm('Wirklich löschen')){
        this.client.delete("api/Document/" + this.metadata.id).subscribe(value => this.router.navigate(['/list']));
      }
  }
}
