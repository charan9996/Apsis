import { Injectable } from '@angular/core';
import { AppService } from "src/app/app.service";
import { ROLES_CONST } from "src/app/models/Constants/ROLES_CONST";

@Injectable({
  providedIn: 'root'
})
export class VerifyRoleService {

  public roles: { Evaluator: string; Learner: string; OPM: string; };
  constructor(public appService: AppService) { 
    this.roles=ROLES_CONST;
  }

  ngOnInit() {
    
  }
  public isLearner()
  {
    if(this.appService.userRoleId() === this.roles.Learner){
     return true;
    }

    return false;
  }
  public isEvaluator()
  {
    if(this.appService.userRoleId() === this.roles.Evaluator){
      return true;
    }

    return false;
  }

}
