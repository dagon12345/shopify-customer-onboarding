import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { CustomerFormComponent } from './customer-form.component';
import { environment } from '../../../environments/environment';

describe('CustomerFormComponent', () => {
  let httpMock: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CustomerFormComponent],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();

    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('does not submit when the form is invalid', () => {
    const fixture = TestBed.createComponent(CustomerFormComponent);
    fixture.detectChanges();

    (fixture.componentInstance as any).submit();

    httpMock.expectNone(`${environment.apiBaseUrl}/customers`);
  });

  it('submits a valid form and shows the success state', () => {
    const fixture = TestBed.createComponent(CustomerFormComponent);
    const component = fixture.componentInstance;
    fixture.detectChanges();

    component['form'].setValue({
      firstName: 'Jane',
      lastName: 'Doe',
      email: 'jane.doe@example.com',
      phone: '',
      acceptsMarketing: true,
      note: '',
    });

    component['submit']();

    const req = httpMock.expectOne(`${environment.apiBaseUrl}/customers`);
    expect(req.request.method).toBe('POST');
    req.flush({
      customerId: '11111111-1111-1111-1111-111111111111',
      shopifySyncSucceeded: true,
      shopifyCustomerId: '999',
      shopifySyncError: null,
    });

    expect(component['state']()).toBe('success');
    expect(component['result']()?.shopifyCustomerId).toBe('999');
  });
});
