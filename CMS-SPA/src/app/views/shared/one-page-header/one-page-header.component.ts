import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { Utility } from "../../../core/utility/utility";

@Component({
  selector: "app-one-page-header",
  templateUrl: "./one-page-header.component.html",
  styleUrls: ["./one-page-header.component.scss"],
})
export class OnePageHeaderComponent {
  constructor( private route: Router,public utility: Utility) {}

  toAddReport(){
    var navigateTo = "/AddRecordPage";
    this.route.navigate([navigateTo]);   
  }
  toReport() {
    var navigateTo = "/Report";
    this.route.navigate([navigateTo]);
  }
  toMaintain() {
    var navigateTo = "/Maintain";
    this.route.navigate([navigateTo]);
  }
}
