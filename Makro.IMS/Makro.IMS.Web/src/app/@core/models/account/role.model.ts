import { AuthMenu } from "../menu/auth-menu.model";

export interface Role {
  name: string;

  authMenus: AuthMenu[];
}
