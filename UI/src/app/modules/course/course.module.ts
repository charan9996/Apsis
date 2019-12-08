import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CourseRoutingModule } from './course-routing.module';
import { CourseListComponent } from 'src/app/modules/course/courselist/course-list.component';
import { CourseDetailsComponent } from 'src/app/modules/course/course-details/course-details.component';
import { CourseSearchMenuComponent } from 'src/app/modules/course/courselist/course-search-menu/course-search-menu.component';
import { SharedModule } from 'src/app/modules/shared/shared.module';
import { MatModuleModule } from 'src/app/ui-component/mat-module/mat-module.module';
import { UploadAssignmentComponent } from 'src/app/modules/course/course-details/upload-assignment/upload-assignment.component';
import { FormsModule,ReactiveFormsModule } from '@angular/forms'; 

@NgModule({
	imports: [CommonModule, CourseRoutingModule, SharedModule, MatModuleModule,FormsModule,ReactiveFormsModule],
	declarations: [CourseListComponent, CourseDetailsComponent, CourseSearchMenuComponent, UploadAssignmentComponent]
})
export class CourseModule {}
