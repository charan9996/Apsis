import { Inject, Injectable } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { Observable } from 'rxjs';
import { DialogData } from 'src/app/ui-component/custom-ui-components/modals/upload-dialog/DialogData';
import { UploadDialogComponent } from 'src/app/ui-component/custom-ui-components/modals/upload-dialog/upload-dialog.component';

@Injectable({
	providedIn: 'root'
})
export class UploadModalService {
	private dialogRef: MatDialogRef<UploadDialogComponent>;
	constructor(private dialog: MatDialog) {}

	public openDialog(data: DialogData): Observable<File> {
		this.dialogRef = this.dialog.open(UploadDialogComponent, { data });
		this.dialogRef.disableClose = true;
		this.dialogRef.updateSize('500px', '80vh');
		this.dialogRef.updatePosition({ top: '5%' });
		return this.dialogRef.componentInstance.fileEvent.asObservable();
	}

	public recieveMessage(message: string) {
		this.dialogRef.componentInstance.value = message;
	}

	public sendSuccess(message?: string) {
		this.dialogRef.componentInstance.onSuccess(message);
	}
}
