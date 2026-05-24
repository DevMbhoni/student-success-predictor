import { Routes, Route, Navigate } from "react-router-dom";
import Login from "./pages/Login";
import StudentDashboard from "./pages/student/Dashboard";
import LecturerDashboard from "./pages/lecturer/Dashboard";
import AdminDashboard from "./pages/admin/Dashboard";
import ProtectedRoute from "./components/ProtectedRoute";
import AdvisorDashboard from "./pages/advisor/Dashboard";

export default function App() {
  return (
    <Routes>
      <Route path="/login" element={<Login />} />
      <Route path="/" element={<Navigate to="/login" replace />} />

      {/* Student routes */}
      <Route path="/student/dashboard" element={
        <ProtectedRoute allowedRoles={["Student"]}>
          <StudentDashboard />
        </ProtectedRoute>
      } />

      {/* Lecturer routes */}
      <Route path="/lecturer/dashboard" element={
        <ProtectedRoute allowedRoles={["Lecturer"]}>
          <LecturerDashboard />
        </ProtectedRoute>
      } />

      {/* Admin routes */}
      <Route path="/admin/dashboard" element={
        <ProtectedRoute allowedRoles={["Administrator", "AcademicAdvisor"]}>
          <AdminDashboard />
        </ProtectedRoute>
      } />

      <Route path="/advisor/dashboard" element={
        <ProtectedRoute allowedRoles={["AcademicAdvisor"]}>
          <AdvisorDashboard />
        </ProtectedRoute>
      } />
      
      <Route path="/unauthorized" element={
        <div className="min-h-screen flex items-center justify-center">
          <p className="text-gray-500">You are not authorised to view this page.</p>
        </div>
      } />
    </Routes>
  );
}