export interface ICategory {
  id: number;
  name: string;
}

export interface IChannel {
  id: number;
  name: string;
  streamId: string;
  epgChannelId: string;
  categoryId: number;
  categoryName: string;
  iconUrl: string;
}

export interface IEpgListing {
  id: string;
  epgId: string;
  title: string;
  language: string;
  start: string;
  end: string;
  description: string;
  channelId: string;
}
