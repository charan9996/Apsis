import { Inject, Injectable } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ConfirmDialogComponent } from 'src/app/ui-component/custom-ui-components/modals/confirm-dialog/confirm-dialog.component';
import { ConfirmDialogData } from './ConfirmDialogData';

@Injectable({
	providedIn: 'root'
})
export class ConfirmDialogService {
	private dialogRef: MatDialogRef<ConfirmDialogComponent>;
	constructor(private dialog: MatDialog) {}

	public confirmDialog(data: ConfirmDialogData): Observable<boolean> {
		this.dialogRef = this.dialog.open(ConfirmDialogComponent, { data });
		this.dialogRef.updateSize('450px', '300px');
		this.dialogRef.disableClose = true;
		return this.dialogRef.componentInstance.confirmEvent.asObservable().pipe(
			map(x => {
				this.dialogRef.close();
				return x;
			})
		);
	}
}
