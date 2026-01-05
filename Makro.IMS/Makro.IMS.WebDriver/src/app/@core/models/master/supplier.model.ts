
interface Supplier {
  internalSupId: number;
  internalGroupId: number;  
  internalSupGroupId:number;
  supCode:string;
  supName:string;
  companyCode:string;
  remark:string;
  contactName:string;
  contactEMail:string;
  phoneNumber:string;
  mobileNumber:string;
  userStamp:string;
}

export { Supplier };

interface SupplierGroup{
  internalSupGroupId:number;
  supName: string;
  remark:string;
  fixTime: boolean;
  comfirmGatePass: boolean;
  fixTimeStart: string;
  fixTimeEnd: string;
  isUpCreateBook: string;
  isUpPreCheckin: string;
  contactName:string;
  contactEMail:string;
  phoneNumber:string;
  mobileNumber:string;  
  postpond: string;
  isFixTime: boolean;
  isConfirmGatePass: boolean;
  isPostPond: boolean;
  isCreateBook: boolean;
  isPreCheckIn: boolean;
  isPallet: boolean;
  userStamp: string;
}

export{SupplierGroup}