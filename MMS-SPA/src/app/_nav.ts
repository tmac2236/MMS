import { Injectable } from '@angular/core';
import { INavData } from '@coreui/angular';
import { AuthService } from './core/_services/auth.service';

export const navItems: INavData[] = [];

@Injectable({
  providedIn: 'root'  // <- ADD THIS
})
export class NavItem {

  navItems: INavData[] = [];
  theUserRoles = this.authService.getUserRole().toUpperCase();
  constructor(private authService: AuthService){}

  getNav() {
    //grandFather
    this.navItems = [];

    //#region "1navReport"
    //father
    const navReport = {
      name: '1. Report',
      icon: 'fa fa-newspaper-o',
      children: []
    };
    //children
    const navReport_Report = {
      name: '1.1 Report',
      url: '/Report/Report',
      class: 'menu-margin',
    };

    //children -> father
    //if (this.theUserRoles.includes("ADM") || this.theUserRoles.includes("GA")|| this.theUserRoles.includes("GUARD")) {
      navReport.children.push(navReport_Report);
    //}

    //father -> grandfather
    if (navReport.children.length > 0) {
      this.navItems.push(navReport);
    }
    //#endregion  "navReport"

    return this.navItems;
  }
}
