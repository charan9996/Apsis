import { Component, EventEmitter, Inject, Input, OnInit, Output, OnDestroy } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';

import { ConfirmDialogData } from './ConfirmDialogData';

@Component({
	selector: 'app-confirm',
	templateUrl: './confirm-dialog.component.html',
	styleUrls: ['./confirm-dialog.component.css']
})
export class ConfirmDialogComponent implements OnInit, OnDestroy {
	@Output() public confirmEvent = new EventEmitter<boolean>();
	public message: string;

	public clickedYes: boolean;
	public clickedNo: boolean;
	public afterYes: boolean;
	public afterNo: boolean;
	public finishedYes: boolean;
	public finishedNo: boolean;
	public finishAnimation: boolean;

	constructor(
		public dialogRef: MatDialogRef<ConfirmDialogComponent>,
		@Inject(MAT_DIALOG_DATA) public data: ConfirmDialogData
	) {}
	public clicked(button: string) {
		if (button === 'yes') {
			this.clickedYes = true;
			setTimeout(() => {
				this.afterYes = true;
			}, 500);
			setTimeout(() => {
				this.finishedYes = true;
			}, 1500);
			setTimeout(() => {
				this.finishAnimation = true;
				this.confirmEvent.emit(true);
			}, 2000);
		}
		if (button === 'no') {
			this.clickedNo = true;
			setTimeout(() => {
				this.afterNo = true;
			}, 500);
			setTimeout(() => {
				this.finishedNo = true;
			}, 1500);
			setTimeout(() => {
				this.finishAnimation = true;
				this.confirmEvent.emit(false);
			}, 2000);
		}
	}
	public ngOnInit() {
		this.clickedYes = false;
		this.afterYes = false;
		this.finishedYes = false;
		this.clickedNo = false;
		this.afterNo = false;
		this.finishedNo = false;
		this.finishAnimation = false;
	}

	public onNoClick(): void {
		this.finishAnimation = true;
	}

	public ngOnDestroy() {
		// this.dialogRef.close();
	}
}
