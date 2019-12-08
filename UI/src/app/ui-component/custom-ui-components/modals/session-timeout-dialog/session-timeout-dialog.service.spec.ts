import { TestBed, inject } from '@angular/core/testing';
import { SessionTimeoutDialogComponent } from 'src/app/ui-component/custom-ui-components/modals/session-timeout-dialog/session-timeout-dialog.component';

describe('SessionTimeoutDialogService', () => {
	beforeEach(() => {
		TestBed.configureTestingModule({
			providers: [SessionTimeoutDialogComponent]
		});
	});

	it(
		'should be created',
		inject([SessionTimeoutDialogComponent], (service: SessionTimeoutDialogComponent) => {
			expect(service).toBeTruthy();
		})
	);
});
