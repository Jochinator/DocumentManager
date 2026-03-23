import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AssignableDetailComponent } from './assignable-detail.component';

describe('AssignableDetailsComponent', () => {
  let component: AssignableDetailComponent;
  let fixture: ComponentFixture<AssignableDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AssignableDetailComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AssignableDetailComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
