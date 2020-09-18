import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CardComponent } from './card/card.component';
import { UrlComponent } from './url/url.component';
import { CatalogComponent } from './catalog/catalog.component';
import { HomeComponent } from './home/home.component';
import { CardCreationComponent } from './card/card-creation/card-creation.component';
import { CatalogCreationComponent } from './catalog/catalog-creation/catalog-creation.component';
import { CardListComponent } from './card/card-list/card-list.component';
import { CardEditComponent } from './card/card-edit/card-edit.component';
import { CatalogListComponent } from './catalog/catalog-list/catalog-list.component';
import { CardLinkComponent } from './catalog/card-link/card-link.component';
import { CardUnlinkComponent } from './catalog/card-unlink/card-unlink.component';
import { ActionOnApprovalComponent } from './catalog/action-on-approval/action-on-approval.component';
import { AdminComponent } from './catalog/admin/admin.component';

export const routes: Routes = [
  {path: 'home' , component:HomeComponent},
  {path: 'urls' , component:UrlComponent},
  {path: 'cardcreation' , component:CardCreationComponent},
  {path: 'cards' , component:CardListComponent},
  {path: 'cards/:id' , component:CardComponent},
  {path: 'cards/edit' , component:CardEditComponent},
  {path: 'edit' , component:CardEditComponent},
  {path: 'catalogcreation' , component:CatalogCreationComponent},
  {path: 'catalog' , component:CatalogComponent},
  {path: 'link', component:CardLinkComponent},
  {path: 'unlink', component:CardUnlinkComponent},
  {path: 'action', component:ActionOnApprovalComponent},
  {path: 'admin', component:AdminComponent},
  {path: 'catalogs' , component:CatalogListComponent},
  {path: '' , redirectTo:'home',pathMatch:'full'},
  {path: '**' , redirectTo:'home'}
  
];


@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
