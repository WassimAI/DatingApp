import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {map} from 'rxjs/operators';
import {JwtHelperService} from '@auth0/angular-jwt';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  baseUrl = 'http://localhost:5000/api/auth/';
  jwtHelper = new JwtHelperService();
  decodedToken;

  constructor(private http: HttpClient) { }

  login(model: any){
    return this.http.post(this.baseUrl + 'login', model).pipe(
      map((response: any) => {
        const user = response;
        this.decodedToken = this.jwtHelper.decodeToken(user.token);
        if (user){
          localStorage.setItem('token', user.token);
        }
      })
    );
  }

  register(model: any){
    return this.http.post(this.baseUrl + 'register', model);
  }

  loggedIn(){
    const token = localStorage.getItem('token');
    return !this.jwtHelper.isTokenExpired(token); // If it is not expired it will return true, and if expired it will return false
  }

  // getUserName(){
  //   const token = localStorage.getItem('token');
  //   return this.jwtHelper.decodeToken(token).unique_name;
  // }
}
