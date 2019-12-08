import { Component, EventEmitter, Inject, Input, OnInit, Output, OnDestroy } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';

import { AppToastService } from '../../toaster/app-toast.service';

@Component({
	selector: 'app-session-timeout-dialog',
	templateUrl: './session-timeout-dialog.component.html',
	styleUrls: ['./session-timeout-dialog.component.css']
})
export class SessionTimeoutDialogComponent implements OnInit, OnDestroy {
	constructor(public dialogRef: MatDialogRef<SessionTimeoutDialogComponent>) {}

	public ngOnInit() {
		//
	}

	public ngOnDestroy() {
		this.dialogRef.close();
	}
}
