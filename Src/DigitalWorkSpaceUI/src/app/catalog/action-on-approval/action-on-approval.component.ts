import { Component, OnInit } from '@angular/core';
import { CatalogService } from '../catalog.services';
import { ActivatedRoute } from '@angular/router';
import { CardService } from '../../card/card.services';
import { UserConfig } from '../../services/userconfig.service';
import { ILinkedCards } from '../../model/linkedCards';
import { ICard } from '../../model/card';
import { ICardLinkingInfo } from '../../model/cardLinkingInfo';

@Component({
  selector: 'app-action-on-approval',
  templateUrl: './action-on-approval.component.html',
  styleUrls: ['./action-on-approval.component.scss']
})
export class ActionOnApprovalComponent implements OnInit {

  public catalogId:number;
 constructor(private catalogService:CatalogService,private route: ActivatedRoute,private cardService:CardService,
   private userConfig:UserConfig) { }
public linkedCards:ILinkedCards[];
public cards:ICard[]=[];
public cardsTemp:ICard[]=[];
public cardsSelected:ICardLinkingInfo[]=[];
public delted:boolean;
 ngOnInit(): void { 
  this.route.queryParams.subscribe(params => {
    this.catalogId = params['catalogId'];
  });
  this.catalogService.GetCardsPendingApproval(this.catalogId).subscribe(data=>this.recievedCards(data));
}

recievedCards(allcards:ILinkedCards[])
{
  this.linkedCards=allcards;
  allcards.forEach(element => {
     this.cardService.GetCardSync(element.id,element.version).then(data=>this.appendData(data))
  });
  
}
appendData(card:ICard[])
{
  this.cards.push(card[0]);
}
 selected(card:ICard)
{
  var cardTobeLinked:ICardLinkingInfo={
    'cardId':card.id,
    'cardVersion':card.version,
    'userId':this.userConfig.getUserId()
  }
  this.cardsSelected.push(cardTobeLinked);
}
unSelected(card:ICard)
{
  
  var index=this.cardsSelected.indexOf(this.cardsSelected.find(v=>v.cardId==card.id && v.cardVersion==card.version),0);
  this.cardsSelected.splice(index,1);
}

approve()
{
  this.catalogService.EditCards(this.cardsSelected,this.catalogId).subscribe(data=>this.onSuccess(data))

}
reject()
{
  this.catalogService.RejectCards(this.cardsSelected,this.catalogId).subscribe(data=>this.onSuccess(data))

}

onSuccess(data:any)
{
  this.ngOnInit();
}

}
