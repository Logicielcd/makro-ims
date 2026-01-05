interface PoList {
  internal_Po_No: number;
  warehouse_Code: string;
  company_Code:string;
  po_Nbr: string;  
  sup_Code: string;  
  plan_Receive_Date: Date;
  total_Qty: number;
  full: number;
  con: number;
  non: number;
  cube_Full: number;
  cube_Con: number;
  cube_Non: number;
  postPoned: string;
  merch_Type: string;
  weight: number;
  warehouse_name:string;
  company_name:string;
  booking_Id: string;
  full_Cs: number;
  half: number;
  half_Cs: number;
  expire_Date: Date;
  alert: string;
  is_Delay: number;
  delay_Reason: string;
  isDelayReason: boolean;
  create_Date: Date;
}
export { PoList };

interface PoComment {
  po: string;
  comment1: string;
  comment2: string;
  comment3: string;
  comment4: string;
  comment5: string;
  comment6: string;
}
export {PoComment};

interface PoDcDelay{
  poNo: string;
  delayReason: string;
}
export {PoDcDelay};

interface PoLog{
  poNbr: string;
  log: string;
  dateTimeStamp: Date;
  userStamp: string;
}
export {PoLog};