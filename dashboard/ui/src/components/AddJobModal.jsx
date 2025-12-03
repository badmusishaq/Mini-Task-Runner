// src/components/AddJobModal.jsx
import React from "react";
import JobFormBase from "./JobFormBase";

export default function AddJobModal({ onJobAdded, onClose }) {
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
        <h2>Add Job (Modal)</h2>
        <JobFormBase onJobAdded={onJobAdded} onClose={onClose} />
        <button onClick={onClose} style={{ marginTop: "1rem" }}>Close</button>
      </div>
    </div>
  );
}
