import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AssignableManagementComponent } from './assignable-management.component';

describe('TagManagementComponent', () => {
  let component: AssignableManagementComponent;
  let fixture: ComponentFixture<AssignableManagementComponent>;

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
