import { Base } from '../base';

interface WarehouseCapacity extends Base{
  warehouseCode: string;
  companyCode: string;
  bookingDate: Date;  
  maxConPerHour: number;
  maxNonPerHour: number;
  maxFullPerHour: number;
  maxAllPerHour: number;
  maxCConPerHour: number;
  maxCNonPerHour: number;
  maxCFullPerHour: number;
  maxCAllPerHour: number;
}

interface WarehouseCapacityDto {
  warehouseCode: string;
  bookingDateTime: Date;
  con:number;
  non:number;
  fulL_PL:number;
  cubE_FULL:number;
  cubE_CON:number;
  cubeNon:number;
  maxCon:number;
  maxNon:number;
  maxFullPl: number;
  maxCubeFull: number;
  maxCubeNon: number;
  maX_CUBE_CON: number;
}

export { WarehouseCapacity,WarehouseCapacityDto };

