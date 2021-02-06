import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/_models/user';
import { AdminService } from 'src/app/_services/admin.service';

@Component({
  selector: 'app-photo-management',
  templateUrl: './photo-management.component.html',
  styleUrls: ['./photo-management.component.css']
})
export class PhotoManagementComponent implements OnInit {

  users: User[];
  constructor(private adminService: AdminService) { }

  ngOnInit(): void {
    this.getUsersWithUnApprovedPhotos();
    console.log('Users Are: ' + this.users);
  }

  getUsersWithUnApprovedPhotos() {
    this.adminService.getUnApprovedPhotos().subscribe((data: User[]) => {
      this.users = data;
    }, error => {
      console.log(error);
    });
  }

  approvePhoto(id: number) {
    this.adminService.approvePhoto(id).subscribe(next => {
      console.log('Approved!');
    }, error => {
      console.log(error);
    });
  }

}
