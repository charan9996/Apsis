import { EvaluatorDetails } from 'src/app/models/EvaluatorDetails';

export class courseDetails {
	public yorbitCourseId: string;
	public id: string;
	public courseName: string;
	public academy: string;
	public batchType: string;
	public courseType: string;
	public evaluators: EvaluatorDetails[];
	public courseProblemStatementUrl: string;
}
