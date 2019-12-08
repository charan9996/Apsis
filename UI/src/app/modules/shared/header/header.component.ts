import { Component, OnInit } from '@angular/core';
import { AppService } from '../../../app.service';
import { UploadModalService } from 'src/app/ui-component/custom-ui-components/modals/upload-dialog/upload-modal.service';
import { ROLES_CONST } from '../../../models/Constants/ROLES_CONST';
import { UploadService } from 'src/app/global-component/services/api-mapping-services/upload.service';
import { StorageHelperService } from 'src/app/global-component/services/wrappers/storage-helper.service';
import { AuthService } from 'src/app/global-component/authentication/auth.service';
import { RequestService } from "src/app/global-component/services/api-mapping-services/request.service";

@Component({
	selector: 'app-header',
	templateUrl: './header.component.html',
	styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {
	public roles: { Evaluator: string; Learner: string; OPM: string };

	constructor(
		private authService: AuthService,
		private uploadModalService: UploadModalService,
		private uploadService: UploadService,
		public appService: AppService,
		public storageHelperService: StorageHelperService,
		public requestService: RequestService
	) { }

	public ngOnInit() {
		this.roles = ROLES_CONST;
	}

	openRequestDialog(): void {
		const data = {
			title: 'Upload Request Yorbit Input',
			note:
			'Do not select empty file and please upload course details and also add evaluator to respective courses',
			fileformat: ['xls', 'xlsx'],
			buttonName: 'Upload',
			message: 'Select a file'
		};

		this.uploadModalService.openDialog(data).subscribe(file => {
			const formData = new FormData();
			formData.append(file.name, file);

			this.uploadService.postRequestInputFile(formData).subscribe(
				data => {
					if (data) {
						const response = data as BaseResponse;
						if (response.isSuccess) {
							this.uploadModalService.recieveMessage('Successfully uploaded');
							this.uploadModalService.sendSuccess();
						} else {
							this.uploadModalService.recieveMessage(
								response.message ? response.message : 'Upload Unsuccessfull.'
							);
						}
						console.log(response);
					} else {
						this.uploadModalService.recieveMessage('Upload failed.');
					}
				},
				error => {
					this.uploadModalService.recieveMessage('Upload failed.');
				}
			);
		});
	}

	openCourseDialog(): void {
		const data = {
			title: 'Upload Course Yorbit Input',
			note: 'Do not select empty file',
			fileformat: ['xls', 'xlsx'],
			buttonName: 'Upload',
			message: 'Select a file'
		};

		this.uploadModalService.openDialog(data).subscribe(file => {
			const formData = new FormData();
			formData.append(file.name, file);

			this.uploadService.postCourseFile(formData).subscribe(
				data => {
					if (data) {
						const response = data as BaseResponse;
						if (response.message === 'Successfully Partially posted') {
							this.uploadModalService.recieveMessage(response.message);
							this.uploadModalService.sendSuccess('Successfully Partially posted');
						} else if (response.isSuccess) {
							this.uploadModalService.recieveMessage('Successfully uploaded');
							this.uploadModalService.sendSuccess();
						} else {
							this.uploadModalService.recieveMessage(
								response.message ? response.message : 'Upload Unsuccessfull.'
							);
						}
						console.log(response);
					} else {
						this.uploadModalService.recieveMessage('Upload Unsuccessfull.');
					}
				},
				error => {
					this.uploadModalService.recieveMessage('Upload failed.');
				}
			);
		});
	}

	logoutUser() {
		this.authService.logout();
	}

	//Download a zip file containing two empty templates
	downloadEmptyTemplate() {
	}
}
