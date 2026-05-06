import axios from 'axios';

const api = axios.create({
  baseURL: '/api',
  withCredentials: true, // send the auth_token cookie with every request
});

export default api;
