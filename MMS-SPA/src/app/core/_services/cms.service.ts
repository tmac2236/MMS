import { HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { map } from 'rxjs/operators';
import { Utility } from "../utility/utility";
import { Car } from "../_models/car";
import { CarManageRecord } from "../_models/car-manage-record";
import { CarManageRecordDto } from "../_models/car-manage-record-dto";
import { Company } from "../_models/company";
import { Department } from "../_models/department";
import { PaginatedResult } from "../_models/pagination";
import { SCarManageRecordDto } from "../_models/s-car-manage-record-dto";

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
  getTheRecord(model: CarManageRecord){
    return this.utility.http.post<CarManageRecord>(
      this.utility.baseUrl + 'CMS/getTheRecord',
      model
    );    
  }
  getLastRecord(model: CarManageRecord){
    return this.utility.http.post<CarManageRecord>(
      this.utility.baseUrl + 'CMS/getLastRecord',
      model
    );    
  }
  addRecord(model: CarManageRecord){
    return this.utility.http.post<CarManageRecord>(
      this.utility.baseUrl + 'CMS/addRecord',
      model
    );
  }
  editRecord(model: CarManageRecord){
    return this.utility.http.post<CarManageRecord>(
      this.utility.baseUrl + 'CMS/editRecord',
      model
    );
  }
  signOutRecord(model: CarManageRecord){
    return this.utility.http.post<CarManageRecord>(
      this.utility.baseUrl + 'CMS/signOutRecord',
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
  getCarManageRecordDto(sCarManageRecordDto: SCarManageRecordDto): Observable<PaginatedResult<CarManageRecordDto[]>>{
    
    const paginatedResult: PaginatedResult<CarManageRecordDto[]> = new PaginatedResult<CarManageRecordDto[]>();
    let params = new HttpParams();

    params = params.append('IsPaging', sCarManageRecordDto.isPaging.toString());
    if (sCarManageRecordDto.currentPage != null && sCarManageRecordDto.itemsPerPage != null) {
      params = params.append('pageNumber', sCarManageRecordDto.currentPage.toString());
      params = params.append('pageSize', sCarManageRecordDto.itemsPerPage.toString());
      //params = params.append('orderBy', sAttendance.orderBy);
    }
    params = params.append('licenseNumber', sCarManageRecordDto.licenseNumber.toString());
    params = params.append('signInDateS', sCarManageRecordDto.signInDateS);
    params = params.append('signInDateE', sCarManageRecordDto.signInDateE);

    return this.utility.http
    .get<CarManageRecordDto[]>(this.utility.baseUrl + 'CMS/getCarManageRecordDto' , {
      observe: 'response',
      params,
    })
    .pipe(
      map((response) => {
        paginatedResult.result = response.body;
        if (response.headers.get('Pagination') != null) {
          paginatedResult.pagination = JSON.parse(
            response.headers.get('Pagination')
          );
        }
        return paginatedResult;
      })
    );
  }
  addOrUpdateCompanyList(companyList: Company[]){
    return this.utility.http.post<boolean>(
      this.utility.baseUrl + 'CMS/addOrUpdateCompanyList',
      companyList
    );
  }
  addOrUpdateCarList(carList: Car[]){
    return this.utility.http.post<boolean>(
      this.utility.baseUrl + 'CMS/addOrUpdateCarList',
      carList
    );
  }
  addOrUpdateDepartmentList(departmentList: Department[]){
    return this.utility.http.post<boolean>(
      this.utility.baseUrl + 'CMS/addOrUpdateDepartmentList',
      departmentList
    );
  }
  
}
