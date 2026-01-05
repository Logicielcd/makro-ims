interface BookingTruckLog {
  id: number;
  internalTruckCheckInId: number;
  action: string;
  remark:string;
  userStamp: string;  
  dateTimeStamp: Date;
  actionDisplay: string;
}

export { BookingTruckLog };
