import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { CatalogService } from './catalog.services';
import { ILinkedCards } from '../model/linkedCards';
import { CardService } from '../card/card.services';
import { HttpParams } from '@angular/common/http';
import { ICard } from '../model/card';
import { IAdmin } from '../model/admin';
import { UserConfig } from '../services/userconfig.service';

@Component({
  selector: 'app-catalog',
  templateUrl: './catalog.component.html',
  styleUrls: ['./catalog.component.scss']
})
export class CatalogComponent implements OnInit {
 // @Input() card:ICard;
 public catalogId:number;
  constructor(private catalogService:CatalogService,private route: ActivatedRoute,private cardService:CardService,
    private router: Router,private userConfig:UserConfig) { }
public linkedCards:ILinkedCards[];
public cards:ICard[]=[];
public cardsTemp:ICard[]=[];
public delete:boolean;
public cardsPendingApproval:ILinkedCards[]=[];
public isAdmin:boolean;
  ngOnInit(): void { 
    this.route.queryParams.subscribe(params => {
      this.catalogId = params['id'];
    });
    this.catalogService.GetCardsPendingApproval(this.catalogId).subscribe(data=>this.cardsPendingApproval=data);
    this.catalogService.GetLinkedCards(this.catalogId).subscribe(data=>this.recievedCards(data));
  }

  recievedCards(allcards:ILinkedCards[])
  {
    this.linkedCards=allcards;
    let parms=new HttpParams();
    allcards.forEach(element => {
       this.cardService.GetCardSync(element.id,element.version).then(data=>this.appendData(data))
    });
    this.catalogService.GetAllAdmin(this.catalogId).subscribe(data=>this.CheckifAdmin(data));
  }

  CheckifAdmin(admin:IAdmin[])
  {
    let user=admin.find(f=>f.id==this.userConfig.getUserId())
    if(user)
    {
      this.isAdmin=true;
    }
    else
    {
      this.isAdmin=false
    }

  }
  appendData(card:ICard[])
  {
    this.cards.push(card[0]);
  }
  addCards()
  {
    //get all cards route to link card list without input
    this.router.navigate(['/link'], { queryParams: { "catalogId": this.catalogId}}); 
    //for delte we ll pass cards in input 
  }
  removeCards()
  {
    this.router.navigate(['/unlink'], { queryParams: { "catalogId": this.catalogId}}); 
    
  }
  actionOnCards()
  {
    this.router.navigate(['/action'], { queryParams: { "catalogId": this.catalogId}}); 
    
  }
  addAdmin()
  {
    this.router.navigate(['/admin'], { queryParams: { "catalogId": this.catalogId,"action": "add"}}); 
    
  }
  removeAdmin()
  {
    this.router.navigate(['/admin'], { queryParams: { "catalogId": this.catalogId,"action": "remove"}}); 
    
  }

 
}
