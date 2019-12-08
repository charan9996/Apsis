import { TestBed, inject } from '@angular/core/testing';

import { AppToastService } from './app-toast.service';

describe('AppToastService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [AppToastService]
    });
  });

  it('should be created', inject([AppToastService], (service: AppToastService) => {
    expect(service).toBeTruthy();
  }));
});
