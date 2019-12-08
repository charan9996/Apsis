import { Component, EventEmitter, Inject, Input, OnInit, Output, OnDestroy } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';

import { AppToastService } from '../../toaster/app-toast.service';

@Component({
	selector: 'app-spinner',
	templateUrl: './spinner-dialog.component.html',
	styleUrls: ['./spinner-dialog.component.css']
})
export class SpinnerDialogComponent implements OnInit, OnDestroy {
	constructor(public dialogRef: MatDialogRef<SpinnerDialogComponent>) {
		console.log('CREATIGN COMPONENT');
	}

	public ngOnInit() {
		//
		console.log('ngonInit COMPONENT');
	}

	public ngOnDestroy() {
		console.log('ngonDestroy COMPONENT');
		this.dialogRef.close();
	}
}
