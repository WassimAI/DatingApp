import { Component, OnInit, Input } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model: any = {};

  constructor(public authService: AuthService, private alertify: AlertifyService, private router: Router) { }

  ngOnInit() {
  }

  login(){
    // console.log(this.model);
    this.authService.login(this.model).subscribe(next => {
      this.alertify.success('logged in successfully');
    }, error => {
      this.alertify.error(error);
    }, () => { // the anonymous function () is actually called complete function, when the call is completed!
      this.router.navigate(['/members']);
    });
  }

  loggedIn(){
    return this.authService.loggedIn(); // this will return after checking if it is expired and if it is a valid token!
  }

  logout(){
    localStorage.removeItem('token');
    this.alertify.message('Logout');
    this.router.navigate(['/home']);
  }

}
