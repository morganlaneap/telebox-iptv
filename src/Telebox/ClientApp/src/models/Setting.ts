export interface ISetting {
  name: string;
  value: string;
}

export interface IUpdateSettingRequest {
  settings: ISetting[];
}

export interface ISystemSettings {
  outputPath: string;
}
