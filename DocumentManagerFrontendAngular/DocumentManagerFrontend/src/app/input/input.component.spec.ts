import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InputComponent } from './input.component';

describe('InputComponent', () => {
  let component: InputComponent<string>;
  let fixture: ComponentFixture<InputComponent<string>>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ InputComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(InputComponent<string>);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
