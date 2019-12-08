import { TestBed, inject } from '@angular/core/testing';

import { UploadModalService } from './upload-modal.service';

describe('UploadModalService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [UploadModalService]
    });
  });

  it('should be created', inject([UploadModalService], (service: UploadModalService) => {
    expect(service).toBeTruthy();
  }));
});
