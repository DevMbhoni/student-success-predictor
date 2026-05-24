import { useAuthStore } from "../store/authStore";
import { useNavigate } from "react-router-dom";
import { LogOut, GraduationCap } from "lucide-react";

export default function Navbar() {
    const { user, logout } = useAuthStore();
    const navigate = useNavigate();

    const handleLogout = () => {
        logout();
        navigate("/login");
    };

    return (
        <nav className="bg-white dark:bg-gray-900 border-b border-gray-100 dark:border-gray-800 px-6 py-4">
            <div className="max-w-7xl mx-auto flex items-center justify-between">
                <div className="flex items-center gap-2">
                    <GraduationCap className="w-6 h-6 text-primary-600" />
                    <span className="font-bold text-gray-900 dark:text-gray-100">
                        Student Success Predictor
                    </span>
                </div>

                <div className="flex items-center gap-4">
                    <div className="text-right">
                        <p className="text-sm font-medium text-gray-900 dark:text-gray-100">
                            {user?.fullName}
                        </p>
                        <p className="text-xs text-gray-400">{user?.role}</p>
                    </div>
                    <button
                        onClick={handleLogout}
                        className="p-2 text-gray-400 hover:text-gray-600 dark:hover:text-gray-200 transition-colors"
                    >
                        <LogOut className="w-5 h-5" />
                    </button>
                </div>
            </div>
        </nav>
    );
}