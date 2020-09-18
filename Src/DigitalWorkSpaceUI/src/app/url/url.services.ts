import { Injectable } from "@angular/core";
import { HttpClient, HttpHeaders,HttpErrorResponse } from "@angular/common/http";
import { Observable, throwError, from } from "rxjs";
import { catchError } from "rxjs/operators";
import { AppConfig } from "../services/app.config.services";
import { ITinyUrl } from "../model/tinyurl";
import { IOriginalUrl } from "../model/originalurl";
import { UserConfig } from "../services/userconfig.service";

const httOptions = {
   headers: new HttpHeaders(
      {
         'Content-Type': 'application/json'
      }
   )
};

@Injectable({
    providedIn: 'root'
 })
 export class UrlService{
     /**
      *
      */
     constructor(private http:HttpClient,private login:UserConfig) { }

     public CreateUrl(originalurl:IOriginalUrl):Observable<ITinyUrl> {
        var url=AppConfig.settings.tinyUrl;
        return this.http.post<ITinyUrl>(url,JSON.stringify(originalurl),httOptions).pipe(catchError(this.handleError));
    }
    
    public GetUrls():Observable<ITinyUrl[]> {
       
      var url=AppConfig.settings.tinyUrl;
      return this.http.get<ITinyUrl[]>(url).pipe(catchError(this.handleError));
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