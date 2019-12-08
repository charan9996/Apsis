import { Injectable } from '@angular/core';
import { Message } from './Message';

@Injectable({
	providedIn: 'root'
})
export class AppToastService {
	public MessageArray: Message[];
	constructor() {
		this.MessageArray = [];
	}

	public getMessages(): Message[] {
		return this.MessageArray;
	}

	public showError(content) {
		const message = new Message(content, 'error');
		this.MessageArray.push(message);
		this.dismissAfterSomeTime();
	}

	public showSuccess(content) {
		const message = new Message(content, 'success');
		this.MessageArray.push(message);
		this.dismissAfterSomeTime();
	}

	public showInfo(content) {
		const message = new Message(content, 'info');
		this.MessageArray.push(message);
		this.dismissAfterSomeTime();
	}

	public dismissAfterSomeTime() {
		setTimeout(() => {
			this.dismissMessage(0);
		}, 40 * 1000);
	}

	public dismissMessage(index: number) {
		this.MessageArray.splice(index, 1);
	}
}
