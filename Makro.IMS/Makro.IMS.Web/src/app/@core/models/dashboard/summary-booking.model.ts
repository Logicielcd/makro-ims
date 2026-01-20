interface SummaryBooking {  
  warehouseCode: string;
  businessCode: string;
  monitorDate: Date;
  inWarehouseQty: number;
  bookingQty: number;
  checkInQty: number;
  pendingQty: number;
  assignQueueQty: number;
  unloadingQty: number;
  endUnloadQty: number;
  waitingDocQty: number;
  waitingCheckOutQty: number;
  checkOutQty: number;

}

export { SummaryBooking };
