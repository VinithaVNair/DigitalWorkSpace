import { Component, OnInit } from '@angular/core';
import { ICard } from '../../model/card';
import { CardService } from '../card.services';

@Component({
  selector: 'app-card-list',
  templateUrl: './card-list.component.html',
  styleUrls: ['./card-list.component.scss']
})
export class CardListComponent implements OnInit {
public cards:ICard[];
  constructor(private cardService:CardService) { }

  ngOnInit(): void {
    this.cardService.GetCards().subscribe(data=>this.recievedCards(data));
  }

  recievedCards(allCards:ICard[])
  {
    this.cards=allCards;
  }
}
