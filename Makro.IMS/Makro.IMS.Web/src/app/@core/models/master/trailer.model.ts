interface Trailer {
  id: number;
  licensePlate: string;
  trailerType: string;
  trailerSize: string;
  status: string;
  locationId: number;
  createDate: Date;
  modDate: Date;
  userStamp: string;
  outDcLicensePlate: string;
  outDcDriver: string;
  trailerGroup: string;
  jobStatus: string;
  jobDockStatus: string;
}

export { Trailer };
