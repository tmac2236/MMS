import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { UrlParamEnum } from "../../core/enum/urlParamEnum";
import { Utility } from "../../core/utility/utility";
import { CarManageRecordDto } from "../../core/_models/car-manage-record-dto";
import { Pagination } from "../../core/_models/pagination";
import { SCarManageRecordDto } from "../../core/_models/s-car-manage-record-dto";
import { defineLocale } from 'ngx-bootstrap/chronos';
import { zhCnLocale } from "ngx-bootstrap/locale"; //中文
import { viLocale } from "ngx-bootstrap/locale"; //越文
import { enGbLocale } from "ngx-bootstrap/locale"; //英文
defineLocale("zh-cn", zhCnLocale); //定義local中文
defineLocale("vn", viLocale);//定義local越文
defineLocale("en", enGbLocale);//定義local英文

@Component({
  selector: "app-report",
  templateUrl: "./report.component.html",
  styleUrls: ["./report.component.scss"],
})
export class ReportComponent implements OnInit {

  actionCode:string;
  result: CarManageRecordDto[] = [];
  scarManageRecordDto: SCarManageRecordDto = new SCarManageRecordDto();
  deadlineNow = new Date(new Date().getTime() - 86400000); // now minus one day

  constructor(
    public utility: Utility,
    private activeRouter: ActivatedRoute,
    private route: Router,
  ) {
    this.activeRouter.queryParams.subscribe((params) => {
      this.actionCode = params.actionCode;
      var urlParamEnum:UrlParamEnum = UrlParamEnum[this.actionCode];

      switch(urlParamEnum){
        /*if want keep condition after press previous page
        case UrlParamEnum.AddRecordSignature :{
          this.scarManageRecordDto = JSON.parse(params.sCondition);
  
          break;
        }
        */
        default :{
          this.scarManageRecordDto.signInDateS = this.utility.datepiper.transform(
            new Date(),
            "yyyy-MM-dd"
          );
          this.scarManageRecordDto.signInDateE = this.utility.datepiper.transform(
            new Date(),
            "yyyy-MM-dd"
          );
          break;
        }
      }
    });
  }
  ngOnInit() {

    this.getAllCompany();
    this.getAllDepartment();
    this.search();
  }

  edit(model: CarManageRecordDto) {
    var navigateTo = "/Transaction/AddRecordPage";
    var navigationExtras = {
      queryParams: {
        sCondition: JSON.stringify(this.scarManageRecordDto),
        signInDate: model.signInDate,
        licenseNumber: model.licenseNumber,
        actionCode: UrlParamEnum.Report,
      },
      skipLocationChange: true,
    };
    this.route.navigate([navigateTo], navigationExtras);
  }

  search() {

  }

  //分頁按鈕
  pageChangeds(event: any): void {
    this.scarManageRecordDto.currentPage = event.page;
    this.search();
  }

  export() {
    const url = this.utility.baseUrl + "CMS/exportReport";
    this.utility.exportFactory(url, "CMS_Report", this.scarManageRecordDto);
  }
  getAllDepartment() {
  }
  getAllCompany() {
    
  }
  confirmBtn(model: CarManageRecordDto) {
    
  }
  viewPic(driverSign: string) {
    let dataUrl = "../assets/ReportPics/" + driverSign;
    window.open(dataUrl);
  }
  clearCondition() {
    this.scarManageRecordDto = new SCarManageRecordDto();
  }
  //due to JSON.parse(params.sCondition) so created this
  public setPagination(sCondition: Pagination,pagination: Pagination) {
    sCondition.currentPage = pagination.currentPage;
    sCondition.itemsPerPage = pagination.itemsPerPage;
    sCondition.totalItems = pagination.totalItems;
    sCondition.totalPages = pagination.totalPages;
  }
}
