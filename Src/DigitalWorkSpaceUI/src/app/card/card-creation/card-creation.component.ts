import { Component, OnInit, EventEmitter  } from '@angular/core';
import { CardService } from '../card.services';
import { UrlService } from '../../url/url.services';
import { ITinyUrl } from '../../model/tinyurl';
import { ICard } from '../../model/card';
import { ICardCreation } from '../../model/cardCreation';

@Component({
  selector: 'app-card-creation',
  templateUrl: './card-creation.component.html',
  styleUrls: ['./card-creation.component.scss']
})
export class CardCreationComponent implements OnInit {
public urls:ITinyUrl[];
public title:string;
public description:string;
public files:any;
public selectedUrl:string;
public image:any;
public cardCreationInput:ICardCreation;
public card:ICard;
public id:number;
  constructor(private cardService:CardService,private urlService:UrlService) { }

  ngOnInit(): void {
    this.urlService.GetUrls().subscribe(data=>this.urls=data)
  }
 

  save()
  {
    let tinyUrlSelecte=this.urls.find(v=>v.shortUrl==this.selectedUrl);
    this.cardCreationInput ={
      
      'imageContent': this.image?this.image.replace('data:image/png;base64,',''):null,
      'title':this.title,
      'shortUrl':this.selectedUrl,
      'favicon':this.extractFavicon(tinyUrlSelecte.originalUrl),
      'expiry':tinyUrlSelecte.expiry,
      'description':this.description
    };
    this.cardService.CreateCard(this.cardCreationInput).subscribe(data=>this.checkCreation(data));
  }

  extractFavicon(url: string): string {
    let hostname;

    // remove protocol
    if (url.indexOf('//') > -1) {
        hostname = url.split('/')[2];
    } else {
        hostname = url.split('/')[0];
    }

    // remove port
    hostname = hostname.split(':')[0];

    // remove query
    hostname = hostname.split('?')[0];

    return 'https://'+hostname+'/favicon.ico';
}

  checkCreation(newCardu:ICard)
  {
    this.card=newCardu;
    this.id=newCardu.id;
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
