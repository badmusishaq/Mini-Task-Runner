import React from "react";

export default function JobDetailsModal({ job, onClose }) {
  if (!job) return null;

  return (
    <div style={{
      position: "fixed",
      top: 0, left: 0,
      width: "100%", height: "100%",
      backgroundColor: "rgba(0,0,0,0.6)",
      display: "flex",
      alignItems: "center",
      justifyContent: "center",
      zIndex: 1000
    }}>
      <div style={{
        backgroundColor: "#fff",
        color: "#000",
        padding: "2rem",
        borderRadius: "8px",
        width: "600px",
        maxHeight: "80%",
        overflowY: "auto"
      }}>
        <h2>Job Details</h2>
        <p><strong>ID:</strong> {job.id}</p>
        <p><strong>Type:</strong> {job.type}</p>
        <p><strong>Status:</strong> {job.status}</p>
        <p><strong>Priority:</strong> {job.priority}</p>
        <p><strong>Created:</strong> {new Date(job.createdAt).toLocaleString()}</p>

        {job.error && (
          <p style={{ color: "red" }}>
            <strong>Error:</strong> {job.error}
          </p>
        )}

        <h3>Payload</h3>
        <pre style={{
          backgroundColor: "#f4f4f4",
          padding: "1rem",
          borderRadius: "4px",
          overflowX: "auto"
        }}>
          {JSON.stringify(job.payload, null, 2)}
        </pre>

        <button onClick={onClose} style={{ marginTop: "1rem" }}>
          Close
        </button>
      </div>
    </div>
  );
}
