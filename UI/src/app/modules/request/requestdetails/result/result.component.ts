import { Component, OnInit, Input, Output, EventEmitter, OnChanges, AfterViewInit, SimpleChanges } from '@angular/core';
import { FormControl, FormGroup, Validators, FormBuilder } from '@angular/forms';
import { RESULT_CODES } from 'src/app/models/Constants/RESULT_CODES';
import { requestDetails } from 'src/app/models/requestDetails';
import { AppService } from "src/app/app.service";
import { ROLES_CONST } from "src/app/models/Constants/ROLES_CONST";
import { AppToastService } from "src/app/ui-component/custom-ui-components/toaster/app-toast.service";
import { VerifyRoleService } from "src/app/verify-role.service";

@Component({
	selector: 'app-result',
	templateUrl: './result.component.html',
	styleUrls: ['./result.component.css']
})
export class ResultComponent implements OnInit, OnChanges {
	public resultCodes = RESULT_CODES;
	public scoreCardFile;
	public verify_role_evaluator:boolean;
	public verify_role_learner:boolean;
	public roles: { Evaluator: string; Learner: string; OPM: string; };
	public ResultDetailsForm = new FormGroup({
		resultId: new FormControl(this.resultCodes.notclear.toLowerCase()),
		comments: new FormControl(''),
		score: new FormControl('', [Validators.pattern('^([0-9]{1,5}|[0-9]{0,5}[.][0-9]{1,2})$')]),
		scoreCard: new FormControl('', [Validators.pattern('^.*\.(zip)$')])
	});

	@Input() public requestDetailsModel: requestDetails;
	@Output() public resultDetailsEvent = new EventEmitter();

	constructor(public appService: AppService,private toastService: AppToastService,private verify_role_service:VerifyRoleService) { }

	public ngOnInit() {
		this.roles = ROLES_CONST;
		this.verify_role_evaluator=this.verify_role_service.isEvaluator();
		this.verify_role_learner=this.verify_role_service.isLearner();
		}
	public ngOnChanges(changes: SimpleChanges) {
		if (changes['requestDetailsModel']) {
			if (this.requestDetailsModel) {
				if (this.requestDetailsModel.resultId !== this.resultCodes.evaluate.toLowerCase()) {
					if (this.requestDetailsModel.resultId === this.resultCodes.error.toLowerCase()) {
						this.ResultDetailsForm.controls["resultId"].setValue('');
						this.ResultDetailsForm.controls["comments"].setValue('');
						this.ResultDetailsForm.controls["score"].setValue('');
						this.ResultDetailsForm.disable({ onlySelf: true });
					}
					else {
						this.ResultDetailsForm.patchValue(this.requestDetailsModel);
						this.ResultDetailsForm.disable({ onlySelf: true });
					}
				}
			}
		}

		if (this.verify_role_learner || (this.requestDetailsModel && this.appService.userMid() === this.requestDetailsModel.learnerMid)) {
			this.ResultDetailsForm.disable({ onlySelf: true });
		}

	}

	public onFileChange(event) {
		if (event.target.files.length > 0) {
			const file = event.target.files[0];
			this.scoreCardFile = file;
		}
		else {
			this.scoreCardFile = '';
		}
	}

	public submitDetails() {
		const formData = new FormData();
		formData.append("comments", this.ResultDetailsForm.value.comments);
		formData.append("score", this.ResultDetailsForm.value.score);
		formData.append("resultId", this.ResultDetailsForm.value.resultId);
		formData.append("scoreCardFile", this.scoreCardFile);
		formData.append("Mid",this.requestDetailsModel.learnerMid);
		this.resultDetailsEvent.emit(formData);
	}
}
