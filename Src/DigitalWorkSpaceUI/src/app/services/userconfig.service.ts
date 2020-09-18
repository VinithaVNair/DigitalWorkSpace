import { Injectable } from "@angular/core";
import { HttpClient, HttpErrorResponse, HttpHeaders, HttpParams } from "@angular/common/http";
import { AppConfig } from "./app.config.services";
import { Observable, throwError } from "rxjs";
import { catchError } from "rxjs/operators";
import { IUser } from "../model/user";
import { BroadcastService, MsalService } from "@azure/msal-angular";


const httOptions = {
    headers: new HttpHeaders(
       {
          'Content-Type': 'application/json'
       }
    )
 };
 @Injectable(
    {
        providedIn:'root'
    }
)
export  class  UserConfig{
    private user:IUser;
    constructor(private http:HttpClient,private broadcastService: BroadcastService, private authService: MsalService) {
        
    }
    initialize(){
         if(!this.user||this.user.userName!=this.authService.getAccount().userName)
         {
       this.CreateUser(this.authService.getAccount().userName).subscribe(data=>this.assignUser(data));
         }
    }

    assignUser(user:IUser)
    {
        if(user!=null&&user.userId)
        {
            console.log('user found '+user.userId);
            this.user={
                'userId':user.userId,
                'userName':user.userName
            }
        }
        else
        {

            this.CreateUser(this.authService.getAccount().userName).subscribe(d=>this.assignUser(d));
        }

    }

    getUserId():number
    {
        return this.user.userId;
    }
    getUser():IUser
    {
        return this.user;
    }
    logout()
    {
        this.user=null;
    }

    public GetUser():Observable<IUser> {
       let account=this.authService.getAccount()
        var url=AppConfig.settings.users+'/'+account.userName;
        return this.http.get<IUser>(url).pipe(catchError(this.handleError));
    }

    public GetUserByName(userName:string):Observable<IUser> {
         var url=AppConfig.settings.users+'/'+userName;
         return this.http.get<IUser>(url).pipe(catchError(this.handleError));
     }

    public CreateUser(userName:string):Observable<IUser> {
        var url=AppConfig.settings.users;
        return this.http.post<IUser>(url,JSON.stringify( userName),httOptions).pipe(catchError(this.handleError));
    }

private handleError(err: HttpErrorResponse){
    let errorMessage = '';
    if(err.error instanceof ErrorEvent){
       errorMessage='An error occured'+ err.error.message;
    }
    else{
       errorMessage='error'+err.error.message
    }
    return throwError(errorMessage);
 }
}