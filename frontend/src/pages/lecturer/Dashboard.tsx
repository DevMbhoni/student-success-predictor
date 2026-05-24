import { useEffect, useState } from "react";
import {
    Users, AlertTriangle, TrendingUp, BookOpen, RefreshCw
} from "lucide-react";
import {
    BarChart, Bar, XAxis, YAxis, Tooltip,
    CartesianGrid, ResponsiveContainer, Legend,
    PieChart, Pie, Cell
} from "recharts";
import Navbar from "../../components/Navbar";
import StatCard from "../../components/StatCard";
import RiskBadge from "../../components/RiskBadge";
import api from "../../services/api";
import type { EnrollmentDto, PredictionResultDto, ModuleDto, RiskLevel } from "../../types";

export default function LecturerDashboard() {
    const [modules, setModules] = useState<ModuleDto[]>([]);
    const [selectedModule, setSelectedModule] = useState<string>("");
    const [enrollments, setEnrollments] = useState<EnrollmentDto[]>([]);
    const [predictions, setPredictions] = useState<Record<string, PredictionResultDto>>({});
    const [loading, setLoading] = useState(false);
    const [predicting, setPredicting] = useState<string | null>(null);

    useEffect(() => {
        api.get<ModuleDto[]>("/api/modules").then(r => {
            setModules(r.data);
            if (r.data.length > 0) setSelectedModule(r.data[0].id);
        });
    }, []);

    useEffect(() => {
        if (!selectedModule) return;
        setLoading(true);
        api.get<EnrollmentDto[]>(`/api/enrollments/module/${selectedModule}`)
            .then(r => setEnrollments(r.data))
            .finally(() => setLoading(false));
    }, [selectedModule]);

    const handlePredict = async (enrollmentId: string) => {
        setPredicting(enrollmentId);
        try {
            const result = await api.post<PredictionResultDto>("/api/predictions", {
                enrollmentId
            });
            setPredictions(prev => ({ ...prev, [enrollmentId]: result.data }));
        } catch {
            alert("Prediction failed. Make sure the ML service is running.");
        } finally {
            setPredicting(null);
        }
    };

    const handlePredictAll = async () => {
        for (const enrollment of enrollments) {
            if (!predictions[enrollment.id]) {
                await handlePredict(enrollment.id);
            }
        }
    };

    const predictionList = Object.values(predictions);
    const highRisk = predictionList.filter(p => p.riskLevel === "High").length;
    const mediumRisk = predictionList.filter(p => p.riskLevel === "Medium").length;
    const lowRisk = predictionList.filter(p => p.riskLevel === "Low").length;

    const riskData = [
        { name: "Low Risk", value: lowRisk, color: "#10b981" },
        { name: "Medium Risk", value: mediumRisk, color: "#f59e0b" },
        { name: "High Risk", value: highRisk, color: "#ef4444" },
    ].filter(d => d.value > 0);

    const performanceData = enrollments.map(e => ({
        name: e.studentNumber,
        Attendance: e.attendancePercentage,
        Tests: e.testAverage,
        Assignments: e.assignmentAverage,
    }));

    return (
        <div className="min-h-screen bg-gray-50 dark:bg-gray-950">
            <Navbar />

            <main className="max-w-7xl mx-auto px-4 sm:px-6 py-8">
                <div className="flex items-center justify-between mb-8">
                    <div>
                        <h1 className="text-2xl font-bold text-gray-900 dark:text-white">
                            Lecturer Dashboard
                        </h1>
                        <p className="text-gray-500 dark:text-gray-400 mt-1">
                            Monitor student performance and run predictions
                        </p>
                    </div>

                    <select
                        value={selectedModule}
                        onChange={e => setSelectedModule(e.target.value)}
                        className="input w-56"
                    >
                        {modules.map(m => (
                            <option key={m.id} value={m.id}>
                                {m.code} — {m.name}
                            </option>
                        ))}
                    </select>
                </div>

                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 mb-8">
                    <StatCard
                        title="Total Students"
                        value={enrollments.length}
                        icon={<Users className="w-5 h-5" />}
                        colour="blue"
                    />
                    <StatCard
                        title="High Risk"
                        value={highRisk}
                        subtitle="need urgent intervention"
                        icon={<AlertTriangle className="w-5 h-5" />}
                        colour="red"
                    />
                    <StatCard
                        title="Medium Risk"
                        value={mediumRisk}
                        subtitle="need monitoring"
                        icon={<TrendingUp className="w-5 h-5" />}
                        colour="amber"
                    />
                    <StatCard
                        title="Predictions Run"
                        value={predictionList.length}
                        subtitle={`of ${enrollments.length} students`}
                        icon={<BookOpen className="w-5 h-5" />}
                        colour="green"
                    />
                </div>

                <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 mb-6">
                    <div className="card lg:col-span-2">
                        <h2 className="text-base font-semibold text-gray-900 dark:text-white mb-4">
                            Class Performance Overview
                        </h2>
                        {performanceData.length > 0 ? (
                            <ResponsiveContainer width="100%" height={260}>
                                <BarChart data={performanceData}>
                                    <CartesianGrid strokeDasharray="3 3" stroke="#e5e7eb" />
                                    <XAxis dataKey="name" tick={{ fontSize: 11 }} />
                                    <YAxis domain={[0, 100]} tick={{ fontSize: 11 }} />
                                    <Tooltip />
                                    <Legend />
                                    <Bar dataKey="Attendance" fill="#6366f1" radius={[4, 4, 0, 0]} />
                                    <Bar dataKey="Tests" fill="#f59e0b" radius={[4, 4, 0, 0]} />
                                    <Bar dataKey="Assignments" fill="#10b981" radius={[4, 4, 0, 0]} />
                                </BarChart>
                            </ResponsiveContainer>
                        ) : (
                            <p className="text-gray-400 text-sm">No enrollments for this module.</p>
                        )}
                    </div>

                    <div className="card flex flex-col items-center justify-center">
                        <h2 className="text-base font-semibold text-gray-900 dark:text-white mb-4">
                            Risk Distribution
                        </h2>
                        {riskData.length > 0 ? (
                            <>
                                <ResponsiveContainer width="100%" height={180}>
                                    <PieChart>
                                        <Pie
                                            data={riskData}
                                            cx="50%" cy="50%"
                                            innerRadius={50} outerRadius={80}
                                            dataKey="value"
                                        >
                                            {riskData.map((entry, i) => (
                                                <Cell key={i} fill={entry.color} />
                                            ))}
                                        </Pie>
                                        <Tooltip />
                                    </PieChart>
                                </ResponsiveContainer>
                                <div className="flex gap-3 mt-2 flex-wrap justify-center">
                                    {riskData.map(d => (
                                        <div key={d.name} className="flex items-center gap-1.5 text-xs text-gray-600 dark:text-gray-400">
                                            <span className="w-2.5 h-2.5 rounded-full" style={{ background: d.color }} />
                                            {d.name} ({d.value})
                                        </div>
                                    ))}
                                </div>
                            </>
                        ) : (
                            <p className="text-gray-400 text-sm text-center">
                                Run predictions to see risk distribution.
                            </p>
                        )}
                    </div>
                </div>

                <div className="card">
                    <div className="flex items-center justify-between mb-4">
                        <h2 className="text-base font-semibold text-gray-900 dark:text-white">
                            Student At-Risk Analysis
                        </h2>
                        <button
                            onClick={handlePredictAll}
                            disabled={loading || enrollments.length === 0}
                            className="btn-primary flex items-center gap-2 text-sm"
                        >
                            <RefreshCw className="w-4 h-4" />
                            Predict All
                        </button>
                    </div>

                    <div className="overflow-x-auto">
                        <table className="w-full text-sm">
                            <thead>
                                <tr className="border-b border-gray-100 dark:border-gray-800">
                                    {["Student", "Attendance", "Tests", "Assignments", "LMS", "Risk", "Pass Prob", "Action"].map(h => (
                                        <th key={h} className="text-left py-2 px-3 text-gray-500 dark:text-gray-400 font-medium">
                                            {h}
                                        </th>
                                    ))}
                                </tr>
                            </thead>
                            <tbody>
                                {loading ? (
                                    <tr>
                                        <td colSpan={8} className="py-8 text-center text-gray-400">
                                            Loading students...
                                        </td>
                                    </tr>
                                ) : enrollments.map(e => {
                                    const pred = predictions[e.id];
                                    return (
                                        <tr key={e.id} className="border-b border-gray-50 dark:border-gray-800/50 hover:bg-gray-50 dark:hover:bg-gray-800/30">
                                            <td className="py-3 px-3">
                                                <p className="font-medium text-gray-900 dark:text-white">{e.studentName}</p>
                                                <p className="text-xs text-gray-400">{e.studentNumber}</p>
                                            </td>
                                            <td className="py-3 px-3">
                                                <span className={e.attendancePercentage < 60 ? "text-red-500 font-medium" : "text-gray-700 dark:text-gray-300"}>
                                                    {e.attendancePercentage.toFixed(1)}%
                                                </span>
                                            </td>
                                            <td className="py-3 px-3">
                                                <span className={e.testAverage < 50 ? "text-red-500 font-medium" : "text-gray-700 dark:text-gray-300"}>
                                                    {e.testAverage.toFixed(1)}%
                                                </span>
                                            </td>
                                            <td className="py-3 px-3 text-gray-700 dark:text-gray-300">
                                                {e.assignmentAverage.toFixed(1)}%
                                            </td>
                                            <td className="py-3 px-3 text-gray-700 dark:text-gray-300">
                                                {e.lmsLoginCount}
                                            </td>
                                            <td className="py-3 px-3">
                                                {pred
                                                    ? <RiskBadge risk={pred.riskLevel as RiskLevel} size="sm" />
                                                    : <span className="text-gray-300 dark:text-gray-600 text-xs">—</span>
                                                }
                                            </td>
                                            <td className="py-3 px-3 text-gray-700 dark:text-gray-300">
                                                {pred
                                                    ? `${(pred.passProbability * 100).toFixed(1)}%`
                                                    : <span className="text-gray-300 dark:text-gray-600 text-xs">—</span>
                                                }
                                            </td>
                                            <td className="py-3 px-3">
                                                <button
                                                    onClick={() => handlePredict(e.id)}
                                                    disabled={predicting === e.id}
                                                    className="text-xs btn-secondary flex items-center gap-1"
                                                >
                                                    {predicting === e.id
                                                        ? <span className="w-3 h-3 border-2 border-gray-400 border-t-transparent rounded-full animate-spin" />
                                                        : <RefreshCw className="w-3 h-3" />
                                                    }
                                                    {pred ? "Re-run" : "Predict"}
                                                </button>
                                            </td>
                                        </tr>
                                    );
                                })}
                            </tbody>
                        </table>
                    </div>

                    {Object.values(predictions).filter(p => p.riskLevel === "High").length > 0 && (
                        <div className="mt-6 border-t border-gray-100 dark:border-gray-800 pt-4">
                            <h3 className="text-sm font-semibold text-gray-900 dark:text-white mb-3">
                                🚨 Urgent Interventions Required
                            </h3>
                            <div className="space-y-2">
                                {Object.entries(predictions)
                                    .filter(([, p]) => p.riskLevel === "High")
                                    .map(([enrollmentId, p]) => {
                                        const enrollment = enrollments.find(e => e.id === enrollmentId);
                                        return (
                                            <div key={enrollmentId} className="bg-red-50 dark:bg-red-900/10 border border-red-100 dark:border-red-900/30 rounded-lg p-3">
                                                <p className="text-sm font-medium text-red-700 dark:text-red-400">
                                                    {enrollment?.studentName} — {enrollment?.studentNumber}
                                                </p>
                                                <p className="text-xs text-red-600 dark:text-red-500 mt-1">
                                                    {p.recommendation}
                                                </p>
                                            </div>
                                        );
                                    })}
                            </div>
                        </div>
                    )}
                </div>
            </main>
        </div>
    );
}