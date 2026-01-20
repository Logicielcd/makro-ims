import { BookingDetail, BookingTruck } from "./booking-header.model";

interface BookingCreate {
  warehouseCode: string;
  warehouseName: string;
  warehouseDisplay: string;
  companyCode: string;
  bookingDate: Date;
  bookingDetail: BookingDetail[];
  bookingTruck: BookingTruck[];
  truckSelected: number;
  driverName:string;
  telNo:string;
  internalDoorId: number;
  dockDoor:string;
  startTime: Date;
  endTime: Date;
  bookingId: string;
  totalTruck:number;
  internalHeaderKey:number;
  merchType: string;
  status: string;
  whseCap: string;
  postponed: boolean;
  backHaul: boolean;
  
  inputPoNo: string;
  remarkDelay: string;
  remark: string;

  minBookingDateTime: Date;
  isLate: boolean;
  isDelay: boolean;
  isCanBook: boolean;
  merchTypeDisplay: string;

  allApproved: boolean;

  totalQty: number;

  firstTimeStart: Date;
  firstTimeEnd: Date;
  firstUser: string;

  nextShift: Date;
  overCutoff: boolean;
}

export { BookingCreate };
