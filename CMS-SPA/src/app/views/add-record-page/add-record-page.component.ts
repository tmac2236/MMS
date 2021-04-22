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
          this.model.signInDate = params.signInDate;
          this.model.licenseNumber = params.licenseNumber;
          break;
        }
        case UrlParamEnum.Report :{
          this.model.signInDate = params.signInDate;
          this.model.licenseNumber = params.licenseNumber;
          this.getTheRecord();
          break;
        }
      }

    });
  }

  ngOnInit(): void {
    this.getAllCarList();
    this.getAllCompany();
    this.getAllDepartment();
  }

  signature() {
    var navigateTo = "/ESignature";
    var navigationExtras = {
      queryParams: {
        signInDate: this.model.signInDate,
        licenseNumber: this.model.licenseNumber,
        actionCode: UrlParamEnum.AddRecordSignature,
      },
      skipLocationChange: true,
    };
    this.route.navigate([navigateTo], navigationExtras);
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
  save(){
    this.utility.spinner.show();
    this.cmsService.addRecord(this.model).subscribe(
      (res) => {
        this.utility.spinner.hide();
        this.utility.alertify.confirm(
          "Sweet Alert",
          "Add Success !",
          () => { 
            this.model = res;
            this.signature(); });  
      },
      (error) => {
        this.utility.spinner.hide();
        this.utility.alertify.error(error);
      }
    );
  }
  edit(){
    this.utility.spinner.show();
    this.cmsService.editRecord(this.model).subscribe(
      (res) => {
        this.utility.spinner.hide();
        this.utility.alertify.confirm(
          "Sweet Alert",
          "Edit Success !",
          () => { 
            this.model = res; });  
      },
      (error) => {
        this.utility.spinner.hide();
        this.utility.alertify.error(error);
      }
    );
  }
  signOut(){
    this.utility.spinner.show();
    this.cmsService.signOutRecord(this.model).subscribe(
      (res) => {
        this.utility.spinner.hide();
        this.utility.alertify.confirm(
          "Sweet Alert",
          "SignOut Success !",
          () => { 
            this.model = res; });  
      },
      (error) => {
        this.utility.spinner.hide();
        this.utility.alertify.error(error);
      }
    );
  }
  getTheRecord(){
    this.utility.spinner.show();
      this.cmsService.getTheRecord(this.model).subscribe(
        (res) => {
          this.utility.spinner.hide();
          this.model = res;
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
