import { Pagination } from "./pagination";

export class CarManageRecord extends Pagination {
  id: string;
  companyName: string;
  plateNumber: string;
  driverName: string;
  licenseNumber: string;

  signInDate: Date;
  tempNumber: string;
  signInReason: string;
  goodsName: string;
  goodsCount: number;

  departmentName: string;
  contactPerson: string;
  sealNumber: string;
  driverSign: string;
  signOutDate: Date;

  guardName: string;
  companyDistance: number;

  constructor() {
    super();
  }
}
