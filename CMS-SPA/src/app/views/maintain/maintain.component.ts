import { Component, OnInit } from '@angular/core';
import { Utility } from '../../core/utility/utility';
import { Car } from '../../core/_models/car';
import { CarManageRecord } from '../../core/_models/car-manage-record';
import { Company } from '../../core/_models/company';
import { Department } from '../../core/_models/department';
import { CmsService } from '../../core/_services/cms.service';

@Component({
  selector: 'app-maintain',
  templateUrl: './maintain.component.html',
  styleUrls: ['./maintain.component.scss']
})
export class MaintainComponent implements OnInit {

  carList: Car[] = [];
  companyList: Company[] = [];
  departmentList: Department[] =[];
  constructor(public utility: Utility, private cmsService:CmsService) { }

  ngOnInit() {
    this.getAllCarList();
    this.getAllCompany();
    this.getAllDepartment();
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
