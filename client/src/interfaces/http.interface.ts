import { AxiosResponse } from "axios";

export interface IHttp<T> extends AxiosResponse {
  data: T;
}

export type IHttpResponse<T> = IHttp<T> | null;
