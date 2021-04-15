import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { Utility } from "../../core/utility/utility";
import { CarManageRecord } from "../../core/_models/car-manage-record";

@Component({
  selector: "app-add-record-page",
  templateUrl: "./add-record-page.component.html",
  styleUrls: ["./add-record-page.component.scss"],
})
export class AddRecordPageComponent implements OnInit {
  
  constructor(public utility: Utility,private activeRouter: ActivatedRoute,
    private route: Router) { }

  model: CarManageRecord = new CarManageRecord();

  ngOnInit(): void {}

  signature() {
    var navigateTo = "/ESignature";
    var navigationExtras = {
      queryParams: {
        id: this.model.id,
        driverName: this.model.driverName,
        licenseNumber: this.model.licenseNumber,
        actionCode:"Edit"
      },
      skipLocationChange: true,
    };
    this.route.navigate([navigateTo], navigationExtras);
  }

  save() {
    alert("save success!");
  }
}
