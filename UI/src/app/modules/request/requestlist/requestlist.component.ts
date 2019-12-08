import { Component, HostListener, OnInit } from '@angular/core';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import { Router } from '@angular/router';
import { RequestService } from 'src/app/global-component/services/api-mapping-services/request.service';
import { DownloadHelperService } from 'src/app/global-component/services/helpers/DownloadHelperService/downloadhelper.service';
import { assignmentDueDateResponse } from 'src/app/models/assignmentDueDateResponse';
import { RESULT_CODES } from 'src/app/models/Constants/RESULT_CODES';
import { RequestView } from 'src/app/models/RequestView';
import { responseStatus } from 'src/app/models/responseStatus';
import { E_REQUEST_FILTER, E_SORT_ORDER, RequestFilter, RequestSearchFilter } from 'src/app/models/SearchFilter';
import { BottomNavComponent } from 'src/app/modules/shared/bottom-nav/bottom-nav.component';
import { FilterMenuComponent } from 'src/app/modules/shared/filter-menu/filter-menu.component';
import { AppToastComponent } from 'src/app/ui-component/custom-ui-components/toaster/app-toast.component';
import { ROLES_CONST } from 'src/app/models/Constants/ROLES_CONST';
import { AppService } from 'src/app/app.service';
import { UploadModalService } from 'src/app/ui-component/custom-ui-components/modals/upload-dialog/upload-modal.service';
import { AppToastService } from 'src/app/ui-component/custom-ui-components/toaster/app-toast.service';
import { StorageHelperService } from 'src/app/global-component/services/wrappers/storage-helper.service';
import { DatePipe } from '@angular/common';
import { FormControl, FormGroup, Validators, FormBuilder } from '@angular/forms';
import { PageEvent } from '@angular/material';

@Component({
	selector: 'app-requestlist',
	templateUrl: './requestlist.component.html',
	styleUrls: ['./requestlist.component.css']
})
export class RequestlistComponent implements OnInit {
	public roles: { Evaluator: string; Learner: string; OPM: string };
	public uploadStatus: boolean = false;
	public Sort = E_SORT_ORDER;
	public resultCodes: { error: string; evaluate: string; cleared: string; notclear: string };
	public filterGroup = RequestFilter;
	public requestList: RequestView[] = [];
	public searchFilter = new RequestSearchFilter();
	public pageCount: number;
	public startDate = new Date();
	public _assignmentDueDateResponse = new assignmentDueDateResponse();
	public checkboxes: boolean[] = [];
	public selectedList: string[] = [];
	public isAllSelected = false;
	public searchFilterForm: FormGroup;
	public dropdownList: boolean[] = [];
	public pageEvent: PageEvent;
	public records: number;
	public pageSizeOptions: number[] = [5, 10, 25];
	public showClear = false;
	public pageNumber: number;
	public filterOptions = Object.keys(RequestFilter);

	constructor(
		private requestService: RequestService,
		private appToastService: AppToastService,
		public appService: AppService,
		private downloadHelperService: DownloadHelperService,
		private uploadModalService: UploadModalService,
		private router: Router,
		private storageHelperService: StorageHelperService,
		private datePipe: DatePipe
	) {}

	ngOnInit() {
		this.initialiseFilter();
		this.getConstants();
		this.roles = ROLES_CONST;
		console.log(this.resultCodes.evaluate);
		this.loadRequestList();
	}

	public loadRequestList() {
		this.requestService.getRequestsFiltered(this.searchFilter).subscribe(
			data => {
				this.requestList = data as RequestView[];
				if (this.requestList != null && this.requestList.length > 0) {
					this.records = this.requestList[0].totalRows;
					this.pageCount = this.requestList[0].totalRows / this.searchFilter.PageSize;
					for (let i = 0, len = this.records; i < len; i++) {
						this.checkboxes[i] = false;
						console.log('checkbox initialized: ' + this.checkboxes[i]);
					}
					this.isAllSelected = false;
					this.selectedList = [];
				} else {
					this.requestList = [];
					this.checkboxes = [];
					this.pageCount = 0;
					this.isAllSelected = false;
					this.selectedList = [];
					this.records = 0;
				}
			},
			error => {
				this.requestList = [];
				this.checkboxes = [];
				this.pageCount = 0;
				this.isAllSelected = false;
				this.selectedList = [];
				this.records = 0;
			}
		);
	}

