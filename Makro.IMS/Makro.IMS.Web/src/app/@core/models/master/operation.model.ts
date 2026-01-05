interface Operation {
  operationName: string;
  warehouseCode: string;
  description: string;
  searchkey: string;
  capacity: number;
  capacityLarge: number;
  overcap: number;
  overcutoff: number;
}

export { Operation };
