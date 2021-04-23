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
    window.location.href = this.utility.serverWebRoot+"AddRecordPage";   
  }
  toReport() {
    window.location.href = this.utility.serverWebRoot+"Report";  
  }
  toMaintain() {
    window.location.href = this.utility.serverWebRoot+"Maintain";  
  }
}
