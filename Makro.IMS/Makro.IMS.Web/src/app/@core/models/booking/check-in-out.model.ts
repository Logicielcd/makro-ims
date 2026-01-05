interface GuardCheckInOutDto {
    internalHeaderKey: number;
    internalTruckCheckInId: number;
    bookingId: string;
    userName: string;
  }
  
  export { GuardCheckInOutDto };
  

  interface ManualBookingDto{
    warehouseCode: string;
    operationType: string;
    supCode: string;
    supName: string;
    subGroupId: number;
    bookingDate: Date;
    licensePlate: string;
    licensePlate2: string;
    driverName: string;
    telNo: string;
    truckType: string;
    lineNo: string;
    userName: string;
    contactName: string;
    contactPhone: string;
    contactEmail: string;
  }

  export { ManualBookingDto};