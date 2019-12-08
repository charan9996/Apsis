import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, CanActivateChild, CanLoad, Route, Router, RouterStateSnapshot } from '@angular/router';
import { AppService } from '../../app.service';
import { AuthService } from '../authentication/auth.service';

@Injectable({
  providedIn: 'root'
})
export class RoleGuard implements CanActivate, CanActivateChild, CanLoad {

  constructor(private authService: AuthService, private router: Router, private appService: AppService) { }

  public canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    const url: string = state.url;
    const expectedRole = route.data.expectedRoleIds;
    if (this.checkLogin(url) && this.checkAuthorization(expectedRole)) {
      return true;
    };

    return false;
  }

  public canActivateChild(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    return this.canActivate(route, state);
  }

  public canLoad(route: Route): boolean {
    const url = `/${route.path}`;
    const expectedRole: string[] = route.data.expectedRoleIds;
    return this.checkLogin(url) && this.checkAuthorization(expectedRole);
  }

  private checkLogin(url: string): boolean {
    if (this.authService.isOnline()) { return true; }

    // Store the attempted URL for redirecting
    this.authService.redirectUri = url;

    // Navigate to the login page
    this.authService.login();
    return false;
  }

  private checkAuthorization(expectedRole: string[]) {
    const userRoleId = this.appService.userRoleId();
    const role = expectedRole.findIndex(r => r === userRoleId );
    if (role !== -1) {
      return true;
    }
    return false;
  }
}
