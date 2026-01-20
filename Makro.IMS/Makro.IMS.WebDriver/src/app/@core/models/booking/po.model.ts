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
}

export { PoList };
