// src/components/AddJobCollapsible.jsx
import React, { useState } from "react";
import JobFormBase from "./JobFormBase";

export default function AddJobCollapsible({ onJobAdded }) {
  const [isOpen, setIsOpen] = useState(false);

  return (
    <div style={{ marginBottom: "1rem" }}>
      <h2
        onClick={() => setIsOpen(!isOpen)}
        style={{ cursor: "pointer", display: "flex", alignItems: "center", gap: "0.5rem" }}
      >
        <span>Add Job (Collapsible)</span>
        <span style={{ transform: isOpen ? "rotate(90deg)" : "rotate(0deg)", transition: "transform 0.3s" }}>
          ▼
        </span>
      </h2>

      <div style={{
        maxHeight: isOpen ? "600px" : "0",
        overflow: "hidden",
        transition: "max-height 0.5s ease"
      }}>
        {isOpen && <JobFormBase onJobAdded={onJobAdded} onClose={() => setIsOpen(false)} />}
      </div>
    </div>
  );
}
