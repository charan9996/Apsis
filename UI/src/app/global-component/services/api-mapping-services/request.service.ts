import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Evaluation } from 'src/app/models/Evaluation';
import { RequestSearchFilter } from 'src/app/models/SearchFilter';
import { environment } from 'src/environments/environment';

/**
 * Route: /Request
 */

@Injectable({
	providedIn: 'root'
})
export class RequestService {
	constructor(private http: HttpClient) {}

	/**
  * Route: /my-requests
  * Body: none.
  * Method to get requests for currently logged in learner.
  */
	public getLearnerRequests() {
		return this.http.get(`${environment.apiUrl}/request/my-requests`);
	}

	/**
  * Route: /
  * Body: none.
  * Method to get all requests based on current filter.
  */
	public getRequestsFiltered(requestSearchFilter: RequestSearchFilter, isExport?:boolean) {
    if(isExport === true)
		  return this.http.post(`${environment.apiUrl}/request?IsExport=true`, requestSearchFilter, {responseType: "blob"});
    return this.http.post(`${environment.apiUrl}/request/`, requestSearchFilter);
	}

	/**
  * Route: /assignment-solution
  * Body: requestId array containing Guids
  * Method: to download all assignments
  * @param requestIds
  */
	public downloadAssignmentSolution(requestIds: string[]) {
		return this.http.post(`${environment.apiUrl}/request/assignment-solution`, requestIds);
	}

	/**
  * Route: /{requestId}/resubmission-date
  * Body: newAssignmentDueDate in localstring to be update
  * Method: to update resubmision date against a requestId
  * @param requestId
  * @param newAssignmentDueDate
  */

	public putResubmissionDate(requestId: string, newAssignmentDueDate: string) {
    const stringified = JSON.stringify(newAssignmentDueDate);
    return this.http.put(`${environment.apiUrl}/request/${requestId}/resubmission-date`, stringified, 
      {headers: new HttpHeaders({"Content-Type": "application/json"})});
	}

	/**
  * Route: /{id}/assignment-solution
  * Body: none
  * Method: to upload solution against a requestId
  * @param requestId
  * @param
  */
	public postAssignmentSolution(requestId: string, formWithFile: FormData) {
		return this.http.post(`${environment.apiUrl}/request/${requestId}/assignment-solution`, formWithFile);
	}

	/**
  * Route: /request-details/{requestId}
  * Body: none
  * Method: to get details of requestId
  * @param requestId
  */
	public getRequestDetails(requestId: string) {
		return this.http.get(`${environment.apiUrl}/request/${requestId}`);
	}

	/**
  * Route: /result-publish
  * Body: none
  * Method: to publish results for a list of requestIds
  * @param requestIds
  */
	public publishResult(requestIds: string[]) {
		return this.http.post(`${environment.apiUrl}/request/result-publish`, requestIds);
	}

	/**
  * Route: /{id}/evaluation
  * Body: none
  * Method: to update evaluation against a resultId
  * @param requestId
  * @param evaluation
  */
	public updateEvaluationResult(requestId: string, evaluation: FormData) {
		return this.http.put(`${environment.apiUrl}/request/${requestId}/evaluation`, evaluation);
  }
  
  /*** Route: /{id}/error
  * Body: formData having files and comments
  * Method: to update error scorecard against a requestId
  * @param requestId
  * @param form
  */
   public postErrorAsync(requestId: string, form: FormData) {
		return this.http.post(`${environment.apiUrl}/request/${requestId}/error`, form);
  }

  	/**
  * Route: /assignment-solution
  * Body: requestId array containing Guids
  * Method: to download all assignments
  * @param requestIds
  */
  public UpdateAssignmentDownloadDate(requestIds: string[])
  {
    return this.http.put(`${environment.apiUrl}/Request/DownloadDate`, requestIds);
  }
  
 
  
}
