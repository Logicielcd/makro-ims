import { Base } from '../base';

interface WarehouseOperationCapacity extends Base{
  warehouseCode: string;
  operationType: string;
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

interface WarehouseOperationCapacityDto {
  warehouseCode: string;
  operationType: string;
  bookingDateTime: Date;
  con:number;
  non:number;
  fullPl:number;
  cubeFull:number;
  cubE_CON:number;
  cubeNon:number;
  maxCon:number;
  maxNon:number;
  maxFullPl: number;
  maxCubeFull: number;
  maxCubeNon: number;
  maX_CUBE_CON: number;
}

export { WarehouseOperationCapacity,WarehouseOperationCapacityDto };

