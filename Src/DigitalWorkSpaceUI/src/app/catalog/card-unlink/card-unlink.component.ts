import { Component, OnInit } from '@angular/core';
import { UserConfig } from '../../services/userconfig.service';
import { ILinkedCards } from '../../model/linkedCards';
import { ICard } from '../../model/card';
import { ICardLinkingInfo } from '../../model/cardLinkingInfo';
import { ActivatedRoute } from '@angular/router';
import { CatalogService } from '../catalog.services';
import { HttpParams } from '@angular/common/http';
import { CardService } from '../../card/card.services';

@Component({
  selector: 'app-card-unlink',
  templateUrl: './card-unlink.component.html',
  styleUrls: ['./card-unlink.component.scss']
})
export class CardUnlinkComponent implements OnInit {

  public catalogId:number;
 constructor(private catalogService:CatalogService,private route: ActivatedRoute,private cardService:CardService,
   private userConfig:UserConfig) { }
public linkedCards:ILinkedCards[];
public cards:ICard[]=[];
public cardsTemp:ICard[]=[];
public cardsSelected:ICardLinkingInfo[]=[];
public delted:boolean;
public message:string;
 ngOnInit(): void { 
  this.route.queryParams.subscribe(params => {
    this.catalogId = params['catalogId'];
  });
  this.catalogService.GetLinkedCards(this.catalogId).subscribe(data=>this.recievedCards(data));
}

recievedCards(allcards:ILinkedCards[])
{
  this.linkedCards=allcards;
  let parms=new HttpParams();
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

removeCards()
{
  this.catalogService.DeleteCards(this.cardsSelected,this.catalogId).subscribe(data=>this.OnCardRemoved(data))

}
OnCardRemoved(sucess:boolean)
{
  this.message="Cards removed successfully"
}
}
