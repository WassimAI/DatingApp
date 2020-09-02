import { Component, OnInit, Input } from '@angular/core';
import { Message } from 'src/app/_models/message';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { AuthService } from 'src/app/_services/auth.service';
import { tap } from 'rxjs/operators';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {

  @Input() recipientId: number;
  messages: Message[];
  newMessage: any = {};

  constructor(private userService: UserService, private auth: AuthService, private alertify: AlertifyService) { }

  ngOnInit(): void {
    this.loadMessages();
  }

  loadMessages() {
    const currentUserId = +this.auth.decodedToken.nameid;
    this.userService.getMessageThread(this.auth.decodedToken.nameid, this.recipientId)
    .pipe(
      tap(messages => {
        for(let i=0; i < messages.length; i++) {
          if(messages[i].isRead === false && messages[i].recipientId === currentUserId) {
            this.userService.markAsRead(currentUserId, messages[i].id);
          }
        }
      })
    )
    .subscribe((data) => {
      this.messages = data;
    }, error => {
      this.alertify.error(error);
    });
  }

  sendMessage() {
    this.newMessage.recipientId = this.recipientId;
    this.userService.sendMessage(this.auth.decodedToken.nameid, this.newMessage).subscribe((message: Message) => {
      // debugger; // to add a break point!!
      this.messages.unshift(message); // to add the new item to the start rather than end!
      this.newMessage.content = '';
    }, error => {
      this.alertify.error(error);
    });
  }

}
