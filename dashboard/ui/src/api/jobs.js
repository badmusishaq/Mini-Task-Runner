// src/api/jobs.js
import axios from "axios";

// Use DashboardController for stats and job listing
const DASHBOARD_API = "http://localhost:5126/api/dashboard";

// Use JobsController for enqueue and job lifecycle
const JOBS_API = "http://localhost:5126/api/jobs";

export const getJobs = (params) =>
  axios.get(`${DASHBOARD_API}/jobs`, { params });

export const getJob = (id) =>
  axios.get(`${DASHBOARD_API}/jobs/${id}`);

export const retryJob = (id) =>
  axios.post(`${DASHBOARD_API}/jobs/${id}/retry`);

export const getStats = () =>
  axios.get(`${DASHBOARD_API}/stats`);

export const enqueueJob = (job) =>
  axios.post(`${JOBS_API}/enqueue`, job);
