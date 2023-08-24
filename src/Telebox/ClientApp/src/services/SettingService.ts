import ServiceBase from "services/ServiceBase";
import { API_URL } from "configuration/Base";
import { ISetting, IUpdateSettingRequest } from "models/Setting";

class SettingService extends ServiceBase {
  constructor() {
    super(API_URL);
  }

  getAll = async (): Promise<ISetting[]> => {
    return await this.get(`/settings`);
  };

  update = async (request: IUpdateSettingRequest): Promise<ISetting[]> => {
    return await this.put(`/settings`, request);
  };
}

export default SettingService;
