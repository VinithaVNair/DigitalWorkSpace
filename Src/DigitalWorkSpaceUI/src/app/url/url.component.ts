import { Component, OnInit, Input, Injectable } from '@angular/core';
import { UrlService } from './url.services';
import { ActivatedRoute } from '@angular/router';
import { IOriginalUrl } from '../model/originalurl';
import { ITinyUrl } from '../model/tinyurl';
import { UserConfig } from "../services/userconfig.service";
import {FormControl} from '@angular/forms';
import { NgbDate, NgbModule, NgbDateAdapter } from '@ng-bootstrap/ng-bootstrap';
import {NgbDateStruct} from '@ng-bootstrap/ng-bootstrap';
import { AppConfig } from '../services/app.config.services';

@Injectable()
export class NgbStringAdapter extends NgbDateAdapter<Date> {

  fromModel(date: Date): NgbDateStruct {
    return date ? {
      year: date.getFullYear(),
      month: date.getMonth() + 1,
      day: date.getDate()
    } : null;
  }

  toModel(date: NgbDateStruct): Date {
    return date ? new Date(date.year, date.month - 1, date.day) : null;
  }
}
@Component({
  selector: 'app-url',
  templateUrl: './url.component.html',
  styleUrls: ['./url.component.scss'],
  providers: [{provide: NgbDateAdapter, useClass: NgbStringAdapter}]
})
export class UrlComponent implements OnInit {

  constructor(private urlservice:UrlService, private route: ActivatedRoute) { }
  public  url:string="";
  public expiry :Date;
  public tinyUrl:ITinyUrl;
  public input:IOriginalUrl;
  public label :string ="Choose Date";
  public tempdate:Date;
  public shortUrl:string;
  date = new FormControl(new Date());
  ngOnInit(): void {
  }

  CreateUrl()

  {
    console.log("date :"+this.expiry)
    this.input={
      'originalUrl':this.url,
      'expiry':this.expiry
    }
   this.urlservice.CreateUrl(this.input).subscribe(data=>this.onUrlCreated(data) );
  }
  
  onUrlCreated(data:ITinyUrl)
  {
    this.tinyUrl=data;
    this.shortUrl=AppConfig.settings.tinyUrl+'/'+this.tinyUrl.shortUrl
  }
}


