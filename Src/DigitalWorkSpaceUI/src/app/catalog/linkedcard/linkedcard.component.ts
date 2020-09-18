import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CardService } from '../../card/card.services';
import { DomSanitizer } from '@angular/platform-browser';
import { ICard } from '../../model/card';
import { AppConfig } from '../../services/app.config.services';

@Component({
  selector: 'app-linkedcard',
  templateUrl: './linkedcard.component.html',
  styleUrls: ['./linkedcard.component.scss']
})
export class LinkedcardComponent implements OnInit {

  @Input() card:ICard;
  @Output() unSelected :EventEmitter<any>=new EventEmitter<any>();
  @Output() selected :EventEmitter<any>=new EventEmitter<any>();
  public faviconstring:string;
  public imageData:any;
  public shortUrl:string;
  public sanitzedImageData:any;
  public isChecked:boolean;
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
      this.sanitzedImageData=this.card.favicon;
    }
    //this.faviconstring=this.card.originalUrl+'favicon.ico'
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
  changeListener() : void {
    if(this.isChecked)
    {
      this.isChecked=false;
      this.unSelected.emit(this.card);
    }
    else
    {
      this.isChecked=true;
      this.selected.emit(this.card);
    }

  }
}
