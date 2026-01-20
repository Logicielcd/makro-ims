import { User } from '../account/user.model';

export interface TokenModel {
  accessToken: string;
  refreshToken: string;
  userInfo: User;  
}

