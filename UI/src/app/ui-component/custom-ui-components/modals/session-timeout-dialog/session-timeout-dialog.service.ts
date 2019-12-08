import { Inject, Injectable, OnDestroy, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { Observable } from 'rxjs';
import { SessionTimeoutDialogComponent } from 'src/app/ui-component/custom-ui-components/modals/session-timeout-dialog/session-timeout-dialog.component';

@Injectable({
	providedIn: 'root'
})
export class SessionTimeoutDialogService implements OnInit, OnDestroy {
	dialogRef: MatDialogRef<SessionTimeoutDialogComponent>;
	constructor(private dialog: MatDialog) {}

	ngOnInit() {}

	ngOnDestroy() {
		this.closeDialog();
	}
	public openDialog() {
		this.dialog.closeAll();
		this.dialogRef = this.dialog.open(SessionTimeoutDialogComponent, {
			maxWidth: '100vw',
			backdropClass: 'bg-white'
		});
		this.dialogRef.updateSize('100%', '100%');
		this.dialogRef.disableClose = true;
	}

	public closeDialog() {
		this.dialogRef.close();
	}
}
