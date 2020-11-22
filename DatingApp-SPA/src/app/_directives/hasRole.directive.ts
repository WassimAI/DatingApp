import { Directive, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { AuthService } from '../_services/auth.service';

@Directive({
  selector: '[appHasRole]'
})
export class HasRoleDirective implements OnInit {
  @Input() appHasRole: string[];
  isVisible = false;

  constructor(private viewContainerRef: ViewContainerRef, private templateRef: TemplateRef<any>,
              private authService: AuthService) { }


  ngOnInit() {
    const userRoles = this.authService.decodedToken.role as Array<string>;
    if (!userRoles) {
      this.viewContainerRef.clear(); // this clears the element that we are applying the directive to!
    }

    if (this.authService.roleMatch(this.appHasRole)) { // we get the role from the input object here & we match it with allowed roles
      if (!this.isVisible) {
        this.isVisible = true;
        this.viewContainerRef.createEmbeddedView(this.templateRef); // this applies to the element we are applying the directive to
      } else {
        this.isVisible = false;
        this.viewContainerRef.clear();
      }
    }
  }

}
