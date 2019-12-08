import { Component, EventEmitter, Inject, Input, OnInit, Output, OnDestroy } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';

import { AppToastService } from '../../toaster/app-toast.service';
import { DialogData } from './DialogData';

@Component({
	selector: 'app-dialog',
	templateUrl: './upload-dialog.component.html',
	styleUrls: ['./upload-dialog.component.css']
})
export class UploadDialogComponent implements OnInit, OnDestroy {
	@Output() public fileEvent: EventEmitter<File> = new EventEmitter<File>();

	public defaultSuccessMessage: string;
	public fileValid = false;
	public value: string;
	public loadingProgress: string;
	public errorMessage: string;

	constructor(
		public dialogRef: MatDialogRef<UploadDialogComponent>,
		@Inject(MAT_DIALOG_DATA) public data: DialogData,
		private appToastService: AppToastService
	) {}

	public ngOnInit() {
		this.defaultSuccessMessage = '';
		this.value = '';
		this.loadingProgress = '';
	}

	public onNoClick(): void {
		this.dialogRef.close();
	}

	public ngOnDestroy() {
		this.dialogRef.close();
	}

	public uploadedFileCheck(files: File[]) {
		this.value = '';
		this.errorMessage = '';
		if (!files || files.length === 0) {
			this.fileValid = false;
			return;
		}
		if (files[0].size > 104857600) {
			this.fileValid = false;
			this.errorMessage = 'File size should be under 100MB';
			return;
		}
		const file = files[0];
		const regexp = new RegExp(`^.*.(${this.data.fileformat.reduce((t, c) => t + '|' + c)})$`);
		const searchfind = regexp.test(file.name);
		if (searchfind) {
			this.fileValid = true;
			this.errorMessage = '';
		} else {
			this.fileValid = false;
			this.errorMessage = 'Please upload valid file format';
		}
	}

	public submitFile(files: File[]) {
		this.data.message = '';
		const file: File = files[0];

		const form: FormData = new FormData();
		form.append('file', file);
		if (file.size < 10000) {
			this.defaultSuccessMessage = `Successfully Uploaded: ${file.name}(${(file.size / 1000)
				.toFixed(2)
				.toString()}KB)`;
		} else {
			this.defaultSuccessMessage = `Successfully Uploaded: ${file.name}(${(file.size / 1000000)
				.toFixed(2)
				.toString()}MB)`;
		}

		this.fileEvent.emit(file);
	}

	public onSuccess(message: string) {
		if (!message) {
			message = this.defaultSuccessMessage;
		}
		this.dialogRef.componentInstance.value = message;
		this.dialogRef.close();
		this.dialogRef.afterClosed().subscribe(x => {
			this.appToastService.showSuccess(message);
		});
	}
}
