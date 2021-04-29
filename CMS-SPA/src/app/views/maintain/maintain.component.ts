import { Component, OnInit } from "@angular/core";
import { FormArray, FormBuilder, FormControl, FormGroup } from "@angular/forms";
import { Utility } from "../../core/utility/utility";
import { Car } from "../../core/_models/car";
import { CarManageRecord } from "../../core/_models/car-manage-record";
import { Company } from "../../core/_models/company";
import { Department } from "../../core/_models/department";
import { CmsService } from "../../core/_services/cms.service";

@Component({
  selector: "app-maintain",
  templateUrl: "./maintain.component.html",
  styleUrls: ["./maintain.component.scss"],
})
export class MaintainComponent implements OnInit {
  //carList: Car[] = [];
  //companyList: Company[] = [];
  //departmentList: Department[] = [];

  public readonly companyFormGroup: FormGroup;
  public companys: FormArray; // formArrayName

  public readonly carFormGroup: FormGroup;
  public cars: FormArray; // formArrayName

  public readonly departmentFormGroup: FormGroup;
  public departments: FormArray; // formArrayName

  constructor(
    public utility: Utility,
    private cmsService: CmsService,
    private readonly fb: FormBuilder
  ) {
    this.companyFormGroup = this.fb.group({
      companys: this.fb.array([]),
    });
    this.carFormGroup = this.fb.group({
      cars: this.fb.array([]),
    });
    this.departmentFormGroup = this.fb.group({
      departments: this.fb.array([]),
    });
  }

  ngOnInit() {
    this.getAllCarList();
    this.getAllCompany();
    this.getAllDepartment();
  }
  getAllCarList() {
    this.utility.spinner.show();
    this.cmsService.getAllCarList().subscribe(
      (res) => {
        this.utility.spinner.hide();
        res.map((x) => {
          this.cars = this.getCarForm;
          this.cars.push(this.createCar(x.id, x.carSize));
        });
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
  getAllDepartment() {
    this.utility.spinner.show();
    this.cmsService.getAllDepartment().subscribe(
      (res) => {
        this.utility.spinner.hide();
        res.map((x) => {
          this.departments = this.getDepartmentForm;
          this.departments.push(this.createDepartment(x.id, x.departmentName));
        });
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
  getAllCompany() {
    this.utility.spinner.show();
    this.cmsService.getAllCompany().subscribe(
      (res) => {
        this.utility.spinner.hide();
        res.map((x) => {
          this.companys = this.getCompanyForm;
          this.companys.push(
            this.createCompany(x.id, x.companyName, x.companyDistance)
          );
        });
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
  ////// Company Form control ///////
  get getCompanyForm(): FormArray {
    return this.companyFormGroup.get("companys") as FormArray;
  }
  createCompany(
    id?: number,
    companyName?: string,
    companyDistance?: string
  ): FormGroup {
    return this.fb.group({
      id: id,
      companyName: companyName,
      companyDistance: companyDistance,
    });
  }
  addEmptyCompany(): void {
    //this.companys = this.companyFormGroup.get("companys") as FormArray;
    this.companys = this.getCompanyForm;
    this.companys.push(this.createCompany());
  }
  //i: index of the list
  removeCompany(i: number) {
    console.log(this.companys.at(i));
    this.companys.removeAt(i);
  }
  submitCompanyList() {
    //console.log(this.companys.value);
    console.log(this.utility.getChangedProperties(this.companys));
  }
  ////// Company Form control ///////

  ////// Car Form control ///////
  get getCarForm(): FormArray {
    return this.carFormGroup.get("cars") as FormArray;
  }
  createCar(id?: number, carSize?: string): FormGroup {
    return this.fb.group({
      id: id,
      carSize: carSize,
    });
  }
  addEmptyCar(): void {
    this.cars = this.getCarForm;
    this.cars.push(this.createCar());
  }
  //i: index of the list
  removeCar(i: number) {
    console.log(this.cars.at(i));
    this.cars.removeAt(i);
  }
  submitCarList() {
    console.log(this.utility.getChangedProperties(this.cars));
    //console.log(this.cars.value);
  }
  ////// Car Form control ///////


  ////// Department Form control ///////
  get getDepartmentForm(): FormArray {
    return this.departmentFormGroup.get("departments") as FormArray;
  }
  createDepartment(id?: number, departmentName?: string): FormGroup {
    return this.fb.group({
      id: id,
      departmentName: departmentName,
    });
  }
  addEmptyDepartment(): void {
    this.departments = this.getDepartmentForm;
    this.departments.push(this.createDepartment());
  }
  //i: index of the list
  removeDepartment(i: number) {
    console.log(this.departments.at(i));
    this.departments.removeAt(i);
  }
  submitDepartmentList() {
    console.log(this.utility.getChangedProperties(this.departments));
    //console.log(this.cars.value);
  }
  ////// Car Form control ///////

}
