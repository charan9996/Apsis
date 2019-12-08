import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AngularFontAwesomeModule } from 'angular-font-awesome';
import { NotFoundComponent } from 'src/app/components/not-found/not-found.component';
import { SharedModule } from 'src/app/modules/shared/shared.module';
import { CustomUiComponentsModule } from 'src/app/ui-component/custom-ui-components/custom-ui-components.module';
import { MatModuleModule } from 'src/app/ui-component/mat-module/mat-module.module';
import { routing } from './app-routing.module';
import { AppComponent } from './app.component';
import { HTTPListener } from 'src/app/global-component/services/http-interceptor/http-interceptor.service';
import { HomeComponent } from './components/home/home.component';
import {DatePipe} from '@angular/common';
@NgModule({
	imports: [
		BrowserAnimationsModule,
		BrowserModule,
		HttpClientModule,
		routing,
		AngularFontAwesomeModule,
		FormsModule,
		ReactiveFormsModule,
		SharedModule,
		MatModuleModule,
		CustomUiComponentsModule
	],
	exports: [MatModuleModule],
	declarations: [AppComponent, NotFoundComponent, HomeComponent],
	bootstrap: [AppComponent],
	providers: [
		HTTPListener,
		{
			provide: HTTP_INTERCEPTORS,
			useClass: HTTPListener,
			multi: true
		},
		DatePipe
	]
})
export class AppModule {}
