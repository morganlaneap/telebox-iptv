export interface IConnection {
  id: number;
  name: string;
  username: string;
  password: string;
  serverUrl: string;
  createdAt: string;
}

export interface ICreateConnectionRequest {
  name: string;
  username: string;
  password: string;
  serverUrl: string;
}

export const EmptyCreateConnectionRequest: ICreateConnectionRequest = {
  name: "",
  username: "",
  password: "",
  serverUrl: "",
};
