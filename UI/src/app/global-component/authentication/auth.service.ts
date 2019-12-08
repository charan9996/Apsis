import { Injectable, OnInit } from '@angular/core';
import * as Msal from 'msal';
import { StorageHelperService } from '../services/wrappers/storage-helper.service';
import { authConfig } from './Config';
@Injectable({
	providedIn: 'root'
})
export class AuthService implements OnInit {
	public redirectUri: string;
	private clientApplication: Msal.UserAgentApplication;

	constructor(private storageService: StorageHelperService) {
		this.clientApplication = new Msal.UserAgentApplication(
			authConfig.clientID,
			authConfig.authority,
			this.authCallback,
			{ redirectUri: authConfig.redirectUri, navigateToLoginRequestUrl: false, cacheLocation: 'localStorage' }
		);
	}

	public ngOnInit() {
		this.redirectUri = '';
	}

	public login(): void {
		this.clientApplication.loginRedirect(authConfig.scopes);
	}

	public logout(): void {
		this.clientApplication.logout();
		localStorage.clear();
	}

	public isOnline(): boolean {
		const user = this.clientApplication.getUser() != null;
		return user;
	}

	public getUser(): Msal.User {
		return this.clientApplication.getUser();
	}

	get getAuthTokenFromStorage() {
		return this.storageService.getItemFromLocal('msal.idtoken');
	}

	public getAuthenticationToken(): Promise<string> {
		return this.clientApplication
			.acquireTokenSilent(authConfig.scopes)
			.then(token => {
				console.log('Got silent access token: ', token);
				return token;
			})
			.catch(error => {
				console.log('Could not silently retrieve token from storage.', error);
				return this.clientApplication
					.acquireTokenPopup(authConfig.scopes)
					.then(token => {
						console.log('Got popup access token: ', token);
						return Promise.resolve(token);
					})
					.catch(innererror => {
						console.log('Could not retrieve token from popup.', innererror);
						return Promise.resolve('');
					});
			});
	}

	private authCallback(errorDesc: any, token: any, error: any, tokenType: any) {
		console.log('auth callback');
		if (token) {
			console.log('Got token', token);
		} else {
			console.log(error + ':' + errorDesc);
		}
	}
}
