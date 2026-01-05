import { BookingHeader } from "./booking-header.model";

interface CreateBookingExcelWarehouse {
  warehouseCode: string;
  fourwheelQty: number;
  sixwheelQty: number;
  tenwheelQty: number;
  eighteenwheelQty: number;
  timeSlot: Date;
  importResult: string;
}

export { CreateBookingExcelWarehouse };

interface CreateBookingExcelPo {
  poNo: string;  
  remark: string;
  importResult: string;
}

export { CreateBookingExcelPo };

interface CreateBookingExcelDto
{
  supplierCode: string;
  internalSupGroupId: number;
  contactName: string;
  contactEmail: string;
  contactPhone: string;
  bookingDate: Date;
  createBookingExcelPos: CreateBookingExcelPo[];
  createBookingExcelWarehouses: CreateBookingExcelWarehouse[];
  bookingHeaders: BookingHeader[];
}

export { CreateBookingExcelDto };