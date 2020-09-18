import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { CardService } from '../../card/card.services';
import { CatalogService } from '../catalog.services';
import { ICatalog } from '../../model/catalog';

@Component({
  selector: 'app-catalog-list',
  templateUrl: './catalog-list.component.html',
  styleUrls: ['./catalog-list.component.scss']
})
export class CatalogListComponent implements OnInit {

  constructor(private catalogService:CatalogService,private route: ActivatedRoute,
    private router: Router) { }
public catalogs:ICatalog[];
  ngOnInit(): void { 
    // let id = +this.route.snapshot.paramMap.get('id');
    this.catalogService.GetCatalogs().subscribe(data=>this.recievedCards(data));
  }

  recievedCards(allCatalogs:ICatalog[])
  {
    this.catalogs=allCatalogs;
  }
navigate(catalog:ICatalog)
{
  this.router.navigate(['/catalog'], { queryParams: { "id": catalog.id} });
  
}
}
