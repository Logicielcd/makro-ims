import { SupplierGroup } from "../master/supplier.model";
import { AuthMenu } from "../menu/auth-menu.model";

interface User {
  userId: string;
  userName: string;
  lastName: string;
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

  warehouseCode: string;
  operationType: string;
}

export { User };

