import { SupplierGroup } from "../master/supplier.model";
import { AuthMenu } from "../menu/auth-menu.model";

interface User {
  userId: string;
  userName: string;
  lastname: string;
  name: string;
  email: string;
  password: string;
  role: string;

  authMenus: AuthMenu[];

  translate: string;
  
  supGroup: SupplierGroup;
  userType: string;
  supCode: string;
  internalSupGroupId: number;
  isActive: boolean;
  approved: string;
  isApproved: boolean;
  warehouseCode: string;
  operationType: string;

  userWarehouses: UserWarehouseDto[];
  userSupplierGroups: UserSupplierGroup[];
}

export { User };

interface UserWarehouseDto{
  warehouseCode: string;
}
export { UserWarehouseDto };

interface UserSupplierGroup{
  internalSupGroupId: number;
}
export { UserSupplierGroup };