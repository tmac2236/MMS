import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { UrlParamEnum } from "../../core/enum/urlParamEnum";
import { Utility } from "../../core/utility/utility";
import { Car } from "../../core/_models/car";
import { CarManageRecord } from "../../core/_models/car-manage-record";
import { Company } from "../../core/_models/company";
import { Department } from "../../core/_models/department";
import { CmsService } from "../../core/_services/cms.service";
@Component({
  selector: "app-add-record-page",
  templateUrl: "./add-record-page.component.html",
  styleUrls: ["./add-record-page.component.scss"],
})
export class AddRecordPageComponent implements OnInit {

  model: CarManageRecord = new CarManageRecord();
  actionCode:string;
  carList: Car[] = [];
  companyList: Company[] = [];
  departmentList: Department[] =[];

  constructor(
    public utility: Utility,private activeRouter: ActivatedRoute,private route: Router,private cmsService:CmsService) {
    this.activeRouter.queryParams.subscribe((params) => {

      this.actionCode = params.actionCode;
      var urlParamEnum:UrlParamEnum = UrlParamEnum[this.actionCode];

      switch(urlParamEnum){
        case UrlParamEnum.Signature :{
          this.model.id = params.id;
          break;
        }
      }

    });
  }

  ngOnInit(): void {}

  signature() {
    var navigateTo = "/ESignature";
    var navigationExtras = {
      queryParams: {
        id: this.model.id,
        driverName: this.model.driverName,
        licenseNumber: this.model.licenseNumber,
        actionCode: UrlParamEnum.AddRecordSignature,
      },
      skipLocationChange: true,
    };
    this.route.navigate([navigateTo], navigationExtras);
  }

  save() {
    this.signature();
  }

  getAllCarList(){
    this.utility.spinner.show();
    this.cmsService.getAllCarList().subscribe(
      (res) => {
        this.utility.spinner.hide();
        this.carList = res;
      },
      (error) => {
        this.utility.spinner.hide();
        this.utility.alertify.confirm(
          "System Notice",
          "Syetem is busy, please try later.",
          () => {}
        );
      }
    );  
  }
  getAllDepartment(){
    this.utility.spinner.show();
    this.cmsService.getAllDepartment().subscribe(
      (res) => {
        this.utility.spinner.hide();
        this.departmentList = res;
      },
      (error) => {
        this.utility.spinner.hide();
        this.utility.alertify.confirm(
          "System Notice",
          "Syetem is busy, please try later.",
          () => {}
        );
      }
    );  
  }
  getAllCompany(){
    this.utility.spinner.show();
    this.cmsService.getAllCompany().subscribe(
      (res) => {
        this.utility.spinner.hide();
        this.companyList = res;
      },
      (error) => {
        this.utility.spinner.hide();
        this.utility.alertify.confirm(
          "System Notice",
          "Syetem is busy, please try later.",
          () => {}
        );
      }
    );  
  }
  
}