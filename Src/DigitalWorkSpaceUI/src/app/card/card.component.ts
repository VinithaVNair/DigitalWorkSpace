import { Component, OnInit,Input, Output  } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CardService } from './card.services';
import { ICard } from '../model/card';
import { DomSanitizer } from '@angular/platform-browser'; 
import { AppConfig } from '../services/app.config.services';


@Component({
  selector: 'app-card',
  templateUrl: './card.component.html',
  styleUrls: ['./card.component.scss']
})
export class CardComponent implements OnInit {

  @Input() card:ICard;
  @Input() editable:boolean;
  public faviconstring:string;
  public imageData:any;
  public shortUrl:string;
  public sanitzedImageData:any;
  constructor(private route: ActivatedRoute,private sanitizer: DomSanitizer,
    private router: Router, private cardService:CardService) { }

  ngOnInit(): void {
    this.getCard(this.card)
  }
  getCard(newCard:ICard)
  {
    this.card=newCard;
    this.shortUrl=AppConfig.settings.tinyUrl+'/'+ this.card.shortUrl;
    if(this.card.imageContent != null)
    {
    this.imageData = 'data:image/png;base64,' + this.card.imageContent;  
    this.sanitzedImageData = this.sanitizer.bypassSecurityTrustUrl(this.imageData);
    }
    else{
      this.sanitzedImageData=this.card.favicon
      // this.sanitzedImageData=this.card.originalUrl+'/favicon.ico'
    }
    //this.faviconstring=this.card.originalUrl+'favicon.ico'
  }
  edit()
  {
    this.router.navigate(['/edit'], { queryParams: { "id": this.card.id ,"version":this.card.version} });
  }
  navigate() : void {
    window.location.href=this.shortUrl;
  }

  extractHostname(url: string): string {
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

    return hostname;
}
}
