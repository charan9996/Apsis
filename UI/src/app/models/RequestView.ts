export class RequestView {
	constructor(
		public totalRows: number,
		public requestId: string,
		public submissionDate: Date,
		public resultId: string,
		public assignmentDueDate: Date,
		public yorbitRequestId: string,
		public name: string,
		public mid: string,
		public yorbitCourseId: string,
		public academy: string,
		public courseName: string,
		public isPublished: boolean
	
	) {}
}
