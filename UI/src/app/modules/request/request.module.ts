import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from 'src/app/modules/shared/shared.module';
import { MatModuleModule } from 'src/app/ui-component/mat-module/mat-module.module';
import { MyRequestsComponent } from './my-requests/my-requests.component';
import { RequestRoutingModule } from './request-routing.module';
import { RequestComponent } from './request/request.component';
import { RequestdetailsComponent } from './requestdetails/requestdetails.component';
import { RequestlistComponent } from './requestlist/requestlist.component';
import { ErrorComponent } from './requestdetails/error/error.component';
import { LogComponent } from './requestdetails/log/log.component';
import { ResultComponent } from './requestdetails/result/result.component';

@NgModule({
	imports: [CommonModule, FormsModule, ReactiveFormsModule, RequestRoutingModule, SharedModule, MatModuleModule],
	declarations: [
		RequestlistComponent,
		RequestdetailsComponent,
		MyRequestsComponent,
		RequestComponent,
		ErrorComponent,
		LogComponent,
		ResultComponent
	]
})
export class RequestModule {}
