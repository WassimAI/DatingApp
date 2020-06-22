import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  //output prop is emitting an event from the child (register) to the parent (home) while input prop comes from parent to child here as input
  @Output() cancelRegister = new EventEmitter();
  model: any = {};

  constructor(private authService: AuthService, private alertify: AlertifyService) { }

  ngOnInit() {
  }

  register(){
    this.authService.register(this.model).subscribe(()=>{
      this.alertify.success('registration successful');
    }, error => {
      this.alertify.error(error);
    });
  }

  cancel(){
    this.cancelRegister.emit(false);
    console.log('cancelled');
  }

}
