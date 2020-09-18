import { Injectable } from "@angular/core";
import { HttpClient, HttpHeaders,HttpErrorResponse } from "@angular/common/http";
import { Observable, throwError, from } from "rxjs";
import { catchError } from "rxjs/operators";
import { AppConfig } from "../services/app.config.services";
import { UserConfig } from "../services/userconfig.service";
import { ICatalogCreation } from "../model/catalogCreation";
import { ILinkedCards } from "../model/linkedCards";
import { ICatalog } from "../model/catalog";
import { ICardLinkingInfo } from "../model/cardLinkingInfo";
import { ICard } from "../model/card";
import { IAdmin } from "../model/admin";
import { INewAdminRequest } from "../model/newAdmin";
import { MsalService } from "@azure/msal-angular";

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
 export class  CatalogService{
     /**
      *
      */
     constructor(private http:HttpClient,private login:UserConfig,private authService: MsalService) { }

     public CreateCatalog(catalogToBeCreated:ICatalogCreation):Observable<ICatalog> {
        var url=AppConfig.settings.catalogs;
        return this.http.post<ICatalog>(url,JSON.stringify(catalogToBeCreated),httOptions).pipe(catchError(this.handleError));
    }

    public LinkCard(cardTobeLinked:ICardLinkingInfo,catalogId:number):Observable<ICard> {
        var url=AppConfig.settings.catalogs+'/'+catalogId+'/cards';
        return this.http.post<ICard>(url,JSON.stringify(cardTobeLinked),httOptions).pipe(catchError(this.handleError));
    }
    public LinkCards(cardsTobeLinked:ICardLinkingInfo[],catalogId:number):Observable<ILinkedCards[]> {
      var url=AppConfig.settings.catalogs+'/'+catalogId+'/cards';
      return this.http.post<ILinkedCards[]>(url,JSON.stringify(cardsTobeLinked),httOptions).pipe(catchError(this.handleError));
  }
  public EditCards(cardsTobeLinked:ICardLinkingInfo[],catalogId:number):Observable<boolean> {
   var url=AppConfig.settings.catalogs+'/'+catalogId+'/cards';
   return this.http.put<boolean>(url,JSON.stringify(cardsTobeLinked),httOptions).pipe(catchError(this.handleError));
   }

   public RejectCards(cardsRejected:ICardLinkingInfo[],catalogId:number):Observable<boolean> {
      var url=AppConfig.settings.catalogs+'/'+catalogId+'/reject';
      const httOptionsDelete = {
         headers: new HttpHeaders(
            {
               'Content-Type': 'application/json'
            }
         ),
         body:JSON.stringify(cardsRejected)
      };
      return this.http.delete<boolean>(url,httOptionsDelete).pipe(catchError(this.handleError));
   }

  public DeleteCards(cardsTobeLinked:ICardLinkingInfo[],catalogId:number):Observable<boolean> {
   var url=AppConfig.settings.catalogs+'/'+catalogId+'/cards';
   const httOptionsDelete = {
      headers: new HttpHeaders(
         {
            'Content-Type': 'application/json'
         }
      ),
      body:JSON.stringify(cardsTobeLinked)
   };
   return this.http.delete<boolean>(url,httOptionsDelete).pipe(catchError(this.handleError));
}
     public GetLinkedCards(catalogId:number):Observable<ILinkedCards[]> {
        var url=AppConfig.settings.catalogs+'/'+catalogId+'/cards';
        return this.http.get<ILinkedCards[]>(url).pipe(catchError(this.handleError));
    }

    public GetCardsPendingApproval(catalogId:number):Observable<ILinkedCards[]> {
      var url=AppConfig.settings.catalogs+'/'+catalogId+'/pendingapproval';
      return this.http.get<ILinkedCards[]>(url).pipe(catchError(this.handleError));
  }

    public GetCatalogs():Observable<ICatalog[]> {
      var url=AppConfig.settings.catalogs ;
      return this.http.get<ICatalog[]>(url).pipe(catchError(this.handleError));
  }

  GetAllAdmin(catalogId:number):Observable<IAdmin[]>{
     var url=AppConfig.settings.catalogs +'/'+catalogId+'/admins';
     return this.http.get<IAdmin[]>(url).pipe(catchError(this.handleError));
  }
  addAdmin(catalogId:number,newAdminReq:INewAdminRequest):Observable<IAdmin>{
   var url=AppConfig.settings.catalogs +'/'+catalogId+'/admins';
   return this.http.post<IAdmin>(url,JSON.stringify(newAdminReq),httOptions).pipe(catchError(this.handleError));
}
removeAdmin(catalogId:number,userId:number,adminid:number):Observable<IAdmin>{
   var url=AppConfig.settings.catalogs +'/'+catalogId+'/admins';
   const httOptionsDelete = {
      headers: new HttpHeaders(
         {
            'Content-Type': 'application/json'
         }
      ),
      body:{'userId':userId,
   'adminId' :adminid}
   };
   return this.http.delete<IAdmin>(url,httOptionsDelete).pipe(catchError(this.handleError));
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