import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DocumentDetailsComponent } from './document-details.component';
import {ActivatedRoute, provideRouter} from "@angular/router";
import {provideHttpClient} from "@angular/common/http";
import {provideHttpClientTesting} from "@angular/common/http/testing";
import { of } from "rxjs";

describe('DocumentDetailsComponent', () => {
  let component: DocumentDetailsComponent;
  let fixture: ComponentFixture<DocumentDetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ DocumentDetailsComponent ],
      providers: [
        provideRouter([]),
        provideHttpClient(),
        provideHttpClientTesting(),
        {
          provide: ActivatedRoute,
          useValue: {
            params: of({ id: '123' }) // ← Mock Route Params!
          }
        }
      ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DocumentDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
