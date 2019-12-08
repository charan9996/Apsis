import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CourseSearchFilter } from 'src/app/models/SearchFilter';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';

/**
 * Route: /Course
 */

@Injectable({
	providedIn: 'root'
})
export class CourseService {
	constructor(private http: HttpClient) {}

	/**
  * Route: /{id}/assignment
  * Body: FormData having a file.
  * Method to post assignment file against a courseId
  */
	public postAssignment(courseId: string, formWithFile: FormData) {
		return this.http.post(`${environment.apiUrl}/course/${courseId}/assignment`, formWithFile);
	}

	/**
  * Route: /
  * Body: none.
  * Method to get all courses.
  */
	public getCourses() {
		return this.http.get(`${environment.apiUrl}/course/`);
	}

	/**
  * Route: /
  * Body: CourseFilter having required filter.
  * Method to get all courses based on a filter.
  * @param courseFilter
  */
	public getCoursesFiltered(courseFilter: CourseSearchFilter) {
		return this.http.post(`${environment.apiUrl}/course/`, courseFilter);
	}

	/**
  * Route: /{id}/evaluator
  * Body: none.
  * Method to get evaluators against a courseId.
  * @param courseId
  */
	public getEvaluators(courseId: string) {
		return this.http.get(`${environment.apiUrl}/course/${courseId}/evaluator`);
	}

	/**
  * Route: /{id}/evaluator/{mid}
  * Body: none.
  * Method to get evaluators against a courseId.
  * @param courseId
  * @param evaluatorMid
  */
	public addEvaluator(courseId: string, evaluatorMid: string) {
		return this.http.put(`${environment.apiUrl}/course/${courseId}/evaluator/${evaluatorMid}`, evaluatorMid);
	}

	/**
  * Route: /{id}/delete/{evaluatorId}
  * Body: none.
  * Method to delete evaluator against a courseId.
  * @param courseId
  * @param evaluatorId
  */
	public deleteEvaluator(courseId: string, evaluatorId: string) {
		return this.http.delete(`${environment.apiUrl}/course/${courseId}/evaluator/${evaluatorId}`);
	}
}
