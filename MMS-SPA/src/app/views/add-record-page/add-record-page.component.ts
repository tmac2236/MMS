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
    if(this.checkFormValidate()) {
      this.utility.alertify.confirm(
        "Sweet Alert",
        "Please select Company、Department、Car !",
        () => { });  
        return;
    } 
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
    if(this.checkFormValidate()) {
      this.utility.alertify.confirm(
        "Sweet Alert",
        "Please select Company、Department、Car !",
        () => {});  
        return;
    }
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
  getLastRecord(){
    console.log(this.model);
    this.utility.spinner.show();
      this.cmsService.getLastRecord(this.model).subscribe(
        (res) => {
          this.utility.spinner.hide();
          if(res == null){
            console.log("The license :" +this.model.licenseNumber + " Number has no record.");
            return;
          } 
          this.model.companyId = res.companyId;
          this.model.plateNumber = res.plateNumber;
          this.model.driverName = res.driverName;
          this.model.licenseNumber = res.licenseNumber;
          this.model.tempNumber = res.tempNumber;
          this.model.signInReason = res.signInReason;
          this.model.goodsName = res.goodsName;
          this.model.goodsCount = res.goodsCount;
          this.model.departmentId = res.departmentId;
          this.model.contactPerson = res.contactPerson;
          this.model.sealNumber = res.sealNumber;
          this.model.guardName = res.guardName;
          this.model.carId = res.carId;
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
  checkFormValidate(){
    let flag = false;
    if(this.model.companyId == null|| this.model.carId == null || this.model.departmentId ==null){
      flag = true
    }
    return flag;
  }
}
