import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { RequestFilter, E_REQUEST_FILTER } from 'src/app/models/SearchFilter';
import { AppToastService } from 'src/app/ui-component/custom-ui-components/toaster/app-toast.service';
import { AppService } from 'src/app/app.service';
import { ROLES_CONST } from 'src/app/models/Constants/ROLES_CONST';

@Component({
	selector: 'app-filter-menu',
	templateUrl: './filter-menu.component.html',
	styleUrls: ['./filter-menu.component.css']
})
export class FilterMenuComponent implements OnInit {
	public roles: { Evaluator: string; Learner: string; OPM: string };
	@Output('search') public submitSearch = new EventEmitter<string>();
	@Output('filter') public submitFilterGroup = new EventEmitter<E_REQUEST_FILTER>();
	@Output('searchText') public submitSearchTextChange = new EventEmitter<string>();
	public filterOptions = Object.keys(RequestFilter);
	public showClear: boolean = false;

	constructor(private toastService: AppToastService, public appService: AppService) {}
	ngOnInit() {
		this.roles = ROLES_CONST;
	}

	public onSearchClick(searchText: string) {
		console.log('emit- ' + searchText);
		this.submitSearch.emit(searchText);
	}

	public onFilterChange(index: E_REQUEST_FILTER) {
		console.log('click filter group ' + this.filterOptions[index]);
		this.toastService.showInfo('Showing Results for ' + this.filterOptions[index]);
		this.submitFilterGroup.next(index);
	}

	public onSearchTextChange(val: string) {
		if (val && val.length > 0) {
			this.showClear = true;
		} else {
			this.showClear = false;
		}
		this.submitSearchTextChange.next(val.trim());
	}
}
