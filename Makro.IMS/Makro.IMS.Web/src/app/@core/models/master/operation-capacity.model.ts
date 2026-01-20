interface OperationCapacity {
  id: number;
  warehouseCode: string;
  operationType: string;
  time: Date;
  monCap: number;
  tueCap: number;
  wedCap: number;
  thuCap: number;
  friCap: number;
  satCap: number;
  sunCap: number;
  monTruck: number;
  tueTruck: number;
  wedTruck: number;
  thuTruck: number;
  friTruck: number;
  satTruck: number;
  sunTruck: number;
}

export { OperationCapacity };
