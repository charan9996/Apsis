import { Component, OnInit } from '@angular/core';
import { EventEmitter, Output, Input } from '@angular/core';
import { MatModuleModule } from 'src/app/ui-component/mat-module/mat-module.module';
import { FormControl } from "@angular/forms";
@Component({
	selector: 'app-course-search-menu',
	templateUrl: './course-search-menu.component.html',
	styleUrls: ['./course-search-menu.component.css']
})
export class CourseSearchMenuComponent implements OnInit {
	@Output('search') public search = new EventEmitter<string>();
	@Input('searchMessage') public searchMessage = '';
	@Input() searchValue = '';
	public searchBar = new FormControl('');
	public showClear = false;

	constructor() {}
	ngOnInit() {}

	public onSearchClick() {
		this.search.emit(this.searchBar.value);
	}

	public onSearchTextChange() {
		if (this.searchBar.value && this.searchBar.value.length) {
			this.showClear = true;
		} else {
			this.showClear = false;
		}
	}
}
