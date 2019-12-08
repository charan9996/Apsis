import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class StorageHelperService {

  public getItemFromLocal(key: string) {
    return localStorage.getItem(key);
  }

  public getItemFromSession(key: string) {
    return sessionStorage.getItem(key);
  }

  public setItemToLocal(key: string, value: any) {
    return localStorage.setItem(key, value);
  }

  public removeItemFromLocal(key:string){
    return localStorage.removeItem(key); 
  }
}
