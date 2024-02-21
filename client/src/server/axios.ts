import axios from "axios";

const OfficialMemo = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
  withCredentials: true,
});

const HR = axios.create({
  baseURL: import.meta.env.VITE_HR_API,
  withCredentials: true,
});

const calendarApiClient = axios.create({
  baseURL: import.meta.env.VITE_CALENDAR_API,
  withCredentials: true,
});

const bpmApi = axios.create({
  baseURL: import.meta.env.VITE_BPM_API,
  withCredentials: true,
});

export default { HR, OfficialMemo, calendarApiClient, bpmApi };
