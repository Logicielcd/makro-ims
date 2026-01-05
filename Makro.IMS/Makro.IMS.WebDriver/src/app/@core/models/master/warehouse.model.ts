interface Warehouse {  
  active: boolean;
  warehouseCode: string;
  warehouseName: string;
  warehouseDisplay: string;
  companyCode: string;
  address1: string;
  address2: string;
  address3: string;
  city: string;
  country: string;
  zipcode: string;
  contactEMail: string;
  contactName: string;
  mobileNumber: string;  
  phoneNumber: string;
  note: string;

  bookingIdPrefix: string;
  bookingIdRunning: number;
  firstTimeOfDay: number;
  endTimeOfDay: number;
  timeIncreaseStep: number;
  timeWidth: number;

  maxAllPerHour: number;
  maxCAllPerHour: number;  
  maxCConPerHour: number;
  maxCFullPerHour: number;
  maxCNonPerHour: number;
  maxConPerHour: number;
  maxNonPerHour: number;
  maxFullPerHour: number;
  
  // poList: PoList[];
  onlineBookingIdPrefix: string;
  onlineBookingIdRunning: number;
  fixDoor: string;
  warehouseLevel: string;
  warehouseMain: string;
  id: string;
  isFixDoor: boolean;
  
  advanceBookingPeriod: number;
  poBeforePeriod: number;
  poAfterPeriod: number;
  advanceCheckinTime: number;
  lateCheckinTime: number;
  capacityType: string;
  warehouseWms: string;


}

export { Warehouse };

