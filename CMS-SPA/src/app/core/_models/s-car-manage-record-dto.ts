import { Pagination } from "./pagination";

export class SCarManageRecordDto extends Pagination {
  licenseNumber: string;
  signInDateS: string;
  signInDateE: string;

  constructor() {
    super();
    this.licenseNumber = "";
    this.signInDateS = "";
    this.signInDateE = "";
    this.isPaging = true; //開分頁
  }
  public setPagination(pagination: Pagination) {
    this.currentPage = pagination.currentPage;
    this.itemsPerPage = pagination.itemsPerPage;
    this.totalItems = pagination.totalItems;
    this.totalPages = pagination.totalPages;
  }
}
