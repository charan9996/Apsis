import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

/**
 * Route: /Upload
 */

@Injectable({
	providedIn: 'root'
})
export class UploadService {
	constructor(private http: HttpClient) {}

	/**
  * Route: /yorbit-input
  * Body: formWithFile the file to upload.
  * Method to get requests for currently logged in learner.
  * @param formWithFile
  */
	public postRequestInputFile(formWithFile: FormData) {
		return this.http.post(`${environment.apiUrl}/Upload/yorbit-input`, formWithFile);
	}

	public postCourseFile(formWithFile: FormData) {
		return this.http.post(`${environment.apiUrl}/Upload/yorbitCourse-input`, formWithFile);
	}
}
