import { Component, OnInit } from '@angular/core';
import { AppToastService } from 'src/app/ui-component/custom-ui-components/toaster/app-toast.service';

@Component({
	selector: 'app-toast',
	templateUrl: './app-toast.component.html',
	styleUrls: ['./app-toast.component.css']
})
export class AppToastComponent implements OnInit {
	public messages: any;
	constructor(private toastService: AppToastService) {}

	ngOnInit() {
		this.messages = this.toastService.getMessages();
	}

	public dismiss(index: number) {
		this.toastService.dismissMessage(index);
	}
}
