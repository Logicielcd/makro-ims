interface BookingHeader {
  internalHeaderKey: number;
  internalKeyId: number;
  internalDoorId: number;
  internalSupGroupId: number;
  warehouseCode: string;
  supCode: string;
  supName: string;
  bookingStart: Date;
  bookingEnd: Date;
  totalPo: number;
  totalQty: number;
  userStamp: string;
  createDate: Date;
  modDate: Date;
  postPoned: boolean;
  backHual: boolean;
  active: boolean;
  firstBookginStart: Date;
  firstBookingEnd: Date;
  firstUserStamp: string;
  bookingId:string;
  revisionPrefix:string;
  revisionRunning: number;
  remark: string;
  orgNeedDate: Date;
  bookingDetails: BookingDetail[];
  bookingTrucks: BookingTruck[];
  contactName: string;
  contactEmail:string;
  contactTel:string;
  status:string;
  approveCondition:string;
  merchType:string;
  companyCode:string;
}

interface BookingDetail {
  internalDetailKey: number;
  internalHeaderKey: number;
  poNbr: string;
  totalQty: number;
  createDate: Date;
  modDate: Date;
  userStamp: string;  
  remark: string;
  planRec: Date;
  fullPl: number;
  con: number;
  non: number;
  cubeFull: number;
  cubeCon: number;
  cubeNon: number;
  postponed: string;
  merchType: string;
  weight: number;
}

interface BookingKey {
  internalKeyId: number;
  companyCode: string;
  createDate: Date;
  modDate: Date;
  userStamp: string;
  active: boolean;
  bookingDate: Date;
  mailStatus: string;
  recMailId: number;
  bookingHeaders: BookingHeader[];
}

interface BookingTruck {
  internalHeaderKey: number;
  internalTruckId: number;
  totalTruck: number;
  truckName: string;
  driverName: string;
  licensePlate: string;
  poNo: string;
  bookingCheckins: BookingTruckCheckIn[];
}

interface BookingKeyDto {
  internalKeyId:number;
  bookingDate: Date;
  bookingHeaders: BookingHeaderDto[];
  userName:string;
}

interface BookingHeaderDto {    
  internalHeaderKey: number;
  internalDoorId: number;
  internalSupGroupId: number;
  warehouseCode: string;
  supCode: string;
  supName: string;
  bookingStart: Date;
  bookingEnd: Date;  
  firstBookingStart: Date;
  firstBookingEnd: Date;  
  bookingDetails: BookingDetail[];
  bookingTrucks: BookingTruck[];
  contactName: string;
  contactEmail:string;
  contactTel:string;
  bookingId:string;
  dockDoor:string;
  totalTruck:string;
  bookingCheckIns: BookingTruckCheckIn[];  
  postPoned:boolean;
  status:string;
  merchType:string;
  companyCode:string;
}

interface BookingTruckCheckIn{
  internalDetailKey: number;
  internalHeaderKey: number;
  internalTruckCheckInId: number;
  internalTruckId: number;
  licensePlate: string;
  driverName: string;
  telNo: string;
  checkInTime: Date;  
  userStamp: string;
  bookingTruckDetails: BookingTruckCheckInDetail[];
  bookingTruckCheckInDetails: BookingTruckCheckInDetail[];
  displayText: string;
  poNo:string;
  lineId:string;
  truckType:string;
  queueSeq: number;
  arrivedTime: Date;
  submitdocTime: Date;
  assignQueueTime: Date;
  ondockTime: Date;
  startUnloadTime: Date;
  finishUnloadTime: Date;
  departureTime: Date;
  status: string;
  calltruckTime: Date;
  checkoutTime: Date;
  doccheckTime: Date;
}

interface BookingTruckCheckInDetail{
  internalTruckCheckInId: number;
  internalTruckDetailId: number;
  internalDetailKey: number;
  checkInTime: Date;
  poNbr: string;
  userStamp: string; 
}

interface BookingCheckOut{
  poNo: string;
  supCode: string;
  userStamp: string;
}

interface PreCheckIn{
  internalHeaderKey: number;
  warehouseCode: string;
  bookingTruckCheckIns: BookingTruckCheckIn[];
  userStamp: string;
}

interface BookingPreCheckInDto {    
  internalHeaderKey: number;
  internalDoorId: number;
  internalSupGroupId: number;
  warehouseCode: string;
  supCode: string;
  supName: string;
  bookingStart: Date;
  bookingEnd: Date;  
  firstBookingStart: Date;
  firstBookingEnd: Date;  
  bookingDetails: BookingDetail[];
  bookingTrucks: BookingTruck[];
  contactName: string;
  contactEmail:string;
  contactTel:string;
  bookingId:string;
  dockDoor:string;
  totalTruck:string;
  bookingCheckIns: BookingTruckCheckInDto[];  
  postPoned:boolean;
  merchType:string;
}


interface BookingTruckCheckInDto{
  internalDetailKey: number;
  internalHeaderKey: number;
  internalTruckCheckInId: number;
  internalTruckId: number;
  licensePlate: string;
  driverName: string;
  telNo: string;
  checkInTime: Date;  
  userStamp: string;
  // bookingTruckDetails: BookingTruckCheckInDetail[];
  bookingTruckCheckInDetails: BookingTruckCheckInDetail[];
  displayText: string;
  poNo:string;
  arrivedTime: Date;
  submitdocTime: Date;
  ondockTime:Date;
  startUnloadTime: Date;
  finishUnloadTime: Date;
  departureTime: Date;
  assignQueueTime: Date;
  status: string;
}


interface BookingCheckInDto
{
    internalHeaderKey: number;
    internalDetailKey: number;
    poNbr: string;
    warehouseCode: string;
    bookingId: string;
    truckType: string;
    driverName: string;
    licensePlate: string;
    telNo: string;
    bookingDate: Date;
    lineId: string;
    merchType: string;
}

interface CheckIn{
  poNbr: string;
  userStamp: string;  
  supCode: string;
  bookingHeaderId: number;
  internalTruckCheckInId: number;
  bookingId: string;
}

interface PoCheckInOut{
  bookingId: string;
  internalTruckCheckInId: number;
  id: number;
  poNbr: string;
  dateTimeStamp: Date;
  userStamp: string;
  status: string;
}

export { BookingHeader, BookingDetail, BookingKey,BookingTruck,BookingKeyDto,BookingHeaderDto,BookingTruckCheckIn
  ,BookingTruckCheckInDetail,BookingCheckOut,PreCheckIn,BookingPreCheckInDto,BookingTruckCheckInDto,BookingCheckInDto
  , CheckIn,PoCheckInOut };

