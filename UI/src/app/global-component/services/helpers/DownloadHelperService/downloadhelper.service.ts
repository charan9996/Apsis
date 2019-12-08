import { Injectable } from '@angular/core';

@Injectable({
	providedIn: 'root'
})
export class DownloadHelperService {
	constructor() {}

	public DownloadFile(Url: string) {
		const link = document.createElement('a');
		link.href = Url;
		link.click();
	}
}
