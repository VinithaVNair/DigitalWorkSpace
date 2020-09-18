import { Component, OnInit } from '@angular/core';
import { ICard } from '../../model/card';
import { CardService } from '../../card/card.services';
import { CatalogService } from '../catalog.services';
import { UserConfig } from "../../services/userconfig.service";
import { ICatalogCreation } from '../../model/catalogCreation';
import { ICatalog } from '../../model/catalog';
import { ICardLinkingInfo } from '../../model/cardLinkingInfo';
import { IUser } from '../../model/user';

@Component({
  selector: 'app-catalog-creation',
  templateUrl: './catalog-creation.component.html',
  styleUrls: ['./catalog-creation.component.scss']
})
export class CatalogCreationComponent implements OnInit {
public name:string;
public selectedCard:number;
public catalogId:number;
public cards:ICard[];
public cardTobeLinked:ICardLinkingInfo
public linkeCard:number;
public createdCatalog:ICatalog;
public message:string;

private catalogCreation:ICatalogCreation;
  constructor( private login:UserConfig,private cardService:CardService, 
    private catalogService:CatalogService) { }

  ngOnInit(): void {
    this.cardService.GetCards().subscribe(data=>this.cards=data);
  }

  create()
  {
    let user:IUser=this.login.getUser();
    this.catalogCreation={
      'catalogName':this.name,
      'userID':user.userId
    };
    this.catalogService.CreateCatalog(this.catalogCreation).subscribe(data=>this.catalogCreated(data));
  }

  catalogCreated(catalog:ICatalog)
  {
    this.createdCatalog=catalog;
    this.catalogId=catalog.id;
    this.message="Catalog created successfully"
  }
}
