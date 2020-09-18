import { Component, OnInit } from '@angular/core';
import { BroadcastService, MsalService } from '@azure/msal-angular';
import { CryptoUtils, Logger } from 'msal';
import { UserConfig } from '../services/userconfig.service';

@Component({
  selector: 'app-auth-button',
  templateUrl: './auth-button.component.html',
  styleUrls: ['./auth-button.component.scss']
})
export class AuthButtonComponent implements OnInit {
  title = 'DigitalWorkSpaceUI';
  isIframe = false;
  loggedIn = false;

  constructor(private broadcastService: BroadcastService, private authService: MsalService,private userConfig:UserConfig) { }

  ngOnInit() {
    this.isIframe = window !== window.parent && !window.opener;

    this.checkAccount();

    this.broadcastService.subscribe('msal:loginSuccess', () => {
      this.checkAccount();
    });
    this.broadcastService.subscribe("msal:loginFailure", payload => {
      this.loggedIn=false;
      payload.checkAccount();
  });
    this.authService.handleRedirectCallback((authError, response) => {
      if (authError) {
        console.error('Redirect Error: ', authError.errorMessage);
        return;
      }

      console.log('Redirect Success: ', response.accessToken);
    });

    this.authService.setLogger(new Logger((logLevel, message, piiEnabled) => {
      console.log('MSAL Logging: ', message);
    }, {
      correlationId: CryptoUtils.createNewGuid(),
      piiLoggingEnabled: false
    }));

    this.broadcastService.subscribe("msal:acquireTokenSuccess", payload => {
      // do something here
  });
   
  this.broadcastService.subscribe("msal:acquireTokenFailure", payload => {
      // do something here
  });
  }

  checkAccount() {
    this.loggedIn = !!this.authService.getAccount();
    if(this.loggedIn)
    {
      this.userConfig.initialize();
      //this.getAuthToken()
    }
  }

  login() {
    const isIE = window.navigator.userAgent.indexOf('MSIE ') > -1 || window.navigator.userAgent.indexOf('Trident/') > -1;
    //this.authServiceMine.login();
    if (isIE) {
      this.authService.loginRedirect();
    } else {
      this.authService.loginPopup();
    }
  }

  logout() {
    
    this.authService.logout();
    this.userConfig.logout()
  };
// getAuthToken()
// {
//   const requestObj = {
//     scopes: ["user.read"]
// };

// this.authService.acquireTokenSilent(requestObj).then(function (tokenResponse) {
//     // Callback code here
//     console.log(tokenResponse.accessToken);
// }).catch(function (error) {
//     console.log(error);
// });
// }
  
}