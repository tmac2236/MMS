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
  companyList: Company[] = [];
  departmentList: Department[] = [];

  public readonly carFormGroup: FormGroup;
  public cars: FormArray; // formArrayName

  constructor(
    public utility: Utility,
    private cmsService: CmsService,
    private readonly fb: FormBuilder
  ) {
    this.carFormGroup = this.fb.group({
      cars: this.fb.array([this.createCar()]),
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
        res.map(x =>{
          this.cars = this.carFormGroup.get("cars") as FormArray;
          this.cars.push(this.createCar(x.id,x.carSize));
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
  getAllCompany() {
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
  ////// Car Form control ///////
  get carControls() {
    return this.carFormGroup.get("cars")["controls"];
  }
  createCar(id?: number, carSize?: string): FormGroup {
    return this.fb.group({
      id: id,
      carSize: carSize,
    });
  }
  addEmptyCar(): void {
    this.cars = this.carFormGroup.get("cars") as FormArray;
    this.cars.push(this.createCar());
  }
  removeCar(i: number) {
    this.cars.removeAt(i);
  }
  logValue() {
    console.log(this.cars.value);
  }
  ////// Car Form control ///////
}
