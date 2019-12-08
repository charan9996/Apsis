import { TestBed, inject } from '@angular/core/testing';

import { DownloadHelperService } from './downloadhelper.service';

describe('DownloadService', () => {
	beforeEach(() => {
		TestBed.configureTestingModule({
			providers: [DownloadHelperService]
		});
	});

	it(
		'should be created',
		inject([DownloadHelperService], (service: DownloadHelperService) => {
			expect(service).toBeTruthy();
		})
	);
});
