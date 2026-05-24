import { useEffect, useState } from "react";
import {
    Users, BookOpen, AlertTriangle, TrendingUp
} from "lucide-react";
import {
    BarChart, Bar, XAxis, YAxis, Tooltip, CartesianGrid,
    ResponsiveContainer, PieChart, Pie, Cell,
    LineChart, Line, Legend
} from "recharts";
import Navbar from "../../components/Navbar";
import StatCard from "../../components/StatCard";
import RiskBadge from "../../components/RiskBadge";
import api from "../../services/api";
import type { StudentDto, ModuleDto, PredictionResultDto, RiskLevel } from "../../types";

export default function AdminDashboard() {
    const [students, setStudents] = useState<StudentDto[]>([]);
    const [modules, setModules] = useState<ModuleDto[]>([]);
    const [predictions, setPredictions] = useState<PredictionResultDto[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const load = async () => {
            try {
                const [s, m, p] = await Promise.all([
                    api.get<StudentDto[]>("/api/students"),
                    api.get<ModuleDto[]>("/api/modules"),
                    api.get<PredictionResultDto[]>("/api/predictions/latest"),
                ]);
                setStudents(s.data);
                setModules(m.data);
                setPredictions(p.data);
            } finally {
                setLoading(false);
            }
        };
        load();
    }, []);

    if (loading) {
        return (
            <div className="min-h-screen flex items-center justify-center">
                <div className="w-8 h-8 border-4 border-primary-600 border-t-transparent rounded-full animate-spin" />
            </div>
        );
    }

    const highRisk = predictions.filter(p => p.riskLevel === "High").length;
    const mediumRisk = predictions.filter(p => p.riskLevel === "Medium").length;
    const lowRisk = predictions.filter(p => p.riskLevel === "Low").length;
    const avgPass = predictions.length > 0
        ? (predictions.reduce((a, p) => a + p.passProbability, 0) / predictions.length * 100).toFixed(1)
        : "—";

    const riskPieData = [
        { name: "Low", value: lowRisk, color: "#10b981" },
        { name: "Medium", value: mediumRisk, color: "#f59e0b" },
        { name: "High", value: highRisk, color: "#ef4444" },
    ].filter(d => d.value > 0);

    const moduleData = modules.map(m => ({
        name: m.code,
        Students: m.enrolledStudentCount,
    }));

    const probBands = [
        { band: "0–20%", count: predictions.filter(p => p.passProbability < 0.2).length },
        { band: "20–40%", count: predictions.filter(p => p.passProbability >= 0.2 && p.passProbability < 0.4).length },
        { band: "40–60%", count: predictions.filter(p => p.passProbability >= 0.4 && p.passProbability < 0.6).length },
        { band: "60–80%", count: predictions.filter(p => p.passProbability >= 0.6 && p.passProbability < 0.8).length },
        { band: "80–100%", count: predictions.filter(p => p.passProbability >= 0.8).length },
    ];

    return (
        <div className="min-h-screen bg-gray-50 dark:bg-gray-950">
            <Navbar />

            <main className="max-w-7xl mx-auto px-4 sm:px-6 py-8">
                <div className="mb-8">
                    <h1 className="text-2xl font-bold text-gray-900 dark:text-white">
                        Admin Analytics Dashboard
                    </h1>
                    <p className="text-gray-500 dark:text-gray-400 mt-1">
                        University-wide performance overview
                    </p>
                </div>

                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 mb-8">
                    <StatCard
                        title="Total Students"
                        value={students.length}
                        icon={<Users className="w-5 h-5" />}
                        colour="blue"
                    />
                    <StatCard
                        title="Total Modules"
                        value={modules.length}
                        icon={<BookOpen className="w-5 h-5" />}
                        colour="green"
                    />
                    <StatCard
                        title="High Risk Students"
                        value={highRisk}
                        subtitle="need urgent action"
                        icon={<AlertTriangle className="w-5 h-5" />}
                        colour="red"
                    />
                    <StatCard
                        title="Avg Pass Probability"
                        value={`${avgPass}%`}
                        subtitle="across all predictions"
                        icon={<TrendingUp className="w-5 h-5" />}
                        colour="amber"
                    />
                </div>

                <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 mb-6">
                    <div className="card flex flex-col items-center">
                        <h2 className="text-base font-semibold text-gray-900 dark:text-white mb-4 self-start">
                            Risk Distribution
                        </h2>
                        {riskPieData.length > 0 ? (
                            <>
                                <ResponsiveContainer width="100%" height={200}>
                                    <PieChart>
                                        <Pie
                                            data={riskPieData}
                                            cx="50%" cy="50%"
                                            outerRadius={80}
                                            dataKey="value"
                                            label={({ name, value }) => `${name}: ${value}`}
                                        >
                                            {riskPieData.map((entry, i) => (
                                                <Cell key={i} fill={entry.color} />
                                            ))}
                                        </Pie>
                                        <Tooltip />
                                    </PieChart>
                                </ResponsiveContainer>
                                <div className="flex gap-4 mt-2 flex-wrap justify-center">
                                    {riskPieData.map(d => (
                                        <div key={d.name} className="flex items-center gap-1.5 text-xs text-gray-600 dark:text-gray-400">
                                            <span className="w-2.5 h-2.5 rounded-full" style={{ background: d.color }} />
                                            {d.name} ({d.value})
                                        </div>
                                    ))}
                                </div>
                            </>
                        ) : (
                            <p className="text-gray-400 text-sm">No predictions yet.</p>
                        )}
                    </div>

                    <div className="card lg:col-span-2">
                        <h2 className="text-base font-semibold text-gray-900 dark:text-white mb-4">
                            Enrollment by Module
                        </h2>
                        <ResponsiveContainer width="100%" height={220}>
                            <BarChart data={moduleData}>
                                <CartesianGrid strokeDasharray="3 3" stroke="#e5e7eb" />
                                <XAxis dataKey="name" tick={{ fontSize: 12 }} />
                                <YAxis tick={{ fontSize: 12 }} />
                                <Tooltip />
                                <Bar dataKey="Students" fill="#6366f1" radius={[4, 4, 0, 0]} />
                            </BarChart>
                        </ResponsiveContainer>
                    </div>
                </div>

                <div className="card mb-6">
                    <h2 className="text-base font-semibold text-gray-900 dark:text-white mb-4">
                        Pass Probability Distribution
                    </h2>
                    <ResponsiveContainer width="100%" height={220}>
                        <BarChart data={probBands}>
                            <CartesianGrid strokeDasharray="3 3" stroke="#e5e7eb" />
                            <XAxis dataKey="band" tick={{ fontSize: 12 }} />
                            <YAxis tick={{ fontSize: 12 }} />
                            <Tooltip />
                            <Bar dataKey="count" name="Students" radius={[4, 4, 0, 0]}>
                                {probBands.map((entry, i) => (
                                    <Cell
                                        key={i}
                                        fill={
                                            entry.band.startsWith("0") || entry.band.startsWith("20")
                                                ? "#ef4444"
                                                : entry.band.startsWith("40")
                                                    ? "#f59e0b"
                                                    : "#10b981"
                                        }
                                    />
                                ))}
                            </Bar>
                        </BarChart>
                    </ResponsiveContainer>
                </div>

                <div className="card">
                    <h2 className="text-base font-semibold text-gray-900 dark:text-white mb-4">
                        All Student Risk Overview
                    </h2>
                    <div className="overflow-x-auto">
                        <table className="w-full text-sm">
                            <thead>
                                <tr className="border-b border-gray-100 dark:border-gray-800">
                                    {["Student", "Student No.", "Pass Prob.", "Fail Prob.", "Risk", "Recommendation"].map(h => (
                                        <th key={h} className="text-left py-2 px-3 text-gray-500 dark:text-gray-400 font-medium">
                                            {h}
                                        </th>
                                    ))}
                                </tr>
                            </thead>
                            <tbody>
                                {predictions
                                    .sort((a, b) => b.failProbability - a.failProbability)
                                    .map(p => (
                                        <tr key={p.id} className="border-b border-gray-50 dark:border-gray-800/50 hover:bg-gray-50 dark:hover:bg-gray-800/30">
                                            <td className="py-3 px-3 font-medium text-gray-900 dark:text-white">
                                                {p.studentName}
                                            </td>
                                            <td className="py-3 px-3 text-gray-500 dark:text-gray-400">
                                                {p.studentNumber}
                                            </td>
                                            <td className="py-3 px-3">
                                                <div className="flex items-center gap-2">
                                                    <div className="w-24 bg-gray-100 dark:bg-gray-800 rounded-full h-1.5">
                                                        <div
                                                            className="bg-primary-600 h-1.5 rounded-full"
                                                            style={{ width: `${p.passProbability * 100}%` }}
                                                        />
                                                    </div>
                                                    <span className="text-gray-700 dark:text-gray-300">
                                                        {(p.passProbability * 100).toFixed(1)}%
                                                    </span>
                                                </div>
                                            </td>
                                            <td className="py-3 px-3 text-gray-700 dark:text-gray-300">
                                                {(p.failProbability * 100).toFixed(1)}%
                                            </td>
                                            <td className="py-3 px-3">
                                                <RiskBadge risk={p.riskLevel as RiskLevel} size="sm" />
                                            </td>
                                            <td className="py-3 px-3 text-gray-500 dark:text-gray-400 max-w-xs truncate">
                                                {p.recommendation}
                                            </td>
                                        </tr>
                                    ))}
                                {predictions.length === 0 && (
                                    <tr>
                                        <td colSpan={6} className="py-8 text-center text-gray-400">
                                            No predictions yet. Ask lecturers to run predictions for their modules.
                                        </td>
                                    </tr>
                                )}
                            </tbody>
                        </table>
                    </div>
                </div>
            </main>
        </div>
    );
}