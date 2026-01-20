interface QueueManageSearchDto {
  warehouseCode: string;
  startDate: Date;
  endDate: Date;
  status: string;
  operationType: string;
  truckType: string;
  bookingId: string;
  licensePlate: string;
  licensePlate2: string;
}

export { QueueManageSearchDto };

interface QueueManageDto {
  internalHeaderKey: number;
  internalTruckCheckInId: number;
  bookingId: string;
  warehouseCode: string;
  supCode: string;
  supName: string;
  companyCode: string;
  truck: string;
  bookingStart: Date;
  bookingEnd: Date;
  queueSeq: string;
  arrivedTime: Date;
  operationType: string;
  displayArrivedTime: string;
  displaySupplier: string;
  waitingTime: number;
  onDockTime: Date;
  licensePlate: string;
  licensePlate2: string;
  Truck: string;
  TelNo: string;
  waitingTimeDisplay: string;
  isFinish: boolean;
  status: string;
  statusDisplay: string;
  waitingDocumentTime: Date;
  waitingDocument: string;
  remark: string;
  totalTruck: number;
  bookingIdDisplay: string;
  lastUpdate: Date;
  backHaul: string;
}
export { QueueManageDto };


interface DoorQueueDto {
  internalDoorId: number;
  doorName: string;
  doorArea: string;
  truckType: string;
  bookingHeaderKey: number;
  bookingId: string;
  supCode: string;
  supName: string;
  arrivedTime: Date;
  submitDocTime: Date;
  onDockTime: Date;
  startUnloadTime: Date;
  finishUnloadTime: Date;
  departureTime:Date;
  areaTruck: string;
  bookingSup: string;
  status: string;
  licensePlate:string;
  licensePlate2: string;
  internalTruckCheckInId: number;
  calltruckTime: Date;
  checkoutTime: Date;
  doccheckTime: Date;
  bookingTruckType: string;
  waitingTime: number;
  waitingTimeDisplay: string;
}
export { DoorQueueDto };

interface QueueSequence {
  warehouseCode:string;
  operationType:string;
  lastSequence:number;
  queueDate: Date;
}
export { QueueSequence };