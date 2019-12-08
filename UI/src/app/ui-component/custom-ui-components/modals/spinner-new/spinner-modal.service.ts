import { Inject, Injectable, OnDestroy, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { Observable } from 'rxjs';
import { SpinnerDialogComponent } from 'src/app/ui-component/custom-ui-components/modals/spinner-new/spinner-dialog.component';

@Injectable({
	providedIn: 'root'
})
export class SpinnerService implements OnInit, OnDestroy {
	dialogRef: MatDialogRef<SpinnerDialogComponent>;
	constructor(private dialog: MatDialog) {
		console.log('CREATING');
	}

	ngOnInit() {}

	ngOnDestroy() {
		this.closeDialog();
	}
	public openDialog() {
		if (this.dialogRef) {
			this.dialogRef.close();
		}
		this.dialogRef = this.dialog.open(SpinnerDialogComponent);
		this.dialogRef.disableClose = true;
	}

	public closeDialog() {
		this.dialogRef.close();
	}
}
