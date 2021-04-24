import { Component, Input, OnInit, ViewChild } from "@angular/core";
import { Router } from "@angular/router";
import { Utility } from "../../../core/utility/utility";
import { ModalDirective } from "ngx-bootstrap/modal";

@Component({
  selector: "app-one-page-header",
  templateUrl: "./one-page-header.component.html",
  styleUrls: ["./one-page-header.component.scss"],
})
export class OnePageHeaderComponent {

  @ViewChild('infoModal') public infoModal: ModalDirective;
  keyPassword: string;
  constructor(private route: Router, public utility: Utility) {}
  refreshPage(){
    window.location.reload();
  }
  toAddReport() {
    window.location.href = this.utility.serverWebRoot + "AddRecordPage";
  }
  toReport() {
    this.openModal();
  }
  toMaintain() {
    window.location.href = this.utility.serverWebRoot + "Maintain";
  }
  openModal(){
    this.keyPassword = "";
    this.infoModal.show();
  }
  closeModal(){
    this.infoModal.hide();
  }
  check(){
    this.infoModal.hide();
    if(this.keyPassword == this.utility.gaurdPassword || this.keyPassword == this.utility.admPassword){
      window.location.href = this.utility.serverWebRoot + "Report";
    }else{
      this.utility.alertify.confirm(
        "Sweet Alert",
        "Password Wrong, please try again.",
        () => {}
      );
    }
  }
}
