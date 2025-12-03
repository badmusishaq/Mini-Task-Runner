import React, { useState } from "react";
import { enqueueJob } from "../api/jobs";

const templates = {
  Email: {
    to: "user@example.com",
    subject: "Welcome to Mini Task Runner",
    body: "This is a test email job."
  },
  SMS: {
    to: "+250788123456",
    message: "This is a test SMS job."
  },
  Webhook: {
    url: "https://example.com/webhook",
    method: "POST",
    headers: { Authorization: "Bearer test-token" },
    body: { event: "JobCreated", jobId: "12345" }
  },
  ReportGeneration: {
    reportName: "Monthly Performance",
    format: "PDF",
    filters: { month: "November", year: 2025 }
  }
};

export default function AddJobForm({ onJobAdded }) {
  const [isOpen, setIsOpen] = useState(false);
  const [type, setType] = useState("Email");
  const [priority, setPriority] = useState(1);
  const [payload, setPayload] = useState(JSON.stringify(templates.Email, null, 2));

  const handleTypeChange = (newType) => {
    setType(newType);
    setPayload(JSON.stringify(templates[newType], null, 2));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const parsedPayload = JSON.parse(payload);
      await enqueueJob({ type, priority, payload: parsedPayload });
      onJobAdded();
      setIsOpen(false); // collapse after submission
    } catch (err) {
      alert("Invalid payload JSON or error submitting job");
    }
  };

  return (
    <div style={{ marginBottom: "1rem" }}>
      <h2
        onClick={() => setIsOpen(!isOpen)}
        style={{
          cursor: "pointer",
          display: "flex",
          alignItems: "center",
          gap: "0.5rem"
        }}
      >
        <span>Add Job</span>
        <span
          style={{
            transform: isOpen ? "rotate(90deg)" : "rotate(0deg)",
            transition: "transform 0.3s"
          }}
        >
          ▼
        </span>
      </h2>

      <div
        style={{
          maxHeight: isOpen ? "600px" : "0",
          overflow: "hidden",
          transition: "max-height 0.5s ease"
        }}
      >
        <form onSubmit={handleSubmit} style={{ marginTop: "1rem" }}>
          <label>
            Type:
            <select value={type} onChange={(e) => handleTypeChange(e.target.value)}>
              <option>Email</option>
              <option>SMS</option>
              <option>Webhook</option>
              <option>ReportGeneration</option>
            </select>
          </label>
          <br />

          <label>
            Priority:
            <input
              type="number"
              value={priority}
              onChange={(e) => setPriority(Number(e.target.value))}
              min="1"
              max="5"
            />
          </label>
          <br />

          <label>
            Payload (JSON):
            <textarea
              value={payload}
              onChange={(e) => setPayload(e.target.value)}
              rows={8}
              cols={60}
            />
          </label>
          <br />

          <button type="submit">Enqueue Job</button>
        </form>
      </div>
    </div>
  );
}
