import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs';
import {map} from 'rxjs/operators';
import {JwtHelperService} from '@auth0/angular-jwt';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  baseUrl = environment.apiUrl + 'auth/';
  jwtHelper = new JwtHelperService();
  decodedToken;
  currentUser: User;
  photoUrl = new BehaviorSubject<string>('../../assets/user.png'); // default value
  currentPhotoUrl = this.photoUrl.asObservable();

  constructor(private http: HttpClient) { }

  changeMemberPhoto(photoUrl: string) {
    this.photoUrl.next(photoUrl);
  }

  login(model: any){
    return this.http.post(this.baseUrl + 'login', model).pipe(
      map((response: any) => {
        const user = response;
        this.decodedToken = this.jwtHelper.decodeToken(user.token);
        if (user){
          localStorage.setItem('token', user.token);
          localStorage.setItem('user', JSON.stringify(user.user));
          this.currentUser = user.user;
          this.changeMemberPhoto(this.currentUser.photoUrl); // here we actually get the actual photoUrl and call the method
          // to update the currentPhotoUrl, which we can subscribe to it as it is an observable
        }
      })
    );
  }

  register(user: User){
    return this.http.post(this.baseUrl + 'register', user);
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
