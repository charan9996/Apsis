import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { RequestComponent } from './request/request.component';
import { RequestlistComponent } from 'src/app/modules/request/requestlist/requestlist.component';
import { RequestdetailsComponent } from 'src/app/modules/request/requestdetails/requestdetails.component';
import { MyRequestsComponent } from 'src/app/modules/request/my-requests/my-requests.component';
import { NotFoundComponent } from 'src/app/components/not-found/not-found.component';
import { RoleGuard } from "src/app/global-component/guards/role-guard.service";
import { ROLES_CONST } from "src/app/models/Constants/ROLES_CONST";
const routes: Routes = [
	{
		path: '',
		children: [
			{
				path: '',
				redirectTo: 'list',
				pathMatch: 'full'
				//component: RequestlistComponent
			},
			{
				path: 'list',
				component: RequestlistComponent,
				canActivate:[RoleGuard],
				data : {
					expectedRoleIds: [
						ROLES_CONST.Evaluator,
						ROLES_CONST.OPM
					]
				}
			},
			{
				path: 'my-requests',
				component: MyRequestsComponent,
				canActivate:[RoleGuard],
				data : {
					expectedRoleIds: [
						ROLES_CONST.Evaluator,
						ROLES_CONST.Learner
					]
				}
			},
			{
				path: 'details',
				component: RequestdetailsComponent
			}
		]
	},
	{
		path: '**',
		component: RequestdetailsComponent
	}
];

@NgModule({
	imports: [RouterModule.forChild(routes)],
	exports: [RouterModule]
})
export class RequestRoutingModule {}
