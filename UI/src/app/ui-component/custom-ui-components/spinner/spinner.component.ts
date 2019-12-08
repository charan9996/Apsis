import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material';
@Component({
	selector: 'app-spinner',
	template: ''
})
export class SpinnerComponent implements OnInit, OnDestroy {
	private lastOpenedSpinner: string;
	private dialogConfig;
	constructor(private dialog: MatDialog) { }

	public ngOnInit() {
		this.openDialog();
	}

	public ngOnDestroy() {
		this.closeDialog();
	}

	public openDialog() {
		this.dialogConfig = this.dialog.open(SpinnerDialog);
		this.lastOpenedSpinner = this.dialogConfig.id;
		this.dialogConfig.disableClose = true;
	}

	public closeDialog() {

		const currentDialogId = this.dialog.getDialogById(this.lastOpenedSpinner);
		if (currentDialogId) {
			currentDialogId.close();
		}
	}
}

// tslint:disable-next-line:max-classes-per-file
@Component({
	selector: 'spinnerDialog',
	templateUrl: 'spinnerDialog.component.html',
	styleUrls: ['./spinnerDialog.component.css']
})
export class SpinnerDialog {
	constructor(public dialogRef: MatDialogRef<SpinnerDialog>) { }
}