	public selectedCheckBox(id, toggleState) {
		this.checkboxes[id] = toggleState;
		console.log('changed selected ' + this.checkboxes[id]);
		let i = 0;
		for (i = 0; i < this.checkboxes.length; i++) {
			if (this.checkboxes[i] === false) {
				this.isAllSelected = false;
				break;
			} else {
				this.isAllSelected = true;
			}
		}
		const requestId = this.requestList[id].requestId;
		this.toggleInSelectedRequest(requestId);
	}

	public toggleInSelectedRequest(requestId: string) {
		const index = this.selectedList.indexOf(requestId);
		if (index === -1) {
			console.log(requestId + ' added');
			this.selectedList.push(requestId);
		} else {
			console.log(requestId + ' removed');
			this.selectedList.splice(index, 1);
		}
	}

	public getConstants() {
		this.resultCodes = RESULT_CODES;
	}

	public noneSelected() {
		if (this.selectedList.length === 0) {
			return true;
		} else {
			return false;
		}
	}

	public downloadClicked(requestId: string) {
		const requestIds: string[] = [];
		requestIds.push(requestId);
		this.downloadSelectedRequests(requestIds);
		console.log('Request Id: ' + requestIds);
	}

	public downloadAllClicked() {
		// count check
		console.log(this.selectedList);
		this.downloadSelectedRequests(this.selectedList);
	}

	public downloadSelectedRequests(requestIds: string[]) {
		console.log('CLICKED : ' + requestIds);
		this.requestService.downloadAssignmentSolution(requestIds).subscribe(
			data => {
				const value = data as responseStatus;
				if (value.isSuccess) {
					this.downloadHelperService.DownloadFile(value.fileUrl);
					this.requestService.UpdateAssignmentDownloadDate(requestIds).subscribe(data => {});
				} else {
					alert('Download failed : ' + value.message);
				}
			},
			error => {
				alert('Download failed : Server didnt respond.');
			}
		);
	}

	public onSortClicked(sortid: E_SORT_ORDER) {
		this.searchFilter.Sort = sortid;
		this.pageNumber = 0;
		this.searchFilter.CurrentPage = 0;
		console.log('sort clicked: ' + sortid);
		this.loadRequestList();
	}

	public onClearSearch() {
		this.searchFilterForm.controls['keyword'].setValue('');
		this.showClear = false;
		this.onSubmitSearch();
	}

	public onSubmitSearch() {
		this.searchFilter.Keyword = this.searchFilterForm.controls['keyword'].value;
		this.pageNumber = 0;
		this.searchFilter.CurrentPage = 0;
		console.log('search clicked: ' + this.searchFilter.Keyword);
		this.loadRequestList();
	}

	public onFilterGroupClicked() {
		this.searchFilter.CurrentPage = 0;
		this.pageNumber = 0;
		this.searchFilter.Filter = this.searchFilterForm.controls['filterIndex'].value;
		console.log('group clicked: ' + this.searchFilter.Filter);
		this.loadRequestList();
	}

	public onPageSwitch() {
		this.pageNumber = this.pageEvent.pageIndex;
		this.searchFilter.CurrentPage = this.pageEvent.pageIndex + 1;
		this.searchFilter.PageSize = this.pageEvent.pageSize;
		this.loadRequestList();
	}

	public updateAssignmentDueDate(requestId: string, event: MatDatepickerInputEvent<Date>) {
		const newAssignmentDueDate = event.target.value.toLocaleDateString().replace(/[^\x00-\x7F]/g, '');
		console.log(requestId + '  ' + newAssignmentDueDate);
		this.requestService.putResubmissionDate(requestId, newAssignmentDueDate).subscribe(
			data => {
				this._assignmentDueDateResponse = data as assignmentDueDateResponse;
				this.appToastService.showSuccess(this._assignmentDueDateResponse.message);
				if (this._assignmentDueDateResponse.isSuccess) {
					this.loadRequestList();
				}
			},
			error => {
				console.log(error);
				this.appToastService.showError('Due date could not be updated.');
				this.loadRequestList();
			}
		);
	}

