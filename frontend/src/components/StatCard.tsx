interface Props {
    title: string;
    value: string | number;
    subtitle?: string;
    colour?: "blue" | "green" | "amber" | "red";
    icon: React.ReactNode;
}

const colours = {
    blue: "bg-blue-50 dark:bg-blue-900/20 text-blue-600 dark:text-blue-400",
    green: "bg-emerald-50 dark:bg-emerald-900/20 text-emerald-600 dark:text-emerald-400",
    amber: "bg-amber-50 dark:bg-amber-900/20 text-amber-600 dark:text-amber-400",
    red: "bg-red-50 dark:bg-red-900/20 text-red-600 dark:text-red-400",
};

export default function StatCard({
    title, value, subtitle, colour = "blue", icon
}: Props) {
    return (
        <div className="card flex items-center gap-4">
            <div className={`p-3 rounded-xl ${colours[colour]}`}>
                {icon}
            </div>
            <div>
                <p className="text-sm text-gray-500 dark:text-gray-400">{title}</p>
                <p className="text-2xl font-bold text-gray-900 dark:text-gray-100">
                    {value}
                </p>
                {subtitle && (
                    <p className="text-xs text-gray-400 dark:text-gray-500 mt-0.5">
                        {subtitle}
                    </p>
                )}
            </div>
        </div>
    );
}