import { Injectable } from '@angular/core';
import { CourseService } from 'src/app/global-component/services/api-mapping-services/course.service';
import { environment } from 'src/environments/environment';
@Injectable({
	providedIn: 'root'
})
export class InternalCourseService {
	private courseId: string;
	private content: any;
	private item: string;

	constructor(private courseService: CourseService) {}

	public saveCourseId(id: string) {
		this.courseId = id;
	}

	public postFile(fileToUpload: File) {
		this.item = localStorage.getItem('courseId');
		const formData: FormData = new FormData();
		formData.append('file', fileToUpload);
		return this.courseService.postAssignment(this.item, formData);
	}
}
