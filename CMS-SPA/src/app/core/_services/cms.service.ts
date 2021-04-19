import { HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { map } from 'rxjs/operators';
import { Utility } from "../utility/utility";
import { Car } from "../_models/car";
import { CarManageRecord } from "../_models/car-manage-record";
import { Company } from "../_models/company";
import { Department } from "../_models/department";
import { PaginatedResult } from "../_models/pagination";

@Injectable({
  providedIn: "root",
})
export class CmsService {
  constructor(private utility: Utility) {}

  getAllCarList(){
    return this.utility.http.get<Car[]>(this.utility.baseUrl +"CMS/getAllCarList");
  }
  getAllDepartment(){
    return this.utility.http.get<Department[]>(this.utility.baseUrl +"CMS/getAllDepartment");
  }
  getAllCompany(){
    return this.utility.http.get<Company[]>(this.utility.baseUrl +"CMS/getAllCompany");
  }
  addRecord(model: CarManageRecord){
    return this.utility.http.post<CarManageRecord>(
      this.utility.baseUrl + 'CMS/addRecord',
      model
    );
  }
  addSignaturePic(formData :FormData){
    console.log("cms.service addSignature:", formData);
    return this.utility.http.post(
      this.utility.baseUrl + "CMS/addSignaturePic",
      formData
    );
  }
  
}
