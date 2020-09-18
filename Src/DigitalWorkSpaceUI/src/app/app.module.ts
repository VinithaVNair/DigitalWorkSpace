import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import {FormsModule} from "@angular/forms";
import {HttpClientModule, HTTP_INTERCEPTORS} from'@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import {MatDatepickerModule} from '@angular/material/datepicker';

// import {MatInputModule} from '@angular/material/input';
import { NgbDate } from '@ng-bootstrap/ng-bootstrap';
import {NgbDateAdapter,NgbDateNativeAdapter,NgbModule} from '@ng-bootstrap/ng-bootstrap';
// import {MatNativeDateModule, NativeDateModule, MAT_DATE_FORMATS, MAT_NATIVE_DATE_FORMATS} from '@angular/material/core';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { CardComponent } from './card/card.component';
import { UrlComponent } from './url/url.component';
import { CatalogComponent } from './catalog/catalog.component';
import { HomeComponent } from './home/home.component';
import { APP_INITIALIZER } from '@angular/core';
import {AppConfig} from './services/app.config.services';
import { CardCreationComponent } from './card/card-creation/card-creation.component';
import { AuthButtonComponent } from './auth-button/auth-button.component';
import { CardListComponent } from './card/card-list/card-list.component';
import { CatalogCreationComponent } from './catalog/catalog-creation/catalog-creation.component';
import { CatalogListComponent } from './catalog/catalog-list/catalog-list.component';
import { CardEditComponent } from './card/card-edit/card-edit.component';
import { CardLinkComponent } from './catalog/card-link/card-link.component';
import { LinkedcardComponent } from './catalog/linkedcard/linkedcard.component';
import { CardUnlinkComponent } from './catalog/card-unlink/card-unlink.component';
import { ActionOnApprovalComponent } from './catalog/action-on-approval/action-on-approval.component';
import { AdminComponent } from './catalog/admin/admin.component';
import {MsalModule, MsalInterceptor} from '@azure/msal-angular';


export function initializeApp(appConfig: AppConfig) {
  return () => appConfig.initialize();
}

const isIE = window.navigator.userAgent.indexOf('MSIE ') > -1 || window.navigator.userAgent.indexOf('Trident/') > -1;
const GRAPH_ENDPOINT = 'https://graph.microsoft.com/v1.0/me';

@NgModule({
  declarations: [
    AppComponent,
    CardComponent,
    UrlComponent,
    CatalogComponent,
    HomeComponent,
    CardCreationComponent,
    AuthButtonComponent,
    CardListComponent,
    CatalogCreationComponent,
    CatalogListComponent,
    CardEditComponent,
    CardLinkComponent,
    LinkedcardComponent,
    CardUnlinkComponent,
    ActionOnApprovalComponent,
    AdminComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    MatDatepickerModule,
    // MatNativeDateModule,
    BrowserAnimationsModule,
    NgbModule,
    MsalModule.forRoot({
      auth: {
        clientId: 'e37343b7-bc8a-430b-bfe4-a06954a7e86c',
        authority: 'https://login.microsoftonline.com/common/',
        redirectUri: 'http://localhost:4200',
      },
      
    },
    {
      consentScopes: [
        'openid',
        'profile',
        'user.read'
      ],
      unprotectedResources: [],
      protectedResourceMap: [
        ['https://graph.microsoft.com/v1.0/', ['user.read']],
        ['https://localhost:5010/api/*', ['user.read']],
    ],
      extraQueryParameters: {}
    })
  ],
  entryComponents:[UrlComponent],
  providers: [
      // { provide: HTTP_INTERCEPTORS,
      //   useClass: AuthInterceptorService,
      //   multi: true }
        // {
        //     provide: HTTP_INTERCEPTORS,
        //     useClass: MsalInterceptor,
        //     multi: true
        // },
        AppConfig,
    { provide: APP_INITIALIZER,
      useFactory: initializeApp,
      deps: [AppConfig], multi: true }
    ],
        
    
  bootstrap: [AppComponent]
})
export class AppModule { }
