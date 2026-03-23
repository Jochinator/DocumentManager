import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AssignableManagementComponent } from './assignable-management.component';
import {DocumentTag} from "../dataModel/documentMetadata";

describe('TagManagementComponent', () => {
  let component: AssignableManagementComponent<DocumentTag>;
  let fixture: ComponentFixture<AssignableManagementComponent<DocumentTag>>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AssignableManagementComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AssignableManagementComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
