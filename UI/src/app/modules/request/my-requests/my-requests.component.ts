import { Component, OnInit, HostListener } from '@angular/core';
import { RequestService } from 'src/app/global-component/services/api-mapping-services/request.service';
import { DownloadHelperService } from 'src/app/global-component/services/helpers/DownloadHelperService/downloadhelper.service';
import { RESULT_CODES } from 'src/app/models/Constants/RESULT_CODES';
import { responseStatus } from 'src/app/models/responseStatus';
import { UploadModalService } from 'src/app/ui-component/custom-ui-components/modals/upload-dialog/upload-modal.service';
import { RequestBasicView } from '../../../models/RequestBasicView';
import { Router } from '@angular/router';

RequestBasicView;
@Component({
	selector: 'app-my-requests',
	templateUrl: './my-requests.component.html',
	styleUrls: ['./my-requests.component.css']
})
export class MyRequestsComponent implements OnInit {
	public resultCodes: { error: string; evaluate: string; cleared: string; notclear: string };
	public requestList: RequestBasicView[] = [];

	constructor(
		public requestService: RequestService,
		private _download: DownloadHelperService,
		private uploadModalService: UploadModalService,
		private router: Router
	) {}

	ngOnInit() {
		this.loadRequests();
		this.getConstants();
	}
	public loadRequests() {
		this.requestService.getLearnerRequests().subscribe(data => {
			if (data) {
				this.requestList = data as RequestBasicView[];
			}
		});
	}

	public getConstants() {
		this.resultCodes = RESULT_CODES;
	}
	public openDialog(request: RequestBasicView) {
		const modalData = {
			title: 'Upload Solution',
			note: 'Do not select empty file',
			fileformat: ['zip'],
			buttonName: 'Upload',
			message: 'Select a file'
		};

		this.uploadModalService.openDialog(modalData).subscribe(file => {
			const formData: FormData = new FormData();
			formData.append('file', file);
			this.requestService.postAssignmentSolution(request.id, formData).subscribe(
				data => {
					const response = data as BaseResponse;
					if (response) {
						if (response.isSuccess) {
							this.uploadModalService.sendSuccess();
							this.refreshOnSuccess();
						} else {
							this.uploadModalService.recieveMessage(response.message);
						}
					}
				},
				error => {
					this.uploadModalService.recieveMessage('Upload Unsuccessfull');
				}
			);
		});
	}

	public refreshOnSuccess() {
		this.loadRequests();
	}

	public download(assignmentPath: string) {
		this._download.DownloadFile(assignmentPath);
	}
	public selectedRequest(requestId: string) {
		const key = 'requestId';
		localStorage.setItem(key, requestId);
		this.router.navigate(['request/details']);
	}
}
