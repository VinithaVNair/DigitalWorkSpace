import { Component, OnInit } from '@angular/core';
import { CatalogService } from '../catalog.services';
import { ActivatedRoute } from '@angular/router';
import { IAdmin } from '../../model/admin';
import { UserConfig } from '../../services/userconfig.service';
import { INewAdminRequest } from '../../model/newAdmin';
import { IUser } from '../../model/user';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.scss']
})
export class AdminComponent implements OnInit {

  constructor(private catalogService:CatalogService,private route: ActivatedRoute,private userService:UserConfig) { }
public catalogId:number;
public action:string;
public isAdd:boolean;
public admins:IAdmin[];
public userName:string;
private user:IUser;
public message:string;

  ngOnInit(): void { 
    this.route.queryParams.subscribe(params => {
      this.catalogId = params['catalogId'];
      this.action = params['action'];
    });
    if(this.action=="add")
    {
      this.isAdd=true;
    }
    else
    {
      this.isAdd=false;
    }
}
removeAdmin()
{
  this.catalogService.GetAllAdmin(this.catalogId).subscribe(data=>this.admins=data);
}
alter()
{
  this.userService.GetUserByName(this.userName).subscribe(data=>this.performAction(data));
}
performAction(userInput:IUser)
{
  this.user=userInput;
  if(this.user)
  {
    if(this.isAdd)
    {
      let newUserRepquest:INewAdminRequest={
        'adminId':this.userService.getUserId(),
        'userId':this.user.userId
      }
      this.catalogService.addAdmin(this.catalogId,newUserRepquest).subscribe(d=>this.OnSucess(d));
    }
    else
    {
      this.catalogService.removeAdmin(this.catalogId,this.user.userId,this.userService.getUserId()).subscribe(d=>this.OnSucess(d));
    }
  }
  else
  {
    console.log("user not found {0}",this.user)
  }
}
OnSucess(userChanged:IAdmin)
{
if(this.isAdd)
{
  this.message="Admin added successfully"
}
else{
  this.message="Admin removed successfully"
}
}
}