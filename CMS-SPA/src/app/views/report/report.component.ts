import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { Utility } from "../../core/utility/utility";
import { CarManageRecord } from "../../core/_models/car-manage-record";

@Component({
  selector: "app-report",
  templateUrl: "./report.component.html",
  styleUrls: ["./report.component.scss"],
})
export class ReportComponent implements OnInit {
  constructor(
    public utility: Utility,
    private activeRouter: ActivatedRoute,
    private route: Router
  ) {}
  sResult: any;
  result: CarManageRecord[] = [];
  ngOnInit() {}

  search() {
    var donut = new CarManageRecord();
    donut.id = "0";
    donut.companyName = "Công Ty TNHH TM-DVXL MT Việt Khải";
    donut.plateNumber = "54X-2862";
    donut.driverName = "Nguyên Văn A";
    donut.licenseNumber = "740118000357";

    donut.signInDate = new Date("2021,003,005,08,05,00,00");
    donut.tempNumber = "015";
    donut.signInReason = "Shipped";
    donut.goodsName = "rác";
    donut.goodsCount = "4570kg";

    donut.departmentName = "A7";
    donut.contactPerson = "Nguyen Van A";
    donut.sealNumber = "000001";
    donut.driverSign = "tài xế ký tên";
    donut.signOutDate = new Date("2021,003,005,09,45,00,00");

    donut.guardName = "Nguyễn Thị Phê";
    donut.carSize = "10T";
    donut.companyDistance = 10;

    this.result.push(donut);
    alert("Coding.... Search Not yet completed");
  }
  export() {
    alert("Coding.... Export Not yet completed");
  }
  edit(model: CarManageRecord) {
    alert("You just click id:" + model.id);
  }
}
