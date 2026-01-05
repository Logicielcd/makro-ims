interface InboundBookingDto{
    warehouseCode: string;
    operationType: string;
    supCode: string;
    supName: string;
    subGroupId: number;
    bookingDate: Date;
    startTime: Date;
    endTime: Date;
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

  export { InboundBookingDto};