import React from "react";
import { Bar, Pie } from "react-chartjs-2";
import {
  Chart as ChartJS,
  Title,
  Tooltip,
  Legend,
  ArcElement,
  BarElement,
  CategoryScale,
  LinearScale
} from "chart.js";

ChartJS.register(Title, Tooltip, Legend, ArcElement, BarElement, CategoryScale, LinearScale);

export default function StatsDashboard({ jobs }) {
  if (!jobs || jobs.length === 0) return <p>No jobs to display stats.</p>;

  // Count jobs by type
  const typeCounts = jobs.reduce((acc, job) => {
    acc[job.type] = (acc[job.type] || 0) + 1;
    return acc;
  }, {});

  // Count jobs by status
  const statusCounts = jobs.reduce((acc, job) => {
    acc[job.status] = (acc[job.status] || 0) + 1;
    return acc;
  }, {});

  const statusMap = {
    0: "Pending",
    1: "Processing",
    2: "Succeeded",
    3: "Failed",
    4: "DeadLettered"
  };

  const barData = {
    labels: Object.keys(typeCounts),
    datasets: [
      {
        label: "Jobs per Type",
        data: Object.values(typeCounts),
        backgroundColor: ["#007bff", "#28a745", "#dc3545", "#6c757d"]
      }
    ]
  };

  const pieData = {
    labels: Object.keys(statusCounts).map((s) => statusMap[s]),
    datasets: [
      {
        label: "Jobs per Status",
        data: Object.values(statusCounts),
        backgroundColor: ["#888", "#007bff", "#28a745", "#dc3545", "#6c757d"]
      }
    ]
  };

  return (
    <div style={{ marginTop: "2rem" }}>
      <h2>Job Statistics</h2>
      <div style={{ display: "flex", gap: "2rem" }}>
        <div style={{ width: "50%" }}>
          <Bar data={barData} />
        </div>
        <div style={{ width: "50%" }}>
          <Pie data={pieData} />
        </div>
      </div>
    </div>
  );
}
