import { ModelInterface } from "./interface/model-interface";

export class CarManageRecordDto implements ModelInterface {
  companyName: string;
  plateNumber: string;
  driverName: string;
  licenseNumber: string;
  signInDate: Date;

  tempNumber: string;
  signInReason: string;
  goodsName: string;
  goodsCount: string;
  departmentName: string;

  contactPerson: string;
  sealNumber: string;
  driverSign: string;
  signOutDate: Date;
  guardName: string;

  carSize: string;
  companyDistance: number;

  isDisplay: number; //only use in front end
  constructor(){}
}
