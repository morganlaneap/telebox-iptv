import axios from "axios";

class ServiceBase {
  baseUrl: string;

  constructor(baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  get = async <T>(path: string): Promise<T> => {
    const response = await axios.get(`${this.baseUrl}${path}`);
    return response.data as T;
  };

  post = async <T>(path: string, content: any): Promise<T> => {
    const response = await axios.post(`${this.baseUrl}${path}`, content);
    return response.data as T;
  };

  put = async <T>(path: string, content: any): Promise<T> => {
    const response = await axios.put(`${this.baseUrl}${path}`, content);
    return response.data as T;
  };

  delete = async (path: string): Promise<void> => {
    await axios.delete(`${this.baseUrl}${path}`);
  };
}

export default ServiceBase;
