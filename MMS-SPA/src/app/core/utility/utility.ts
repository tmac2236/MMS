import { DatePipe } from "@angular/common";
import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { FormArray } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { JwtHelperService } from "@auth0/angular-jwt";
import { BsLocaleService } from "ngx-bootstrap/datepicker";
import { NgxSpinnerService } from "ngx-spinner";
import { environment } from "../../../environments/environment";
import { Pagination } from "../_models/pagination";
import { AlertifyService } from "../_services/alertify.service";
import { LanguageService } from "../_services/language.service";

@Injectable({
  providedIn: "root",
})
export class Utility {
  baseUrl = environment.apiUrl;
  serverWebRoot =  environment.serverWebRoot;
  guardPassword = environment.guardPassword;
  admPassword = environment.admPassword;
  //getUserName
  jwtHelper = new JwtHelperService();


  constructor(
    public http: HttpClient,
    public alertify: AlertifyService,
    public spinner: NgxSpinnerService,
    public datepiper: DatePipe,
    public languageService: LanguageService,
    private localeService: BsLocaleService
  ) {}

  logout() {
    localStorage.removeItem("token");
    this.alertify.message("logged out");
  }
  //匯出
  exportFactory(url: string, nameParam: string, params?: Pagination) {
    this.spinner.show();
    this.http
      .post(url, params, { responseType: "blob" })
      .subscribe((result: Blob) => {
        if (result.type !== "application/xlsx") {
          alert(result.type);
          this.spinner.hide();
        }
        const blob = new Blob([result]);
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement("a");
        const currentTime = new Date();
        const filename =
          nameParam +
          currentTime.getFullYear().toString() +
          (currentTime.getMonth() + 1) +
          currentTime.getDate() +
          currentTime
            .toLocaleTimeString()
            .replace(/[ ]|[,]|[:]/g, "")
            .trim() +
          ".xlsx";
        link.href = url;
        link.setAttribute("download", filename);
        document.body.appendChild(link);
        link.click();
        this.spinner.hide();
      });
  }
  //取得目前語言
  getCurrentLang() {
    return this.languageService.translate.currentLang;
  }
  //設定語言
  useLanguage(language: string) {
    this.languageService.setLang(language);
    this.localeService.use(language);
  }
  //設定是否分頁
  setPagination(bo: boolean, objS: Pagination) {
    let powerStr = "on";
    if (!bo) powerStr = "off";
    this.alertify.confirm(
      "System Alert",
      "You just turned " + powerStr + " the pagination mode.",
      () => {
        objS.isPaging = bo;
      }
    );
  }
  getToDay() {
    const toDay =
      new Date().getFullYear().toString() +
      "/" +
      (new Date().getMonth() + 1).toString() +
      "/" +
      new Date().getDate().toString();
    return toDay;
  }

  getDateFormat(day: Date) {
    const dateFormat =
      day.getFullYear().toString() +
      "/" +
      (day.getMonth() + 1).toString() +
      "/" +
      day.getDate().toString();
    return dateFormat;
  }

  getAccount() {
    const jwtTtoken = localStorage.getItem("token");
    if (jwtTtoken) {
      return this.jwtHelper.decodeToken(jwtTtoken)["unique_name"];
    }
    return "";
  }
  public blobToFile = (theBlob: Blob, fileName: string): File => {
    var b: any = theBlob;
    //A Blob() is almost a File() - it's just missing the two properties below which we will add
    b.lastModifiedDate = new Date();
    b.name = fileName;

    //Cast to a File() type
    return <File>theBlob;
  };

  public getChangedProperties(form: FormArray): FormArray[] {
    let changedProperties = [];
  
    Object.keys(form.controls).forEach((name) => {
      const currentControl = form.controls[name];
  
      if (currentControl.dirty) {
        changedProperties.push(currentControl);
      }
    });
  
    return changedProperties;
  }
  //check max file
  //e.g  maxValue: 1128659 = 1MB
  checkFileMaxFormat(file: File, maxVal:number) {
    var isLegal = true;
    if (file.type != "image/jpeg") isLegal = false;
    if (file.size >= maxVal) isLegal = false; //最大上傳1MB
    return isLegal;
  }
  getRole(){
    const token = localStorage.getItem('token');
    let role = this.jwtHelper.decodeToken(token)["role"];
    return role;
  }
}
