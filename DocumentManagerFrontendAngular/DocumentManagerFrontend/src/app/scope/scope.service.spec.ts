import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { ScopeService } from './scope.service';

describe('ScopeService', () => {
  let service: ScopeService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });

    service = TestBed.inject(ScopeService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch scopes', () => {
    const mockScopes = ['default', 'arbeit', 'privat'];

    service.scopes$.subscribe(scopes => {
      expect(scopes).toHaveLength(3);
      expect(scopes).toEqual(mockScopes);
    });

    httpMock.expectOne('api/Scope').flush(mockScopes);
  });

  it('should return empty array when no scopes', () => {
    service.scopes$.subscribe(scopes => {
      expect(scopes).toHaveLength(0);
    });

    httpMock.expectOne('api/Scope').flush([]);
  });
});
