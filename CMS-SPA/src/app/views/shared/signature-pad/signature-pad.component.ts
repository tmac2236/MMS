import {AfterViewInit, Component, ElementRef, OnInit, ViewChild} from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import SignaturePad from 'signature_pad';
import { UrlParamEnum } from '../../../core/enum/urlParamEnum';
import { Utility } from '../../../core/utility/utility';
import { CarManageRecord } from '../../../core/_models/car-manage-record';
import { CmsService } from '../../../core/_services/cms.service';

@Component({
  selector: 'app-signature-pad',
  templateUrl: './signature-pad.component.html',
  styleUrls: ['./signature-pad.component.scss']
})
export class SignaturePadComponent implements OnInit, AfterViewInit {
  @ViewChild('sPad', {static: true}) signaturePadElement;
  signaturePad: any;

  urlParam: CarManageRecord = new CarManageRecord();
  actionCode:string;

  constructor(public utility: Utility,private activeRouter: ActivatedRoute,
    private route: Router,private translate: TranslateService,private cmsService: CmsService) {
      this.activeRouter.queryParams.subscribe((params) => {
        this.urlParam.licenseNumber = params.licenseNumber;
        this.urlParam.signInDate = params.signInDate;
        this.actionCode = params.actionCode;
      });}

  ngOnInit(): void {
  }

  ngAfterViewInit(): void {
    this.signaturePad = new SignaturePad(this.signaturePadElement.nativeElement);
  }

  changeColor() {
    const r = Math.round(Math.random() * 255);
    const g = Math.round(Math.random() * 255);
    const b = Math.round(Math.random() * 255);
    const color = 'rgb(' + r + ',' + g + ',' + b + ')';
    this.signaturePad.penColor = color;
  }

  clear() {
    this.signaturePad.clear();
  }
  
  undo() {
    const data = this.signaturePad.toData();
    if (data) {
      data.pop(); // remove the last dot or line
      this.signaturePad.fromData(data);
    }
  }

  download(dataURL, filename) {
    if (navigator.userAgent.indexOf('Safari') > -1 && navigator.userAgent.indexOf('Chrome') === -1) {
      window.open(dataURL);
    } else {
      const blob = this.dataURLToBlob(dataURL);
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = filename;

      document.body.appendChild(a);
      a.click();

      window.URL.revokeObjectURL(url);
    }
  }

  dataURLToBlob(dataURL) {
    // Code taken from https://github.com/ebidel/filer.js
    const parts = dataURL.split(';base64,');
    const contentType = parts[0].split(':')[1];
    const raw = window.atob(parts[1]);
    const rawLength = raw.length;
    const uInt8Array = new Uint8Array(rawLength);
    for (let i = 0; i < rawLength; ++i) {
      uInt8Array[i] = raw.charCodeAt(i);
    }
    return new Blob([uInt8Array], { type: contentType });
  }

  save() {
    if (this.signaturePad.isEmpty()) {
      alert('Please provide a signature first.');
    } else {
      var fileName: string = this.urlParam.licenseNumber + this.urlParam.signInDate.toString() + '.jpg';
      //const dataURL = this.signaturePad.toDataURL();
      //const dataURL = this.signaturePad.toDataURL('image/svg+xml');
      const dataURL = this.signaturePad.toDataURL('image/jpg');
      //this.download(dataURL, 'signature.jpg');
      const blob : Blob = this.dataURLToBlob(dataURL);
      //Blob => File
      var file: File = this.utility.blobToFile(blob,fileName);
      var formData = new FormData();
      formData.append("file", file);
      formData.append("licenseNumber", this.urlParam.licenseNumber);
      formData.append("signInDate", this.urlParam.signInDate.toString());
      this.cmsService.addSignaturePic(formData).subscribe(
        () => {
          this.utility.alertify.success("Add succeed!");
          location.reload();
        },
        (error) => {
          this.utility.alertify.error("Add failed !!!!");
        }
      );
      //this.redirect();
    }
  }
  redirect(){
    var urlParamEnum:UrlParamEnum = UrlParamEnum[this.actionCode];
    var navigateTo = "";
    var navigationExtras ={};

    switch(urlParamEnum){
      case UrlParamEnum.AddRecordSignature :{
        navigateTo = "/AddRecordPage";
        navigationExtras = {
          queryParams: {
            actionCode: UrlParamEnum.Signature,
            licenseNumber:this.urlParam.licenseNumber,
            signInDate:this.urlParam.signInDate,
          },

          
          skipLocationChange: true,
        };
        break;
      }

    }
    this.route.navigate([navigateTo], navigationExtras);    
  }
}
