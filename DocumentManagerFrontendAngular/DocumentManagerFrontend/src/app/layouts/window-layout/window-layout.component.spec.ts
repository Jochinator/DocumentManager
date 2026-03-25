import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WindowLayoutComponent } from './window-layout.component';

describe('WindowLayoutComponent', () => {
  let component: WindowLayoutComponent;
  let fixture: ComponentFixture<WindowLayoutComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WindowLayoutComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(WindowLayoutComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
