import type { RiskLevel } from "../types";

interface Props {
    risk: RiskLevel;
    size?: "sm" | "md";
}

export default function RiskBadge({ risk, size = "md" }: Props) {
    const base = size === "sm"
        ? "px-2 py-0.5 text-xs"
        : "px-3 py-1 text-sm";

    const colours: Record<RiskLevel, string> = {
        Low: "risk-badge-low",
        Medium: "risk-badge-medium",
        High: "risk-badge-high",
    };

    return (
        <span className={`${base} ${colours[risk]} font-medium rounded-full`}>
            {risk} Risk
        </span>
    );
}