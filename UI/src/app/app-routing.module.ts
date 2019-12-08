import { ModuleWithProviders } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { NotFoundComponent } from './components/not-found/not-found.component';
import { AuthGuard } from './global-component/guards/auth-guard.service';
import { RoleGuard } from './global-component/guards/role-guard.service';
import { ROLES_CONST } from './models/Constants/ROLES_CONST';

const routes: Routes = [
	{
		path: '',
		redirectTo: 'home',
		pathMatch: 'full'
	},
	{
		path: 'home',
		component: HomeComponent,
		canActivate: [AuthGuard]
	},
	{
		path: 'request',
		loadChildren: './modules/request/request.module#RequestModule',
		canActivate: [RoleGuard],
		data : {
			expectedRoleIds: [
				ROLES_CONST.Evaluator,
				ROLES_CONST.Learner,
				ROLES_CONST.OPM
			]
		}
	},
	{
		path: 'course',
		loadChildren: './modules/course/course.module#CourseModule',
		canActivate:[RoleGuard],
		data : {
			expectedRoleIds: [
				ROLES_CONST.OPM
			]
		}
	},
	{
		path: '**',
		component: NotFoundComponent
	}
];

export const routing: ModuleWithProviders = RouterModule.forRoot(routes);
