import { Injectable } from "@angular/core";
import { HttpClient, HttpHeaders,HttpErrorResponse, HttpParams } from "@angular/common/http";
import { Observable, throwError } from "rxjs";
import { catchError } from "rxjs/operators";
import { AppConfig } from "../services/app.config.services";
import { ICard } from "../model/card";
import { ICardCreation } from "../model/cardCreation";
import { IEditedCard } from "../model/editedCard";

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

 export class CardService {
    /**
     *
     */
    constructor(private http:HttpClient) { }

    public GetCard(id:number,version:number):Observable<ICard[]> {
       var url=AppConfig.settings.cards+'/'+id  ;
       var params=new HttpParams().set("version",version.toString())
       return this.http.get<ICard[]>(url,{params}).pipe(catchError(this.handleError));
   }
   public  GetCardSync(id:number,version:number):Promise<ICard[]> {
      var url=AppConfig.settings.cards+'/'+id ;
      var params=new HttpParams().set("version",version.toString())
      return this.http.get<ICard[]>(url,{params}).pipe(catchError(this.handleError)).toPromise();
  }
   public GetLinkedCards(id:Record<number,number>):Observable<ICard> {
      var url=AppConfig.settings.cards+'/'+id ;
      return this.http.get<ICard>(url).pipe(catchError(this.handleError));
  }

   public GetCards():Observable<ICard[]> {
      var url=AppConfig.settings.cards
      return this.http.get<ICard[]>(url).pipe(catchError(this.handleError));
  }
   public CreateCard(cardCreation:ICardCreation):Observable<ICard> {
      var url=AppConfig.settings.cards;
      return this.http.post<ICard>(url,JSON.stringify(cardCreation),httOptions).pipe(catchError(this.handleError));
  }
  public EditCard(editedCard:IEditedCard):Observable<ICard> {
   var url=AppConfig.settings.cards;
   return this.http.patch<ICard>(url,JSON.stringify(editedCard),httOptions).pipe(catchError(this.handleError));
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