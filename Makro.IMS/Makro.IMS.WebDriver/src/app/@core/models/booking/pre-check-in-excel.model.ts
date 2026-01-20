interface PreCheckInExcelDetailDto {
  bookingId: string;
  warehouseCode: string;
  poNo: string;
  truckType: string;  
  truckLicense: string;
  driverName: string;
  telNo: string;
  lineNo: string;
  supCode: string;
  internalSupGroupId: number;
  importResult: string;
}

export { PreCheckInExcelDetailDto };

interface PreCheckInExcelDto{
  preCheckInExcelDetailDtos: PreCheckInExcelDetailDto[];
}

export { PreCheckInExcelDto }