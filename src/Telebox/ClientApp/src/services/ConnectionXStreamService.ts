import ServiceBase from "services/ServiceBase";
import { API_URL } from "configuration/Base";
import { ICategory, IChannel, IEpgListing } from "models/XStream";

class ConnectionXStreamService extends ServiceBase {
  constructor() {
    super(API_URL);
  }

  getCategories = async (connectionId: string): Promise<ICategory[]> => {
    return await this.get(`/connections/${connectionId}/xstream/categories`);
  };

  getChannels = async (
    connectionId: string,
    categoryId: number
  ): Promise<IChannel[]> => {
    return await this.get(
      `/connections/${connectionId}/xstream/categories/${categoryId}/channels`
    );
  };

  getChannelEpg = async (
    connectionId: string,
    categoryId: number,
    channelId: string
  ): Promise<IEpgListing[]> => {
    return await this.get(
      `/connections/${connectionId}/xstream/categories/${categoryId}/channels/${channelId}/epg`
    );
  };
}

export default ConnectionXStreamService;
