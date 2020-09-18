import { Component, OnInit, Input } from '@angular/core';
import { CatalogService } from '../catalog.services';
import { ActivatedRoute, Router } from '@angular/router';
import { CardService } from '../../card/card.services';
import { ICard } from '../../model/card';
import { ILinkedCards } from '../../model/linkedCards';
import { ICardLinkingInfo } from '../../model/cardLinkingInfo';
import { UserConfig } from '../../services/userconfig.service';

@Component({
  selector: 'app-card-link',
  templateUrl: './card-link.component.html',
  styleUrls: ['./card-link.component.scss']
})
export class CardLinkComponent implements OnInit {

 public catalogId:number;
 constructor(private catalogService:CatalogService,private route: ActivatedRoute,private cardService:CardService,
   private userConfig:UserConfig) { }
public linkedCards:ILinkedCards[];
public cards:ICard[]=[];
public cardsTemp:ICard[]=[];
public cardsSelected:ICardLinkingInfo[]=[];
public message:string;
 ngOnInit(): void { 
  this.route.queryParams.subscribe(params => {
    this.catalogId = params['catalogId'];
  });
     this.cardService.GetCards().subscribe(data=>this.cards=data);
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

addCards()
{
  this.catalogService.LinkCards(this.cardsSelected,this.catalogId).subscribe(data=>this.OnCardAdded(data))

}
OnCardAdded(card:ILinkedCards[])
{
  this.message="Cards added successfully"
}
}
