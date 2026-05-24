import { useEffect, useState } from "react";
import {
    Users, AlertTriangle, CheckCircle,
    Search, Filter, PhoneCall
} from "lucide-react";
import {
    BarChart, Bar, XAxis, YAxis, Tooltip,
    CartesianGrid, ResponsiveContainer, Cell
} from "recharts";
import Navbar from "../../components/Navbar";
import StatCard from "../../components/StatCard";
import RiskBadge from "../../components/RiskBadge";
import api from "../../services/api";
import type { PredictionResultDto, StudentDto, RiskLevel } from "../../types";

export default function AdvisorDashboard() {
    const [predictions, setPredictions] = useState<PredictionResultDto[]>([]);
    const [students, setStudents] = useState<StudentDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [searchTerm, setSearchTerm] = useState("");
    const [riskFilter, setRiskFilter] = useState<string>("All");
    const [selectedStudent, setSelectedStudent] = useState<PredictionResultDto | null>(null);

    useEffect(() => {
        const load = async () => {
            try {
                const [p, s] = await Promise.all([
                    api.get<PredictionResultDto[]>("/api/predictions/latest"),
                    api.get<StudentDto[]>("/api/students"),
                ]);
                setPredictions(p.data);
                setStudents(s.data);
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

    const highRisk = predictions.filter(p => p.riskLevel === "High");
    const mediumRisk = predictions.filter(p => p.riskLevel === "Medium");
    const lowRisk = predictions.filter(p => p.riskLevel === "Low");

    const filtered = predictions
        .filter(p => {
            const matchesSearch =
                p.studentName.toLowerCase().includes(searchTerm.toLowerCase()) ||
                p.studentNumber.toLowerCase().includes(searchTerm.toLowerCase());
            const matchesRisk =
                riskFilter === "All" || p.riskLevel === riskFilter;
            return matchesSearch && matchesRisk;
        })
        .sort((a, b) => b.failProbability - a.failProbability);

    const programmeMap: Record<string, { high: number; medium: number; low: number }> = {};
    predictions.forEach(p => {
        const student = students.find(s => s.id === p.studentId);
        const prog = student?.programme ?? "Unknown";
        if (!programmeMap[prog]) programmeMap[prog] = { high: 0, medium: 0, low: 0 };
        if (p.riskLevel === "High") programmeMap[prog].high++;
        if (p.riskLevel === "Medium") programmeMap[prog].medium++;
        if (p.riskLevel === "Low") programmeMap[prog].low++;
    });
    const programmeData = Object.entries(programmeMap).map(([name, counts]) => ({
        name: name.replace("BSc ", "").replace("BCom ", ""),
        ...counts
    }));

    return (
        <div className="min-h-screen bg-gray-50 dark:bg-gray-950">
            <Navbar />

            <main className="max-w-7xl mx-auto px-4 sm:px-6 py-8">

                <div className="mb-8">
                    <h1 className="text-2xl font-bold text-gray-900 dark:text-white">
                        Academic Advisor Dashboard
                    </h1>
                    <p className="text-gray-500 dark:text-gray-400 mt-1">
                        Student intervention and support management
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
                        title="High Risk"
                        value={highRisk.length}
                        subtitle="urgent intervention needed"
                        icon={<AlertTriangle className="w-5 h-5" />}
                        colour="red"
                    />
                    <StatCard
                        title="Medium Risk"
                        value={mediumRisk.length}
                        subtitle="monitoring required"
                        icon={<PhoneCall className="w-5 h-5" />}
                        colour="amber"
                    />
                    <StatCard
                        title="Low Risk"
                        value={lowRisk.length}
                        subtitle="performing well"
                        icon={<CheckCircle className="w-5 h-5" />}
                        colour="green"
                    />
                </div>

                <div className="card mb-6">
                    <h2 className="text-base font-semibold text-gray-900 dark:text-white mb-4">
                        Risk Breakdown by Programme
                    </h2>
                    <ResponsiveContainer width="100%" height={240}>
                        <BarChart data={programmeData}>
                            <CartesianGrid strokeDasharray="3 3" stroke="#e5e7eb" />
                            <XAxis dataKey="name" tick={{ fontSize: 11 }} />
                            <YAxis tick={{ fontSize: 11 }} />
                            <Tooltip />
                            <Bar dataKey="high" name="High Risk" stackId="a" fill="#ef4444" radius={[0, 0, 0, 0]} />
                            <Bar dataKey="medium" name="Medium Risk" stackId="a" fill="#f59e0b" />
                            <Bar dataKey="low" name="Low Risk" stackId="a" fill="#10b981" radius={[4, 4, 0, 0]} />
                        </BarChart>
                    </ResponsiveContainer>
                </div>

                <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">

                    <div className="card lg:col-span-2">
                        <div className="flex flex-col sm:flex-row gap-3 mb-4">
                            <div className="relative flex-1">
                                <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
                                <input
                                    type="text"
                                    placeholder=" Search by name or student number..."
                                    value={searchTerm}
                                    onChange={e => setSearchTerm(e.target.value)}
                                    className="input pl-9"
                                />
                            </div>

                            <div className="relative">
                                <Filter className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
                                <select
                                    value={riskFilter}
                                    onChange={e => setRiskFilter(e.target.value)}
                                    className="input pl-9 w-40"
                                >
                                    <option value="All">All Risk Levels</option>
                                    <option value="High">High Risk</option>
                                    <option value="Medium">Medium Risk</option>
                                    <option value="Low">Low Risk</option>
                                </select>
                            </div>
                        </div>

                        <div className="overflow-x-auto">
                            <table className="w-full text-sm">
                                <thead>
                                    <tr className="border-b border-gray-100 dark:border-gray-800">
                                        {["Student", "Programme", "Pass Prob.", "Risk", ""].map(h => (
                                            <th key={h} className="text-left py-2 px-3 text-gray-500 dark:text-gray-400 font-medium">
                                                {h}
                                            </th>
                                        ))}
                                    </tr>
                                </thead>
                                <tbody>
                                    {filtered.length === 0 ? (
                                        <tr>
                                            <td colSpan={5} className="py-8 text-center text-gray-400">
                                                No students match your search.
                                            </td>
                                        </tr>
                                    ) : filtered.map(p => {
                                        const student = students.find(s => s.id === p.studentId);
                                        return (
                                            <tr
                                                key={p.id}
                                                onClick={() => setSelectedStudent(p)}
                                                className={`border-b border-gray-50 dark:border-gray-800/50 cursor-pointer transition-colors
                          ${selectedStudent?.id === p.id
                                                        ? "bg-primary-50 dark:bg-primary-900/20"
                                                        : "hover:bg-gray-50 dark:hover:bg-gray-800/30"
                                                    }`}
                                            >
                                                <td className="py-3 px-3">
                                                    <p className="font-medium text-gray-900 dark:text-white">
                                                        {p.studentName}
                                                    </p>
                                                    <p className="text-xs text-gray-400">{p.studentNumber}</p>
                                                </td>
                                                <td className="py-3 px-3 text-gray-500 dark:text-gray-400 text-xs">
                                                    {student?.programme ?? "—"}
                                                </td>
                                                <td className="py-3 px-3">
                                                    <div className="flex items-center gap-2">
                                                        <div className="w-16 bg-gray-100 dark:bg-gray-800 rounded-full h-1.5">
                                                            <div
                                                                className="bg-primary-600 h-1.5 rounded-full"
                                                                style={{ width: `${p.passProbability * 100}%` }}
                                                            />
                                                        </div>
                                                        <span className="text-gray-700 dark:text-gray-300 text-xs">
                                                            {(p.passProbability * 100).toFixed(1)}%
                                                        </span>
                                                    </div>
                                                </td>
                                                <td className="py-3 px-3">
                                                    <RiskBadge risk={p.riskLevel as RiskLevel} size="sm" />
                                                </td>
                                                <td className="py-3 px-3 text-primary-600 text-xs font-medium">
                                                    View →
                                                </td>
                                            </tr>
                                        );
                                    })}
                                </tbody>
                            </table>
                        </div>

                        <p className="text-xs text-gray-400 mt-3">
                            Showing {filtered.length} of {predictions.length} students
                        </p>
                    </div>

                    <div className="card">
                        {selectedStudent ? (
                            <>
                                <div className="flex items-start justify-between mb-4">
                                    <div>
                                        <h3 className="font-semibold text-gray-900 dark:text-white">
                                            {selectedStudent.studentName}
                                        </h3>
                                        <p className="text-xs text-gray-400 mt-0.5">
                                            {selectedStudent.studentNumber}
                                        </p>
                                    </div>
                                    <RiskBadge risk={selectedStudent.riskLevel as RiskLevel} />
                                </div>

                                <div className="space-y-3 mb-5">
                                    <div>
                                        <div className="flex justify-between text-xs text-gray-500 mb-1">
                                            <span>Pass Probability</span>
                                            <span className="font-medium text-emerald-600">
                                                {(selectedStudent.passProbability * 100).toFixed(1)}%
                                            </span>
                                        </div>
                                        <div className="w-full bg-gray-100 dark:bg-gray-800 rounded-full h-2">
                                            <div
                                                className="bg-emerald-500 h-2 rounded-full transition-all"
                                                style={{ width: `${selectedStudent.passProbability * 100}%` }}
                                            />
                                        </div>
                                    </div>
                                    <div>
                                        <div className="flex justify-between text-xs text-gray-500 mb-1">
                                            <span>Fail Probability</span>
                                            <span className="font-medium text-red-500">
                                                {(selectedStudent.failProbability * 100).toFixed(1)}%
                                            </span>
                                        </div>
                                        <div className="w-full bg-gray-100 dark:bg-gray-800 rounded-full h-2">
                                            <div
                                                className="bg-red-500 h-2 rounded-full transition-all"
                                                style={{ width: `${selectedStudent.failProbability * 100}%` }}
                                            />
                                        </div>
                                    </div>
                                </div>

                                <div className="grid grid-cols-2 gap-2 mb-5">
                                    {[
                                        { label: "Attendance", value: `${selectedStudent.attendancePercentage?.toFixed(1) ?? "—"}%` },
                                        { label: "Test Avg", value: `${selectedStudent.testAverage?.toFixed(1) ?? "—"}%` },
                                        { label: "Assignment", value: `${selectedStudent.assignmentAverage?.toFixed(1) ?? "—"}%` },
                                        { label: "LMS Logins", value: selectedStudent.lmsLoginCount ?? "—" },
                                    ].map(item => (
                                        <div key={item.label} className="bg-gray-50 dark:bg-gray-800 rounded-lg p-3">
                                            <p className="text-xs text-gray-400">{item.label}</p>
                                            <p className="text-base font-semibold text-gray-900 dark:text-white mt-0.5">
                                                {item.value}
                                            </p>
                                        </div>
                                    ))}
                                </div>

                                <div className={`rounded-lg p-3 text-sm
                  ${selectedStudent.riskLevel === "High"
                                        ? "bg-red-50 dark:bg-red-900/10 border border-red-100 dark:border-red-900/30"
                                        : selectedStudent.riskLevel === "Medium"
                                            ? "bg-amber-50 dark:bg-amber-900/10 border border-amber-100 dark:border-amber-900/30"
                                            : "bg-emerald-50 dark:bg-emerald-900/10 border border-emerald-100 dark:border-emerald-900/30"
                                    }`}
                                >
                                    <p className="font-medium text-gray-700 dark:text-gray-300 mb-1">
                                        Recommended Intervention
                                    </p>
                                    <p className="text-xs text-gray-600 dark:text-gray-400 leading-relaxed">
                                        {selectedStudent.recommendation}
                                    </p>
                                </div>

                                <p className="text-xs text-gray-400 mt-4">
                                    Last predicted: {new Date(selectedStudent.predictedAt).toLocaleDateString()}
                                </p>
                            </>
                        ) : (
                            <div className="flex flex-col items-center justify-center h-full min-h-64 text-center">
                                <Users className="w-10 h-10 text-gray-200 dark:text-gray-700 mb-3" />
                                <p className="text-gray-400 text-sm">
                                    Select a student from the list to view their detailed intervention plan.
                                </p>
                            </div>
                        )}
                    </div>
                </div>
            </main>
        </div>
    );
}