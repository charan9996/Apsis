import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AppService } from './app.service';
import { UserService } from './global-component/services/api-mapping-services/user.service';
import { FlyerService } from './global-component/services/wrappers/flyer.service';
import { IUser } from './models/User.model';
import { AuthService } from './global-component/authentication/auth.service';
import { ROLES_CONST } from './models/Constants/ROLES_CONST';
import { StorageHelperService } from './global-component/services/wrappers/storage-helper.service';

@Component({
	selector: 'app-root',
	templateUrl: './app.component.html',
	styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
	constructor(
		private userService: UserService,
		private appService: AppService,
		private authService: AuthService,
		private storageService: StorageHelperService
	) {}

	public ngOnInit() {
		if (this.authService.isOnline()) {
			this.userService.getCurrentUser().subscribe(data => {
				if (data != null) {
					const user = data as IUser;
					//user.roleId = ROLES_CONST.Evaluator;
					this.appService.setUser(user);
				}
			});
		}
	}
}
