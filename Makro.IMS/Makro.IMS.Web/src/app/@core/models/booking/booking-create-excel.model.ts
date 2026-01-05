import { BookingHeader } from "./booking-header.model";

interface CreateBookingExcelWarehouse {
  warehouseCode: string;
  poNo: string;
  truckType: string;
  deliveryType: string;
  truckNo: number;
  truckGroup: string;
  remark: string;
  timeSlot: Date;
  inputDataValidate: string;
  importResult: string;
  bookingGroup: string;
  systemOverride: boolean;
  operationType: string;
}

export { CreateBookingExcelWarehouse };

interface CreateBookingExcelDto
{
  supplierCode: string;
  internalSupGroupId: number;
  contactName: string;
  contactEmail: string;
  contactPhone: string;
  bookingDate: Date;  
  createBookingExcelWarehouses: CreateBookingExcelWarehouse[];
  bookingHeaders: BookingHeader[];
}

export { CreateBookingExcelDto };