interface Yard {
  id: number;
  yardNo: string;
  yardType: string;
  yardZone: string;
  trailerType: string;
  locationNo: string;
  status: string;
  userStamp: string;
  createDate: Date;
  modDate: Date;
}

interface YardMonitor {
  yardId: number;
  yardNo: string;
  yardType: string;
  yardZone: string;
  yardStatus: string;
  trailerId: number;
  trailerLicensePlate: string;
  trailerStatus: string;
}
export { Yard, YardMonitor };
