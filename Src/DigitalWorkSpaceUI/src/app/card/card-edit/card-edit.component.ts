import { Component, OnInit, Input } from '@angular/core';
import { ICardCreation } from '../../model/cardCreation';
import { ICard } from '../../model/card';
import { ITinyUrl } from '../../model/tinyurl';
import { CardService } from '../card.services';
import { Router, ActivatedRoute } from '@angular/router';
import { IEditedCard } from '../../model/editedCard';

@Component({
  selector: 'app-card-edit',
  templateUrl: './card-edit.component.html',
  styleUrls: ['./card-edit.component.scss']
})
export class CardEditComponent implements OnInit {

  constructor(private cardService:CardService,private route: ActivatedRoute,
    private router: Router) { }

 
  public title:string;
  public description:string;
  public files:any;
  public selectedUrl:string;
  public image:any;
  public cardEditInput:IEditedCard;
  public editId:number;
  public editVesion:number;
  public expiry:Date;
  public newCard:ICard;
  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.editId = params['id'];
      this.editVesion=params['version'];
    });
    this.cardService.GetCard(this.editId,this.editVesion).subscribe(data=>this.populateData(data));
    // this.pageTitle += `: ${id}`;
    
  }
  save()
  {
    this.cardEditInput ={
      'id':this.editId,
      'version':this.editVesion,
      'imageContent': this.image?this.image.replace('data:image/png;base64,',''):null,
      'title':this.title,
      'description':this.description
    };
    this.cardService.EditCard(this.cardEditInput).subscribe(data=>this.onEditSuccessfull(data));
  }

  populateData(cards:ICard[])
  {
    let card=cards[0];
    this.editId=card.id;
    this.editVesion=card.version;
    this.title=card.title;
    this.description=card.description;
    // if(card.imageContent != null)
    // {
    // this.image = 'data:image/png;base64,' + this.card.imageContent;  
    // this.sanitzedImageData = this.sanitizer.bypassSecurityTrustUrl(this.imageData);
    // }
    // else{
    //   this.sanitzedImageData=this.card.originalUrl+'/favicon.ico'
    // }
  }
  onEditSuccessfull(editCard:ICard)
  {
    this.newCard=editCard;
  }

  changeListener($event) : void {
    this.readThis($event.target);
  }
  
  readThis(inputValue: any): void {
    var file:File = inputValue.files[0];
    var myReader:FileReader = new FileReader();
  
    myReader.onloadend = (e) => {
      this.image = myReader.result;
    }
    myReader.readAsDataURL(file);
  }
}
