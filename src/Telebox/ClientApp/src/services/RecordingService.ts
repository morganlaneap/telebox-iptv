import ServiceBase from "services/ServiceBase";
import { API_URL } from "configuration/Base";
import { IRecording, IScheduleRecordingRequest } from "models/Recording";

class RecordingService extends ServiceBase {
  constructor() {
    super(API_URL);
  }

  getAll = async (connectionId: string): Promise<IRecording[]> => {
    return await this.get(`/connections/${connectionId}/recordings`);
  };

  schedule = async (
    connectionId: string,
    request: IScheduleRecordingRequest
  ): Promise<IRecording> => {
    return await this.post(`/connections/${connectionId}/recordings`, request);
  };

  remove = async (connectionId: string, recordingId: number): Promise<void> => {
    return await this.delete(
      `/connections/${connectionId}/recordings/${recordingId}`
    );
  };
}

export default RecordingService;
