/*import { useState } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from '/vite.svg'
import './App.css'

function App() {
  const [count, setCount] = useState(0)

  return (
    <>
      <div>
        <a href="https://vite.dev" target="_blank">
          <img src={viteLogo} className="logo" alt="Vite logo" />
        </a>
        <a href="https://react.dev" target="_blank">
          <img src={reactLogo} className="logo react" alt="React logo" />
        </a>
      </div>
      <h1>Vite + React</h1>
      <div className="card">
        <button onClick={() => setCount((count) => count + 1)}>
          count is {count}
        </button>
        <p>
          Edit <code>src/App.jsx</code> and save to test HMR
        </p>
      </div>
      <p className="read-the-docs">
        Click on the Vite and React logos to learn more
      </p>
    </>
  )
}

export default App*/

/*import { useEffect, useState } from "react";
import { getJobs } from "./api/jobs";

function App() {
  const [jobs, setJobs] = useState([]);

  useEffect(() => {
    getJobs().then(res => setJobs(res.data.jobs));
  }, []);

  return (
    <div>
      <h1>Mini Task Runner Dashboard</h1>
      <ul>
        {jobs.map(job => (
          <li key={job.id}>
            {job.type} — {job.status} — Priority {job.priority}
          </li>
        ))}
      </ul>
    </div>
  );
}

export default App;*/

import { useEffect, useState } from "react";
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

export default App;
