import { HubConnectionBuilder, HubConnection, LogLevel } from '@microsoft/signalr';
import api from './api';

const resolveHubUrl = () => {
  const baseUrl = api.defaults.baseURL || '';
  if (!baseUrl) return '';
  return baseUrl.replace(/\/api\/?$/, '');
};

export const createNotificationHubConnection = (accessToken: string): HubConnection => {
  const hubUrl = `${resolveHubUrl()}/hubs/notifications`;

  return new HubConnectionBuilder()
    .withUrl(hubUrl, {
      accessTokenFactory: () => accessToken
    })
    .withAutomaticReconnect()
    .configureLogging(LogLevel.Warning)
    .build();
};
