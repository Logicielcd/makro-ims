interface Job {
  id: number;
  jobId: string;
  jobDate: Date;

  trailerId: number;
  trailerLicensePlate: string;
  trailerType: string;
  fromYardId: number;
  fromYardNo: string;
  fromYardType: string;
  fromYardZone: string;

  shuntId: number;
  shuntLicensePlate: string;
  shuntDriver: string;
  shuntTelNo: string;

  LocationId: number;
  LocationType: string;

  status: string;

  userStamp: string;
  createDate: Date;
  modDate: Date;
}

interface JobTransferRequest {
  trailerId: number;
  shuntId: number;
  locationId: number;
  locationType: string | null;
}

export { Job, JobTransferRequest };
