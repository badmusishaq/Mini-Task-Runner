/*import { useEffect, useState } from "react";
import { getJobs } from "./api/jobs";

const statusMap = {
  0: "Pending",
  1: "Processing",
  2: "Succeeded",
  3: "Failed",
  4: "DeadLettered"
};

function App() {
  const [jobs, setJobs] = useState([]);

  useEffect(() => {
    getJobs().then(res => {
      console.log(res.data); // ✅ Check the shape in console
      setJobs(res.data.jobs); // ✅ Use res.data.jobs if API returns { jobs: [...] }
    });
  }, []);

  return (
    <div style={{ padding: "2rem", fontFamily: "sans-serif", color: "#fff", backgroundColor: "#121212" }}>
      <h1>Mini Task Runner Dashboard</h1>

      {jobs.length === 0 ? (
        <p>No jobs found.</p>
      ) : (
        <ul>
          {jobs.map(job => (
            <li key={job.id}>
              {job.type} — {statusMap[job.status]} — Priority {job.priority}
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}

export default App;*/

/*import JobsPage from "./pages/JobsPage";

function App() {
  return <JobsPage />;
}

export default App;
*/

import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import JobsPage from "./pages/JobsPage";

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<JobsPage />} />
      </Routes>
    </Router>
  );
}

export default App;


