import { Pagination } from "./pagination";

export class CarManageRecord extends Pagination {
  companyId: string;
  plateNumber: string;
  driverName: string;
  licenseNumber: string;
  signInDate: Date;

  tempNumber: string;
  signInReason: string;
  goodsName: string;
  goodsCount: string;
  departmentId: string;

  contactPerson: string;
  sealNumber: string;
  driverSign: string;
  signOutDate: Date;
  guardName: string;

  carId: string;

  constructor() {
    super();
  }
}
