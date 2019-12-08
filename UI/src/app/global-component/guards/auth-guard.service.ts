import { Injectable } from '@angular/core';
import {
	ActivatedRouteSnapshot,
	CanActivate,
	CanActivateChild,
	CanLoad,
	Route,
	Router,
	RouterStateSnapshot
} from '@angular/router';
import { AuthService } from '../authentication/auth.service';

@Injectable({
	providedIn: 'root'
})
export class AuthGuard implements CanActivate, CanLoad, CanActivateChild {
	constructor(private authService: AuthService, private router: Router) {}

	public canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
		const url: string = state.url;
		return this.checkLogin(url);
	}

	public canActivateChild(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
		return this.canActivate(route, state);
	}

	public canLoad(route: Route): boolean {
		const url = `/${route.path}`;
		return this.checkLogin(url);
	}

	private checkLogin(url: string): boolean {
		if (this.authService.isOnline()) {
			return true;
		}

		// Store the attempted URL for redirecting
		this.authService.redirectUri = url;

		// Navigate to the login page
		this.authService.login();
		return false;
	}
}
