import { Component, OnInit } from '@angular/core';
import { requestDetails } from 'src/app/models/requestDetails';
import { RESULT_CODES } from 'src/app/models/Constants/RESULT_CODES';
import { Evaluation } from 'src/app/models/Evaluation';
import { RequestService } from 'src/app/global-component/services/api-mapping-services/request.service';
import { DownloadHelperService } from 'src/app/global-component/services/helpers/DownloadHelperService/downloadhelper.service';
import { responseStatus } from 'src/app/models/responseStatus';
import { AppToastService } from 'src/app/ui-component/custom-ui-components/toaster/app-toast.service';
import { AppService } from 'src/app/app.service';
import { ROLES_CONST } from 'src/app/models/Constants/ROLES_CONST';
import { ConfirmDialogService } from 'src/app/ui-component/custom-ui-components/modals/confirm-dialog/confirm-dialog.service';
import { StorageHelperService } from 'src/app/global-component/services/wrappers/storage-helper.service';
import { Router } from '@angular/router';
import { VerifyRoleService } from 'src/app/verify-role.service';

@Component({
	selector: 'app-requestdetails',
	styleUrls: ['./requestdetails.component.css'],
	templateUrl: './requestdetails.component.html'
})
export class RequestdetailsComponent implements OnInit {
	public requestId: string;
	public requestDetails: requestDetails;
	public resultCodes = RESULT_CODES;
	public loading: boolean;
	public evaluation: Evaluation;
	public model: responseStatus;
	public message: string;
	public response: responseStatus;
	public isLearner: boolean;
	constructor(
		private confirmDialogService: ConfirmDialogService,
		private downloadHelperService: DownloadHelperService,
		private requestService: RequestService,
		private toasterService: AppToastService,
		private appService: AppService,
		private roleService: VerifyRoleService,
		private storageHelperService: StorageHelperService,
		private router: Router
	) {
		this.evaluation = new Evaluation();
	}

	public ngOnInit() {
		this.requestId = this.storageHelperService.getItemFromLocal('requestId');
		this.loadRequestDetails(this.requestId);
	}

	public loadRequestDetails(requestId: string) {
		this.loading = true;
		this.requestService.getRequestDetails(requestId).subscribe(data => {
			if (data) {
				this.requestDetails = data as requestDetails;
				this.loading = false;
				this.isLearner =
					this.appService.userRoleId() === ROLES_CONST.Learner ||
					this.appService.userMid() === this.requestDetails.learnerMid;
			}
		});
	}

	public resultDetails(value: FormData) {
		this.updateEvaluation(this.requestId, value);
	}

	public updateEvaluation(requestId: string, evaluation: FormData) {
		this.confirmDialogService
			.confirmDialog({
				message: 'Are you sure, you want to submit provided data?',
				confirmButtonName: 'Confirm',
				cancelButtonName: 'Cancel',
				title: `Evaluate Request`
			})
			.subscribe(x => {
				if (x) {
					this.requestService.updateEvaluationResult(requestId, evaluation).subscribe(data => {
						this.response = data as responseStatus;
						if (this.response.isSuccess === true) {
							this.message = 'Successfully Updated';
							this.toasterService.showInfo(this.message);
							this.loadRequestDetails(requestId);
						} else if (this.response.isSuccess === false) {
							this.toasterService.showInfo(this.response.message);
						}
					});
				}
			});
	}

	public downloadScoreCard(FileUrl: string) {
		if (FileUrl != null && FileUrl.length > 0) {
			this.downloadHelperService.DownloadFile(String(FileUrl));
		} else {
			alert('No File Available');
		}
	}

	public downloadAssignment(FileUrl: string) {
		if (FileUrl != null && FileUrl.length > 0) {
			this.downloadHelperService.DownloadFile(String(FileUrl));
			const requestIds: string[] = [];
			requestIds.push(this.requestId);
			this.requestService.UpdateAssignmentDownloadDate(requestIds).subscribe(data => {});
		} else {
			alert('No File Available');
		}
	}

	public downloadAssignmentDisable(FileUrl: string) {
		if (FileUrl != null && FileUrl.length > 0) {
			return false;
		} else {
			return true;
		}
	}

	public downloadScoreCardDisable(FileUrl: string) {
		if (FileUrl != null && FileUrl.length > 0) {
			return false;
		} else {
			return true;
		}
	}

	public postErrorAsync(form: FormData) {
		this.confirmDialogService
			.confirmDialog({
				message: 'Are you sure you want to upload error data for this request?',
				confirmButtonName: 'Confirm',
				cancelButtonName: 'Cancel',
				title: `Evaluate Request`
			})
			.subscribe(x => {
				if (x) {
					this.message = '';
					form.append('mid', this.requestDetails.learnerMid);
					this.requestService.postErrorAsync(this.requestId, form).subscribe(
						data => {
							this.model = data as responseStatus;
							if (this.model.isSuccess === true) {
								this.message = 'Successfully Uploaded';
								this.toasterService.showInfo(this.message);
								this.loadRequestDetails(this.requestId);
							}
							if (this.model.isSuccess === false) {
								this.message = this.model.message;
								this.toasterService.showInfo(this.message);
							}
						},
						error => {
							this.message = 'Error in file Upload';
							this.toasterService.showError(this.message);
						}
					);
				}
			});
	}

	public backToList() {
		if (this.requestDetails.learnerMid === this.appService.userMid()) {
			this.router.navigate(['request/my-requests']);
		} else {
			this.router.navigate(['request']);
		}
	}
}
