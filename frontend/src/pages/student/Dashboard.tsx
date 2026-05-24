import { useEffect, useState } from "react";
import { BookOpen, TrendingUp, AlertTriangle, Activity } from "lucide-react";
import {
    RadialBarChart, RadialBar, ResponsiveContainer,
    BarChart, Bar, XAxis, YAxis, Tooltip, CartesianGrid, Legend
} from "recharts";
import Navbar from "../../components/Navbar";
import StatCard from "../../components/StatCard";
import RiskBadge from "../../components/RiskBadge";
import { studentService } from "../../services/studentService";
import { useAuthStore } from "../../store/authStore";
import type { StudentDto, EnrollmentDto, PredictionResultDto, RiskLevel } from "../../types";

export default function StudentDashboard() {
    const { user } = useAuthStore();
    const [student, setStudent] = useState<StudentDto | null>(null);
    const [enrollments, setEnrollments] = useState<EnrollmentDto[]>([]);
    const [predictions, setPredictions] = useState<PredictionResultDto[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const load = async () => {
            try {
                const s = await studentService.getMe();
                setStudent(s);
                const [e, p] = await Promise.all([
                    studentService.getEnrollments(s.id),
                    studentService.getPredictions(s.id),
                ]);
                setEnrollments(e);
                setPredictions(p);
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

    const performanceData = enrollments.map((e) => ({
        name: e.moduleCode,
        Attendance: e.attendancePercentage,
        Assignments: e.assignmentAverage,
        Tests: e.testAverage,
    }));

    const latestPrediction = predictions[0];

    const gaugeData = latestPrediction
        ? [{ value: latestPrediction.passProbability * 100, fill: "#6366f1" }]
        : [];

    return (
        <div className="min-h-screen bg-gray-50 dark:bg-gray-950">
            <Navbar />

            <main className="max-w-7xl mx-auto px-4 sm:px-6 py-8">
                <div className="mb-8">
                    <h1 className="text-2xl font-bold text-gray-900 dark:text-white">
                        Welcome back, {student?.fullName.split(" ")[0]} 👋
                    </h1>
                    <p className="text-gray-500 dark:text-gray-400 mt-1">
                        {student?.programme} · Year {student?.yearOfStudy} · {student?.studentNumber}
                    </p>
                </div>

                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 mb-8">
                    <StatCard
                        title="Modules Enrolled"
                        value={enrollments.length}
                        icon={<BookOpen className="w-5 h-5" />}
                        colour="blue"
                    />
                    <StatCard
                        title="Avg Attendance"
                        value={`${(enrollments.reduce((a, e) => a + e.attendancePercentage, 0) / (enrollments.length || 1)).toFixed(1)}%`}
                        icon={<Activity className="w-5 h-5" />}
                        colour="green"
                    />
                    <StatCard
                        title="Avg Test Score"
                        value={`${(enrollments.reduce((a, e) => a + e.testAverage, 0) / (enrollments.length || 1)).toFixed(1)}%`}
                        icon={<TrendingUp className="w-5 h-5" />}
                        colour="amber"
                    />
                    <StatCard
                        title="At-Risk Modules"
                        value={predictions.filter(p => p.riskLevel === "High").length}
                        subtitle="requiring attention"
                        icon={<AlertTriangle className="w-5 h-5" />}
                        colour="red"
                    />
                </div>

                <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 mb-6">
                    <div className="card lg:col-span-2">
                        <h2 className="text-base font-semibold text-gray-900 dark:text-white mb-4">
                            Performance by Module
                        </h2>
                        <ResponsiveContainer width="100%" height={260}>
                            <BarChart data={performanceData}>
                                <CartesianGrid strokeDasharray="3 3" stroke="#e5e7eb" />
                                <XAxis dataKey="name" tick={{ fontSize: 12 }} />
                                <YAxis domain={[0, 100]} tick={{ fontSize: 12 }} />
                                <Tooltip />
                                <Legend />
                                <Bar dataKey="Attendance" fill="#6366f1" radius={[4, 4, 0, 0]} />
                                <Bar dataKey="Assignments" fill="#10b981" radius={[4, 4, 0, 0]} />
                                <Bar dataKey="Tests" fill="#f59e0b" radius={[4, 4, 0, 0]} />
                            </BarChart>
                        </ResponsiveContainer>
                    </div>

                    <div className="card flex flex-col items-center justify-center">
                        <h2 className="text-base font-semibold text-gray-900 dark:text-white mb-2">
                            Pass Probability
                        </h2>
                        {latestPrediction ? (
                            <>
                                <ResponsiveContainer width="100%" height={180}>
                                    <RadialBarChart
                                        cx="50%" cy="50%"
                                        innerRadius="60%" outerRadius="90%"
                                        startAngle={180} endAngle={0}
                                        data={[{ value: 100, fill: "#e5e7eb" }, ...gaugeData]}
                                    >
                                        <RadialBar dataKey="value" cornerRadius={8} />
                                    </RadialBarChart>
                                </ResponsiveContainer>
                                <p className="text-3xl font-bold text-primary-600 -mt-8">
                                    {(latestPrediction.passProbability * 100).toFixed(1)}%
                                </p>
                                <p className="text-sm text-gray-500 mt-1">likelihood of passing</p>
                                <div className="mt-3">
                                    <RiskBadge risk={latestPrediction.riskLevel as RiskLevel} />
                                </div>
                            </>
                        ) : (
                            <p className="text-gray-400 text-sm text-center">
                                No predictions yet. Ask your lecturer to run a prediction.
                            </p>
                        )}
                    </div>
                </div>

                {predictions.filter(p => p.riskLevel !== "Low").length > 0 && (
                    <div className="card border-l-4 border-amber-400">
                        <h2 className="text-base font-semibold text-gray-900 dark:text-white mb-3">
                            Recommendations
                        </h2>
                        <div className="space-y-3">
                            {predictions
                                .filter(p => p.riskLevel !== "Low")
                                .map(p => (
                                    <div key={p.id} className="flex gap-3">
                                        <RiskBadge risk={p.riskLevel as RiskLevel} size="sm" />
                                        <p className="text-sm text-gray-600 dark:text-gray-400">
                                            <span className="font-medium">{p.moduleCode}:</span>{" "}
                                            {p.recommendation}
                                        </p>
                                    </div>
                                ))}
                        </div>
                    </div>
                )}

                <div className="card mt-6">
                    <h2 className="text-base font-semibold text-gray-900 dark:text-white mb-4">
                        Module Enrollments
                    </h2>
                    <div className="overflow-x-auto">
                        <table className="w-full text-sm">
                            <thead>
                                <tr className="border-b border-gray-100 dark:border-gray-800">
                                    {["Module", "Attendance", "Assignments", "Tests", "LMS Logins", "Status"].map(h => (
                                        <th key={h} className="text-left py-2 px-3 text-gray-500 dark:text-gray-400 font-medium">
                                            {h}
                                        </th>
                                    ))}
                                </tr>
                            </thead>
                            <tbody>
                                {enrollments.map(e => (
                                    <tr key={e.id} className="border-b border-gray-50 dark:border-gray-800/50 hover:bg-gray-50 dark:hover:bg-gray-800/30">
                                        <td className="py-3 px-3 font-medium text-gray-900 dark:text-white">
                                            {e.moduleCode}
                                            <span className="block text-xs text-gray-400 font-normal">{e.moduleName}</span>
                                        </td>
                                        <td className="py-3 px-3">
                                            <span className={e.attendancePercentage < 60 ? "text-red-500 font-medium" : "text-gray-700 dark:text-gray-300"}>
                                                {e.attendancePercentage.toFixed(1)}%
                                            </span>
                                        </td>
                                        <td className="py-3 px-3 text-gray-700 dark:text-gray-300">{e.assignmentAverage.toFixed(1)}%</td>
                                        <td className="py-3 px-3 text-gray-700 dark:text-gray-300">{e.testAverage.toFixed(1)}%</td>
                                        <td className="py-3 px-3 text-gray-700 dark:text-gray-300">{e.lmsLoginCount}</td>
                                        <td className="py-3 px-3">
                                            <span className="px-2 py-0.5 bg-emerald-100 text-emerald-700 dark:bg-emerald-900/30 dark:text-emerald-400 text-xs rounded-full">
                                                {e.status}
                                            </span>
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                </div>
            </main>
        </div>
    );
}
