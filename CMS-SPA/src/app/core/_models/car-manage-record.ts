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
  goodsCount: string;

  departmentName: string;
  contactPerson: string;
  sealNumber: string;
  driverSign: string;
  signOutDate: Date;

  guardName: string;
  carSize: string;
  companyDistance: number;

  constructor() {
    super();
  }
}
