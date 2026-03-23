import {TestBed} from '@angular/core/testing';
import {ComponentFixture} from '@angular/core/testing';
import {RedirectComponent} from './redirect.component';
import {ActivatedRoute, Router} from '@angular/router';
import {vi} from 'vitest';

describe('RedirectComponent', () => {
  let fixture: ComponentFixture<RedirectComponent>;
  let router: Router;

  const createComponent = async (queryParams: Record<string, string>) => {
    await TestBed.configureTestingModule({
      imports: [RedirectComponent],
      providers: [
        {
          provide: ActivatedRoute,
          useValue: {snapshot: {queryParams}}
        },
        {
          provide: Router,
          useValue: {navigate: vi.fn()}
        }
      ]
    }).compileComponents();

    router = TestBed.inject(Router);
    fixture = TestBed.createComponent(RedirectComponent);
    fixture.detectChanges();
    await fixture.whenStable();
  };

  it('should create', async () => {
    await createComponent({to: '/default/list'});
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('should navigate to target', async () => {
    await createComponent({to: '/default/list'});
    expect(router.navigate).toHaveBeenCalledWith(['/default/list'], {queryParams: {}});
  });

  it('should forward additional params', async () => {
    await createComponent({to: '/default/list', stateKey: 'abc123'});
    expect(router.navigate).toHaveBeenCalledWith(['/default/list'], {queryParams: {stateKey: 'abc123'}});
  });
});
