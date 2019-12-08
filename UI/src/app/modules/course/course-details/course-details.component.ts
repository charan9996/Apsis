import { Component, OnInit } from '@angular/core';
import { courseDetails } from 'src/app/models/courseDetails';
import { CourseService } from 'src/app/global-component/services/api-mapping-services/course.service';
import { DownloadHelperService } from 'src/app/global-component/services/helpers/DownloadHelperService/downloadhelper.service';
import { Router } from '@angular/router';
import { StorageHelperService } from 'src/app/global-component/services/wrappers/storage-helper.service';

@Component({
	selector: 'app-course-details',
	templateUrl: './course-details.component.html',
	styleUrls: ['./course-details.component.css']
})
export class CourseDetailsComponent implements OnInit {
	public courseDetails: courseDetails;
	public inputMidLength = 0;
	public myItem;
	public disableDownload = false;
	constructor(private courseService: CourseService, 
				private downloadHelperService: DownloadHelperService,
				private router:Router,
				private storageHelperService:StorageHelperService) {}

	ngOnInit() {
		this.myItem = this.storageHelperService.getItemFromLocal('courseId');
		this.loadDetails(this.myItem);
	}

	public onValueChange(inputMidValue: string) {
		this.inputMidLength = inputMidValue.length;
	}

	public enableAddBtn() {
		if (this.inputMidLength === 0) {
			return true;
		} else {
			return false;
		}
	}

	public loadDetails(courseId: string) {
		this.courseService.getEvaluators(courseId).subscribe(data => {
			if (data) {
				this.courseDetails = data as courseDetails;
				if (
					this.courseDetails.courseProblemStatementUrl == null ||
					this.courseDetails.courseProblemStatementUrl === ''
				) {
					this.disableDownload = true;
				} else {
					this.disableDownload = false;
				}
			}
		});
	}

	public addEvaluator(inpbox) {
		const mid = inpbox.value;
		this.courseService.addEvaluator(this.myItem, mid).subscribe(
			data => {
				if (data) {
					inpbox.value = '';
					this.loadDetails(this.myItem);
					this.inputMidLength = 0;
				}
			},
			error => {
				inpbox.value = '';
				this.inputMidLength = 0;
				alert('Invalid MID');
			},
			() => {
				console.log('done');
			}
		);
	}

	public deleteEvaluator(evaluatorId: string) {
		console.log('delete value: ' + evaluatorId);
		this.courseService.deleteEvaluator(this.myItem, evaluatorId).subscribe(data => {
			if (data) {
				//this.courseDetails = data.body as courseDetails;
				this.loadDetails(this.myItem);
			}
		});
	}

	public downloadCourseAssignment(FileUrl: String) {
		if (FileUrl != null && FileUrl.length > 0) {
			this.downloadHelperService.DownloadFile(String(FileUrl));
		} else {
			alert('No File Available');
		}
	}

	public onFileChange(fileUrl) {
		this.disableDownload = false;
		this.courseDetails.courseProblemStatementUrl = fileUrl;
	}
	public Back()
	{
		this.router.navigate(['course']);
	}
}
