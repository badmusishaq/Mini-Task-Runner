// src/components/JobFormBase.jsx
import React, { useState } from "react";
import { enqueueJob } from "../api/jobs";

const templates = {
  Email: { to: "user@example.com", subject: "Welcome", body: "Test email job." },
  SMS: { to: "+250788123456", message: "Test SMS job." },
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

export default function JobFormBase({ onJobAdded, onClose }) {
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
      if (onClose) onClose(); // close collapsible or modal if provided
    } catch {
      alert("Invalid payload JSON or error submitting job");
    }
  };

  return (
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
  );
}