	// Function is Called On Submit Button
	public openDialog(request: RequestView) {
		const data = {
			title: 'Upload Solution',
			note: 'do not select empty file',
			fileformat: ['zip'],
			buttonName: 'Upload',
			message: 'Select a file'
		};

		this.uploadModalService.openDialog(data).subscribe(file => {
			const formData: FormData = new FormData();
			formData.append('file', file);
			this.requestService.postAssignmentSolution(request.requestId, formData).subscribe(
				res => {
					const response = res as BaseResponse;
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

	public toggleSelectAll(event) {
		console.log(event);
		console.log('toggle ' + this.isAllSelected);
		this.isAllSelected = event;
		for (let i = 0; i < this.checkboxes.length; i++) {
			this.checkboxes[i] = this.isAllSelected;
			const requestId = this.requestList[i].requestId;
			const index = this.selectedList.indexOf(requestId);
			if (this.checkboxes[i] === true && index === -1) {
				this.selectedList.push(requestId);
			} else {
				this.selectedList.splice(index, 1);
			}
		}
		this.checkboxes.forEach(x => console.log(x));
	}

	public PublishResultClicked(requestId: string) {
		const requestIds: string[] = [];
		requestIds.push(requestId);
		this.PublishResultSelectedRequests(requestIds);
		console.log('Request Id: ' + requestIds);
	}

	public PublishResultAllClicked() {
		console.log(this.selectedList);
		this.PublishResultSelectedRequests(this.selectedList);
	}

	public refreshOnSuccess() {
		this.loadRequestList();
	}

	public PublishResultSelectedRequests(requestIds: string[]) {
		console.log('CLICKED : ' + requestIds);
		this.requestService.publishResult(requestIds).subscribe(
			data => {
				const value = data as assignmentDueDateResponse;
				this.appToastService.showSuccess(value.message);
				if (value.isSuccess) {
					this.loadRequestList();
				}
			},
			error => {
				this.appToastService.showError('Result could not be Published');
			},
			() => {
				console.log('done');
			}
		);
	}

	public selectedRequest(requestId: string) {
		console.log('Select RequestId (on click): ' + requestId);
		const key = 'requestId';
		const filterKey = 'filterKey';
		this.storageHelperService.setItemToLocal(key, requestId);
		this.storageHelperService.setItemToLocal(filterKey, JSON.stringify(this.searchFilter));
		this.router.navigate(['request/details']);
	}

	public downloadReport() {
		this.requestService.getRequestsFiltered(this.searchFilter, true).subscribe(
			data => {
				if (data) {
					if (navigator.msSaveOrOpenBlob) {
						var currentDate = new Date();
						var name = 'RequestReport_' + this.datePipe.transform(currentDate, 'dd-MM-yyyy') + '.xlsx';
						navigator.msSaveOrOpenBlob(data, name);
						return;
					}
					const a = document.createElement('a');
					document.body.appendChild(a);
					a.style.display = 'none';
					const blob = new Blob([data], { type: 'octet/stream' });
					const url = window.URL.createObjectURL(blob);
					a.href = url;
					var date = new Date();
					a.download = 'RequestReport_' + this.datePipe.transform(date, 'dd-MM-yyyy') + '.xlsx';
					a.click();
					window.URL.revokeObjectURL(url);
				}
			},
			error => {
				if (error.status === 400) {
					const response = new Blob([error.error], { type: 'text/plain' });
					const reader = new FileReader();
					reader.addEventListener('loadend', e => {
						this.appToastService.showError(JSON.parse((e.currentTarget as FileReader).result));
					});

					reader.readAsText(response);
					return;
				}
				this.appToastService.showError('No data available to export');
			}
		);
	}

	public onSearchTextChange(val) {
		this.showClear = val && val.length > 0 ? true : false;
	}

	public initialiseFilter() {
		this.searchFilterForm = new FormGroup({
			keyword: new FormControl(''),
			filterIndex: new FormControl(E_REQUEST_FILTER.ALL)
		});

		const localFilter = JSON.parse(this.storageHelperService.getItemFromLocal('filterKey'));
		if (localFilter !== undefined && localFilter !== null) {
			this.searchFilter = localFilter;
			this.pageNumber = this.searchFilter.CurrentPage - 1;
			this.searchFilterForm.controls['keyword'].setValue(this.searchFilter.Keyword);
			this.searchFilterForm.controls['filterIndex'].setValue(this.searchFilter.Filter);

			if (this.searchFilter.Keyword) {
				this.showClear = true;
			}
			this.storageHelperService.removeItemFromLocal('filterKey');
		} else {
			this.pageNumber = 0;
			this.searchFilter.CurrentPage = 0;
			this.searchFilter.PageSize = 10;
		}
	}
}
