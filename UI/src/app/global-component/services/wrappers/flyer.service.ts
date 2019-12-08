import { Injectable } from '@angular/core';

@Injectable({
	providedIn: 'root'
})
export class FlyerService {
	public showSpinner: boolean;

	constructor() {
		this.showSpinner = false;
	}

	public enableSpinner() {
		this.showSpinner = true;
	}

	public disableSpinner() {
		this.showSpinner = false;
	}
}
