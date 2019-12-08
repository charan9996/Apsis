import { TestBed, inject } from '@angular/core/testing';
import { SpinnerDialogComponent } from 'src/app/ui-component/custom-ui-components/modals/spinner-new/spinner-dialog.component';

describe('UploadModalService', () => {
	beforeEach(() => {
		TestBed.configureTestingModule({
			providers: [SpinnerDialogComponent]
		});
	});

	it(
		'should be created',
		inject([SpinnerDialogComponent], (service: SpinnerDialogComponent) => {
			expect(service).toBeTruthy();
		})
	);
});
