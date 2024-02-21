import axios from "axios";

export default axios.create({
  baseURL: import.meta.env.VITE_ACCESSRIGHT_API_URL,
  withCredentials: true,
});
