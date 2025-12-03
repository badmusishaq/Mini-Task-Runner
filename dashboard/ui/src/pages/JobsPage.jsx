import { useEffect, useState } from "react";
//import { getJobs } from "../api/jobs";
import { getJobs, retryJob } from "../api/jobs";
import JobDetailsModal from "../components/JobDetailsModal";
import StatsDashboard from "../components/StatsDashboard";
import AddJobForm from "../components/AddJobForm";
//import AddJobModal from "../components/AddJobModal";

const statusMap = {
  0: "Pending",
  1: "Processing",
  2: "Succeeded",
  3: "Failed",
  4: "DeadLettered"
};

const getStatusColor = (status) => {
  switch (status) {
    case 0: return "#888"; // Pending
    case 1: return "#007bff"; // Processing
    case 2: return "#28a745"; // Succeeded
    case 3: return "#dc3545"; // Failed
    case 4: return "#6c757d"; // DeadLettered
    default: return "#444";
  }
};



export default function JobsPage() {
  const [jobs, setJobs] = useState([]);
  const [page, setPage] = useState(1);
  const [pageSize] = useState(20);
  const [totalPages, setTotalPages] = useState(1);
  const [search, setSearch] = useState("");
  const [sortBy, setSortBy] = useState("createdAt");
  const [sortDir, setSortDir] = useState("desc");
  const [selectedJob, setSelectedJob] = useState(null);
  const [isAddJobOpen, setIsAddJobOpen] = useState(false);

  useEffect(() => {
    const delayDebounce = setTimeout(() => {
      loadJobs();
    }, 300); // debounce search

    return () => clearTimeout(delayDebounce);
  }, [page, search, sortBy, sortDir]);

  const loadJobs = async () => {
    const res = await getJobs({ page, pageSize, search, sortBy, sortDir });
    setJobs(res.data.jobs);
    setTotalPages(res.data.totalPages);
  };

  const toggleSort = (field) => {
    if (sortBy === field) {
      setSortDir(sortDir === "asc" ? "desc" : "asc");
    } else {
      setSortBy(field);
      setSortDir("asc");
    }
  };

  return (
    <div style={{ padding: "2rem", fontFamily: "sans-serif", backgroundColor: "#121212", color: "#fff" }}>
      <h1>Mini Task Runner Dashboard</h1>

      <input
        placeholder="Search jobs..."
        value={search}
        onChange={(e) => setSearch(e.target.value)}
        style={{ marginBottom: "1rem", padding: "0.5rem", width: "300px" }}
      />
      
      {/*Collapsible Add Job Form */}
      <AddJobForm onJobAdded={() => loadJobs()} />

      {/*Modal Add Job Button */}
      <button onClick={() => setIsAddJobOpen(true)} style={{ marginBottom: "1rem" }}>
        + Add Job (Modal)
      </button>
      {isAddJobOpen && (
        <AddJobModal onClose={() => setIsAddJobOpen(false)}>
          <AddJobForm onJobAdded={() => { loadJobs(); setIsAddJobOpen(false); }} />
        </AddJobModal>
      )}
      <table style={{ width: "100%", borderCollapse: "collapse" }}>
        <thead>
            <tr>
                <th onClick={() => toggleSort("type")}>Type</th>
                <th onClick={() => toggleSort("status")}>Status</th>
                <th onClick={() => toggleSort("priority")}>Priority</th>
                <th onClick={() => toggleSort("createdAt")}>Created</th>
                <th>Actions</th>
            </tr>
        </thead>
        <tbody>
          {jobs.map((job) => (
            <tr key={job.id} onClick={() => setSelectedJob(job)} style={{ cursor: "pointer" }}>
              <td>{job.type}</td>
              <td>
                <span style={{
                  padding: "0.25rem 0.5rem",
                  borderRadius: "4px",
                  backgroundColor: getStatusColor(job.status),
                  color: "#fff"
                }}>
                  {statusMap[job.status]}
                </span>
              </td>
              <td>{job.priority}</td>
              <td>{new Date(job.createdAt).toLocaleString()}</td>
              <td>
                {(job.status === 3 || job.status === 4) && (
                  <button onClick={(e) => { e.stopPropagation(); retryJob(job.id); }}>
                    Retry
                  </button>
                )}
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      <div style={{ marginTop: "1rem" }}>
        Page {page} of {totalPages}
        <button disabled={page === 1} onClick={() => setPage(page - 1)} style={{ marginLeft: "1rem" }}>
          Prev
        </button>
        <button disabled={page === totalPages} onClick={() => setPage(page + 1)} style={{ marginLeft: "0.5rem" }}>
          Next
        </button>
      </div>

        {selectedJob && (
        <JobDetailsModal job={selectedJob} onClose={() => setSelectedJob(null)} />
        )}

        <StatsDashboard jobs={jobs} />
    </div>
  );
}
