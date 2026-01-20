interface JobOnDock {
  id: number;
  jobId: string;
  locationNo: string;
  locationZone: string;
  locationType: string;
  trailerId: number;
  trailerLicensePlate: string;
  trailerStatus: string;
  status: string;
  createDate: Date;
  modDate: Date;
  totalProcessTime: string;
}

export { JobOnDock };
