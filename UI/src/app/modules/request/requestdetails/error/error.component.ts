import { Component, OnInit, EventEmitter, Input, Output } from '@angular/core';
import { requestDetails } from "src/app/models/requestDetails";
import { RequestService } from "src/app/global-component/services/api-mapping-services/request.service";
import { responseStatus } from "src/app/models/responseStatus";
import { RESULT_CODES } from "src/app/models/Constants/RESULT_CODES";
import { ROLES_CONST } from "src/app/models/Constants/ROLES_CONST";
import { AppService } from "src/app/app.service";

@Component({
	selector: 'app-error',
	templateUrl: './error.component.html',
	styleUrls: ['./error.component.css']
})
export class ErrorComponent implements OnInit {
	public roles: { Evaluator: string; Learner: string; OPM: string; };
	public isUpload: boolean;
	public model: responseStatus;
	public error: any = {};
	public requestID: any;
	@Input() public message = "";
	@Input() public isLearner= true;
	public fileValid = false;
	public isVisibleOnError: boolean
	public filePresent = false;
	@Input() public requestDetailsModel: requestDetails;
	@Output() public outputRequest = new EventEmitter();
	constructor(private http: RequestService, public appService: AppService) {
		this.requestID = localStorage.getItem('requestId');
	}

	public ngOnInit() {
		this.isVisibleOnError = true;
		this.roles = ROLES_CONST;
		this.error = {
			comment: '',
			file: File
		};
	}
	public  ngOnChanges() {
		if (this.requestDetailsModel) {
			this.isVisibleOnError = this.errorTabDisable(this.requestDetailsModel.errorFileUrl, this.requestDetailsModel.resultId);
		}
	}

	public UploadedFileCheck(files: File[]) {
		this.message = '';
		if (files.length === 0) {
			this.fileValid = false;
			return;
		}
		const file = files[0];

		const regexp = new RegExp('^.*\.(zip)$');
		const searchfind = regexp.test(file.name);
		if (searchfind) {
			this.fileValid = true;
			this.message = '';
		} else {
			this.message = 'Choosen file is not a zip file';
			this.fileValid = false;
		}
	}

	public SubmitError(files: File[]) {
		this.isUpload = false;
		this.message = '';
		const file = files[0];
		const comment = this.error.comment.trim();
		if (comment.length === 0) {
			this.message = "Please add comment";
			return;
		}
		this.error.file = file;
		const form: FormData = new FormData();
		form.append("comment", this.error.comment);
		form.append("file", file);
		this.outputRequest.emit(form);
	}

	public errorTabDisable(errorFileUrl: string, resultId: string) {

		if (resultId === RESULT_CODES.cleared.toLowerCase()) {
			return true;
		}

		if (resultId === RESULT_CODES.evaluate.toLowerCase()) {
			return false;
		}
		if (resultId === RESULT_CODES.notclear.toLowerCase()) {
			return true;
		}


		if (this.requestDetailsModel.comments != null) {
			this.error.comment = this.requestDetailsModel.comments;
		}
		if (this.requestDetailsModel.errorFileUrl != null) {
			this.filePresent = true;
		}
		return true;

	}
}
