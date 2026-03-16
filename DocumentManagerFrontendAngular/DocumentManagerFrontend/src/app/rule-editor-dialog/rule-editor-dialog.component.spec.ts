import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RuleEditorDialogComponent } from './rule-editor-dialog.component';

describe('RuleEditorDialogComponent', () => {
  let component: RuleEditorDialogComponent;
  let fixture: ComponentFixture<RuleEditorDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RuleEditorDialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RuleEditorDialogComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
