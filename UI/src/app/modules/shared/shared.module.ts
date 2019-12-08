import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BottomNavComponent } from './bottom-nav/bottom-nav.component';
import { FilterMenuComponent } from './filter-menu/filter-menu.component';
import { FooterComponent } from './footer/footer.component';
import { HeaderComponent } from './header/header.component';
import { RouterModule } from '@angular/router';
import { MatModuleModule } from 'src/app/ui-component/mat-module/mat-module.module';
import { ReactiveFormsModule } from '@angular/forms'; 

@NgModule({
	imports: [CommonModule, RouterModule, MatModuleModule, ReactiveFormsModule],
	exports: [FilterMenuComponent, BottomNavComponent, HeaderComponent, FooterComponent],
	declarations: [FilterMenuComponent, BottomNavComponent, HeaderComponent, FooterComponent]
})
export class SharedModule {}
