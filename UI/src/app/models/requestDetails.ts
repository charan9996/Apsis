import { AttemptsLog } from './AttemptsLog';

export class requestDetails {
    
        public learnerMid: string;
        public learnerName: string;
        public yorbitRequestId : string;
        public location: string;
        public email: string;
        public yorbitCourseId: string;
        public courseName: string;
        public academy: string;
        public assignmentAttemptId: string;
        public score:number;
        public comments: string;
        public resultId: string;
        public resultName: string;
        public assignmentSolutionFileName: string;
        public scoreCardFileName: string;
        public assignmentSolutionUrl: string;
        public scoreCardUrl: string;
        public attemptsLog : AttemptsLog[];
        public errorFileUrl: string;
        public errorFileName : string;
        public evaluatorId : string;
    }