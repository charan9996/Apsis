import { TestBed, inject } from '@angular/core/testing';

import { VerifyRoleService } from './verify-role.service';

describe('VerifyRoleService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [VerifyRoleService]
    });
  });

  it('should be created', inject([VerifyRoleService], (service: VerifyRoleService) => {
    expect(service).toBeTruthy();
  }));
});
