import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { Router } from '@angular/router';
import { CourseSearchFilter, E_COURSE_SORT } from 'src/app/models/SearchFilter';
import { courseView } from 'src/app/models/courseView';
import { CourseService } from 'src/app/global-component/services/api-mapping-services/course.service';
import { InternalCourseService } from 'src/app/modules/course/internal-course.service';
import { StorageHelperService } from 'src/app/global-component/services/wrappers/storage-helper.service';
import {PageEvent} from '@angular/material';

@Component({
	selector: 'app-courses',
	templateUrl: './course-list.component.html',
	styleUrls: ['./course-list.component.css']
})
export class CourseListComponent implements OnInit {
	@Input('searchMessage') public searchMessage = '';
	public filterOptions = Object.keys(CourseSearchFilter);
	public pageCount: number;
	public Sort = E_COURSE_SORT;
	public searchFinishMessage = '';
	public searchedValued ='';
	public searchFilter = new CourseSearchFilter();
	public courses: courseView[] = [];
	public records:number;
	public pageEvent:PageEvent;
	public pageNumber:number;
	public pageSizeOptions:number[]= [5,10,25];
	constructor(
		private courseService: CourseService,
		private router: Router,
		private internalCourseService: InternalCourseService,
		private storageHelperService: StorageHelperService
	) {
	}

	ngOnInit() {
		this.pageNumber=0;
		this.searchFilter.CurrentPage = 0;
		this.searchFilter.PageSize = 10;
		let filter= JSON.parse(this.storageHelperService.getItemFromLocal('searchFilter')); 
		if(filter != null && filter != undefined )
		  {
			this.searchFilter = filter as CourseSearchFilter ;
			this.searchedValued = this.searchFilter.Keyword;
			this.pageNumber=filter.CurrentPage-1;	 
		  }
		  if(this.searchFilter.Keyword==undefined)
		  {
			this.searchedValued = '';
		  }
		  
		this.getCourseSorted();
		this.storageHelperService.removeItemFromLocal('searchFilter');
	}

	public getCourseSorted() {
		this.courseService.getCoursesFiltered(this.searchFilter).subscribe(data => {
			if (data) {
				this.courses = data as courseView[];
				this.records=this.courses[0].rowsCount;
				this.pageCount = this.courses[0].rowsCount / this.searchFilter.PageSize;
			} else {
				console.log('RESETING course');
				this.courses = [];
				this.pageCount = 0;
			}
		}, error => ((this.courses = []), (this.pageCount = 0)));
	}

	public onSortClicked(sortid: E_COURSE_SORT) {
		this.searchFilter.Sort = sortid;
		this.searchFilter.CurrentPage = 0;
		console.log('sort clicked: ' + sortid);
		this.getCourseSorted();
	}

	public onSubmitSearch(searchText: string) {
		this.searchFilter.Keyword = searchText;
		this.searchFilter.CurrentPage = 0;
		console.log('search clicked: ' + searchText);
		this.getCourseSorted();
		if (searchText.length === 0) {
			this.searchFinishMessage = '';
		} else {
			this.searchFinishMessage = "Showing results for: '" + searchText + "'";
		}
	}

	public selectedCourse(courseId: string) {
		var filter=JSON.stringify(this.searchFilter);
		this.storageHelperService.setItemToLocal('searchFilter',filter);
		console.log('courseId: ' + courseId);
		this.internalCourseService.saveCourseId(courseId);
		const key = 'courseId';
		this.storageHelperService.setItemToLocal(key,courseId);
		//localStorage.setItem(key, courseId);
		this.router.navigate(['/course/details']);
	}
	public getData(){
		this.pageNumber=this.pageEvent.pageIndex;
		this.searchFilter.CurrentPage=this.pageEvent.pageIndex+1;
		this.searchFilter.PageSize=this.pageEvent.pageSize;
		this.getCourseSorted();
	}

}
