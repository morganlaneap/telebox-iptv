import { IChannel, IEpgListing } from "./XStream";

export enum RecordingStatus {
  Scheduled = 0,
  Recording = 1,
  Recorded = 2,
  Errored = 3,
}

export interface IRecording {
  id: number;
  connectionId: number;
  name: string;
  streamId: number;
  epgId: number;
  channelName: string;
  startTime: string;
  endTime: string;
  status: RecordingStatus;
  fileName: string;
  createdAt: string;
}

export interface IScheduleRecordingRequest {
  name: string;
  streamId: number;
  epgId: number;
  channelName: string;
  startTime: string;
  endTime: string;
}

const epgListingToScheduleRecordingRequest = (
  channel: IChannel,
  epg: IEpgListing
): IScheduleRecordingRequest => {
  return {
    name: epg.title,
    streamId: Number.parseInt(channel.streamId),
    epgId: Number.parseInt(epg.id),
    channelName: channel.name,
    startTime: epg.start,
    endTime: epg.end,
  };
};
export { epgListingToScheduleRecordingRequest };
