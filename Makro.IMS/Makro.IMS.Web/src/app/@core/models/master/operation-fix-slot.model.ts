import { SupplierGroup } from "./supplier.model";

interface OperationFixSlot {
    id: number;
    operationType: string;
    warehouseCode: string;
    supGroupId: number;
    startTime: Date;
    endTime: Date;
    supGroup: SupplierGroup;
    daysOfWeek: string;
    dayName: string;
    supGroupName: string;
    isVip: string;
  }
  
  export { OperationFixSlot };
  