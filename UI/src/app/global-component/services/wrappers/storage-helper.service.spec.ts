import { TestBed, inject } from '@angular/core/testing';

import { StorageHelperService } from './storage-helper.service';

describe('StorageHelperService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [StorageHelperService]
    });
  });

  it('should be created', inject([StorageHelperService], (service: StorageHelperService) => {
    expect(service).toBeTruthy();
  }));
});
