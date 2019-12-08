import { TestBed, inject } from '@angular/core/testing';

import { FlyerService } from './flyer.service';

describe('FlyerService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [FlyerService]
    });
  });

  it('should be created', inject([FlyerService], (service: FlyerService) => {
    expect(service).toBeTruthy();
  }));
});
