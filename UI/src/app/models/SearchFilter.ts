export class SearchFilter {
	public Keyword: string;
	public CurrentPage: number;
	public PageSize: number = 10;
}
export class RequestSearchFilter extends SearchFilter {
	public Filter: E_REQUEST_FILTER;
	public Sort: E_SORT_ORDER;
}
export enum E_REQUEST_FILTER {
	ALL,
	ERROR,
	Result_Awaited,
	Project_Cleared,
	Project_Not_Cleared,
	YET_TO_SUBMIT_Project,
	YET_TO_PUBLISHED_Result
}
export var RequestFilter = {
	'All': E_REQUEST_FILTER.ALL,
	'Error': E_REQUEST_FILTER.ERROR,
	'Result awaited': E_REQUEST_FILTER.Result_Awaited,
	'Course completed': E_REQUEST_FILTER.Project_Cleared,
	'Project not cleared': E_REQUEST_FILTER.Project_Not_Cleared,
	'Yet to submit project': E_REQUEST_FILTER.YET_TO_SUBMIT_Project,
	'Yet to publish result': E_REQUEST_FILTER.YET_TO_PUBLISHED_Result
};

export enum E_SORT_ORDER {
	YORBIT_ID_ASC,
	YORBIT_ID_DESC,
	COURSE_NAME_ASC,
	COURSE_NAME_DESC,
	LEARNER_NAME_ASC,
	LEARNER_NAME_DESC,
	LEARNER_MID_ASC,
	LEARNER_MID_DESC,
	ACADEMY_NAME_ASC,
	ACADEMY_NAME_DESC,
	EVALUATION_DATE_ASC,
	EVALUATION_DATE_DESC,
	SUBMISSION_DATE_ASC,
	SUBMISSION_DATE_DESC
}

export class CourseSearchFilter extends SearchFilter {
	public Sort: E_COURSE_SORT;
}
export enum E_COURSE_SORT {
	COURSE_NAME_ASC,
	COURSE_NAME_DESC,
	ACADEMY_NAME_ASC,
	ACADEMY_NAME_DESC,
	BATCH_TYPE_ASC,
	BATCH_TYPE_DESC,
	COURSE_TYPE_ASC,
	COURSE_TYPE_DESC,
	YORBIT_COURSE_ID_ASC,
	YORBIT_COURSE_ID_DESC
}
