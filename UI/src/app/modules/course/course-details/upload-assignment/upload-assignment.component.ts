import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { environment } from 'src/environments/environment';
import { CourseService } from 'src/app/global-component/services/api-mapping-services/course.service';
import { responseStatus } from 'src/app/models/responseStatus';
import { UploadModalService } from 'src/app/ui-component/custom-ui-components/modals/upload-dialog/upload-modal.service';
import { InternalCourseService } from 'src/app/modules/course/internal-course.service';
import { AppToastComponent } from '../../../../ui-component/custom-ui-components/toaster/app-toast.component';
import { AppToastService } from '../../../../ui-component/custom-ui-components/toaster/app-toast.service';
@Component({
	selector: 'app-upload-assignment',
	templateUrl: './upload-assignment.component.html',
	styleUrls: ['./upload-assignment.component.css']
})
export class UploadAssignmentComponent implements OnInit {
	public item: string;
	public courseId: string;
	@Output('fileChanged') public sendFileChange = new EventEmitter<string>();
	constructor(
		private courseService: CourseService,
		private internalCourseService: InternalCourseService,
		private uploadModalService: UploadModalService,
		private appToastService : AppToastService
	) {}

	public ngOnInit() {}

	public openDialog() {
		const modalData = {
			title: 'Upload Assignment',
			note: 'do not select empty file',
			fileformat: ['zip'],
			buttonName: 'Upload',
			message: 'Select a file'
		};
		
		this.uploadModalService.openDialog(modalData).subscribe(file => {
			const formData: FormData = new FormData();
			formData.append('file', file);
			this.internalCourseService.postFile(file).subscribe(data => {
				if (data) {
					const response = data as responseStatus;
					if (response.isSuccess) {
						this.uploadModalService.sendSuccess();
						this.sendFileChange.emit(response.fileUrl);
					} else {
						this.uploadModalService.recieveMessage(response.message);
					}
				}
			});
		});
	}
}
