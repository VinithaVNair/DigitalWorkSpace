import { Injectable } from "@angular/core";
import { IAppConfig } from "../model/appconfig";
import { HttpClient, HttpBackend } from "@angular/common/http";

@Injectable(
)
export class AppConfig{
    public static settings:IAppConfig
    private httpClient: HttpClient;
    constructor(private handler: HttpBackend) {
        this.httpClient = new HttpClient(handler);
        
    }
    initialize(){
        const jsonFile=`assets/config/config.json`;

        return new Promise<void>((resolve,reject)=>{this.httpClient.get(jsonFile).toPromise().then((response :IAppConfig)=>{
            AppConfig.settings=<IAppConfig>response;
            resolve();
        }).catch((response: any)=>{reject(`Could not load file '${jsonFile}':${JSON.stringify(response)}`);
    });
    });
}
}