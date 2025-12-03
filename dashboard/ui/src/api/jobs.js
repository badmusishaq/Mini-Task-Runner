// src/api/jobs.js

import axios from "axios";

const API = "http://localhost:5126/api/dashboard";

export const getJobs = (params) =>
  axios.get(`${API}/jobs`, { params });

export const getJob = (id) =>
  axios.get(`${API}/jobs/${id}`);

export const retryJob = (id) =>
  axios.post(`${API}/jobs/${id}/retry`);

export const getStats = () =>
  axios.get(`${API}/stats`);

export const enqueueJob = (job) =>
  axios.post(`${API}/enqueue`, job);