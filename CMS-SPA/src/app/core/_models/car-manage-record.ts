import { Pagination } from "./pagination";

export class CarManageRecord extends Pagination {
  companyId: number;
  plateNumber: string;
  driverName: string;
  licenseNumber: string;
  signInDate: Date;

  tempNumber: string;
  signInReason: string;
  goodsName: string;
  goodsCount: string;
  departmentId: number;

  contactPerson: string;
  sealNumber: string;
  driverSign: string;
  signOutDate: Date;
  guardName: string;

  carId: number;

  constructor() {
    super();
  }
}
