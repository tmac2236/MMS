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
  modalName: string;

  refreshPage(){
    window.location.reload();
  }
  toAddReport() {
    window.location.href = this.utility.serverWebRoot + "AddRecordPage";
  }
  toReport() {
    this.keyPassword = "";
    this.modalName = "Report"
    this.openModal();
  }
  toMaintain() {
    this.keyPassword = "";
    this.modalName = "Maintain"
    this.openModal();
  }
  openModal(){
    this.infoModal.show();
  }
  closeModal(){
    this.infoModal.hide();
  }
  check(modalName :string){
    this.infoModal.hide();
    if(modalName == "Report" && (this.keyPassword == this.utility.gaurdPassword || this.keyPassword == this.utility.admPassword)) {
      var navigateTo = "/"+ this.modalName;  //dynamic name
      var navigationExtras = {
        queryParams: {},
        skipLocationChange: true,
      };
      this.route.navigate([navigateTo], navigationExtras);
    }else if ( modalName == "Maintain" && (this.keyPassword == this.utility.admPassword)){
      var navigateTo = "/"+ this.modalName;  //dynamic name
      var navigationExtras = {
        queryParams: {},
        skipLocationChange: true,
      };
      this.route.navigate([navigateTo], navigationExtras);
    }else{
      this.utility.alertify.confirm(
        "Sweet Alert",
        "Password Wrong, please try again.",
        () => {}
      );
    }
  }
  goToBottom(){
    window.scrollTo(0,document.body.scrollHeight);
  }

}
