import { Injectable } from '@angular/core';
import { IUser } from './models/User.model';
import { StorageHelperService } from './global-component/services/wrappers/storage-helper.service';

@Injectable({
  providedIn: 'root'
})
export class AppService {
  public user: IUser

  constructor(private storageService: StorageHelperService){}

  public setUser(user: IUser) {
    this.user = user;
    this.storageService.setItemToLocal('user',JSON.stringify(user));
  }

  public userMid(){
    if(this.user){
     return this.user.mid;
    }
  }

  public userRoleId() {
    const storageUserString  = this.storageService.getItemFromLocal('user');
    if (this.user) {
      return this.user.roleId;
    } else if(storageUserString) {
      this.user = JSON.parse(storageUserString);
      return this.user.roleId;
    }


    return '';
  }
}
