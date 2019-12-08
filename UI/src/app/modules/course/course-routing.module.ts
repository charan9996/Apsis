import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CourseListComponent } from 'src/app/modules/course/courselist/course-list.component';
import { CourseDetailsComponent } from 'src/app/modules/course/course-details/course-details.component';

const routes: Routes = [
	{ path: '', component: CourseListComponent },
	{ path: 'details', component: CourseDetailsComponent }
];

@NgModule({
	imports: [RouterModule.forChild(routes)],
	exports: [RouterModule]
})
export class CourseRoutingModule {}
