import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import {
	MatButtonModule,
	MatCheckboxModule,
	MatDatepickerModule,
	MatInputModule,
	MatMenuModule,
	MatNativeDateModule,
	MatOptionModule,
	MatProgressSpinnerModule,
	MatSelectModule,
	MatTabsModule,
	MatPaginatorModule,
	MatTooltipModule
} from '@angular/material';
import { MatDialogModule } from '@angular/material/dialog';
@NgModule({
	imports: [
		CommonModule,
		MatTabsModule,
		MatInputModule,
		MatNativeDateModule,
		MatDatepickerModule,
		MatButtonModule,
		MatCheckboxModule,
		MatMenuModule,
		MatSelectModule,
		MatOptionModule,
		MatProgressSpinnerModule,
		MatDialogModule,
		MatPaginatorModule,
		MatTooltipModule
	],
	declarations: [],
	exports: [
		MatTabsModule,
		MatInputModule,
		MatNativeDateModule,
		MatDatepickerModule,
		MatButtonModule,
		MatCheckboxModule,
		MatMenuModule,
		MatSelectModule,
		MatOptionModule,
		MatProgressSpinnerModule,
		MatDialogModule,
		MatPaginatorModule,
		MatTooltipModule
	],
	providers: []
})
export class MatModuleModule {}
