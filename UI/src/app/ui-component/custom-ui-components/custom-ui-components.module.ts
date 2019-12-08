import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { UploadDialogComponent } from 'src/app/ui-component/custom-ui-components/modals/upload-dialog/upload-dialog.component';
import { UploadModalService } from 'src/app/ui-component/custom-ui-components/modals/upload-dialog/upload-modal.service';
import { MatModuleModule } from 'src/app/ui-component/mat-module/mat-module.module';
import { ConfirmDialogComponent } from '../custom-ui-components/modals/confirm-dialog/confirm-dialog.component';
import { SpinnerComponent, SpinnerDialog } from './spinner/spinner.component';
import { AppToastComponent } from './toaster/app-toast.component';
import { SpinnerDialogComponent } from 'src/app/ui-component/custom-ui-components/modals/spinner-new/spinner-dialog.component';
import { SessionTimeoutDialogComponent } from 'src/app/ui-component/custom-ui-components/modals/session-timeout-dialog/session-timeout-dialog.component';
@NgModule({
	imports: [CommonModule, MatModuleModule],
	declarations: [
		SpinnerDialog,
		SpinnerComponent,
		AppToastComponent,
		UploadDialogComponent,
		ConfirmDialogComponent,
		SpinnerDialogComponent,
		SessionTimeoutDialogComponent
	],
	exports: [
		SpinnerDialog,
		SpinnerComponent,
		AppToastComponent,
		UploadDialogComponent,
		ConfirmDialogComponent,
		SpinnerDialogComponent,
		SessionTimeoutDialogComponent
	],
	entryComponents: [
		SpinnerDialog,
		UploadDialogComponent,
		ConfirmDialogComponent,
		SpinnerDialogComponent,
		SessionTimeoutDialogComponent
	]
})
export class CustomUiComponentsModule {}
