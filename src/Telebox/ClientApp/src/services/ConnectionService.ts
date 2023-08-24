import ServiceBase from "services/ServiceBase";
import { API_URL } from "configuration/Base";
import { IConnection, ICreateConnectionRequest } from "models/Connection";

class ConnectionService extends ServiceBase {
  constructor() {
    super(API_URL);
  }

  getAll = async (): Promise<IConnection[]> => {
    return await this.get(`/connections`);
  };

  getById = async (connectionId: string): Promise<IConnection> => {
    return await this.get(`/connections/${connectionId}`);
  };

  create = async (
    connection: ICreateConnectionRequest
  ): Promise<IConnection> => {
    return await this.post(`/connections`, connection);
  };

  remove = async (connectionId: string): Promise<void> => {
    return await this.delete(`/connections/${connectionId}`);
  };
}

export default ConnectionService;
