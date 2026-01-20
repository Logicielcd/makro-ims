interface PreCheckInExcelDetailDto {
  bookingId: string;
  warehouseCode: string;
  truckNo: string;
  truckType: string;  
  truckLicense: string;
  truckLicense2: string;
  truckSequence: string;
  driverName: string;
  telNo: string;
  lineNo: string;
  supCode: string;
  internalSupGroupId: number;
  importResult: string;
  validateExcel: string;
}

export { PreCheckInExcelDetailDto };

interface PreCheckInExcelDto{
  preCheckInExcelDetailDtos: PreCheckInExcelDetailDto[];
}

export { PreCheckInExcelDto }