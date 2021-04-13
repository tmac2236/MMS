import { Component, OnInit } from "@angular/core";
import { CarManageRecord } from "../../core/_models/car-manage-record";

@Component({
  selector: "app-add-record-page",
  templateUrl: "./add-record-page.component.html",
  styleUrls: ["./add-record-page.component.scss"],
})
export class AddRecordPageComponent implements OnInit {
  constructor() {}
  model:CarManageRecord = new CarManageRecord();

  ngOnInit() {}
}
