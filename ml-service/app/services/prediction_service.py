import joblib
import numpy as np
from pathlib import Path

BASE_DIR = Path(__file__).resolve().parent.parent

class PredictionService:
    def __init__(self):
        self.lr_model = joblib.load(BASE_DIR / "models/logistic_regression.joblib")
        self.rf_model = joblib.load(BASE_DIR / "models/random_forest.joblib")
        self.dt_model = joblib.load(BASE_DIR / "models/decision_tree.joblib")
        self.scaler   = joblib.load(BASE_DIR / "models/scaler.joblib")

    def predict(self, attendance: float, assignment_avg: float,
                test_avg: float, lms_logins: int) -> dict:

        features = np.array([[attendance, assignment_avg, test_avg, lms_logins]])
        features_scaled = self.scaler.transform(features)

        lr_proba      = self.lr_model.predict_proba(features_scaled)[0]
        fail_prob_lr  = round(float(lr_proba[0]), 4)
        pass_prob_lr  = round(float(lr_proba[1]), 4)

        rf_risk_proba = self.rf_model.predict_proba(features)[0]
        rf_prediction = self.rf_model.predict(features)[0]

        dt_prediction = self.dt_model.predict(features)[0]

        if fail_prob_lr >= 0.65:
            risk_level = "High"
        elif fail_prob_lr >= 0.35:
            risk_level = "Medium"
        else:
            risk_level = "Low"

        recommendation = self._get_recommendation(
            risk_level, attendance, assignment_avg, test_avg, lms_logins
        )

        return {
            "pass_probability":    pass_prob_lr,
            "fail_probability":    fail_prob_lr,
            "risk_level":          risk_level,
            "predicted_outcome":   "Pass" if rf_prediction == 1 else "Fail",
            "recommendation":      recommendation,
            "model_version":       "1.0.0",
            "feature_breakdown": {
                "attendance_percentage": attendance,
                "assignment_average":    assignment_avg,
                "test_average":          test_avg,
                "lms_login_count":       lms_logins
            }
        }

    def _get_recommendation(
        self, risk: str, attendance: float,
        assignment_avg: float, test_avg: float, lms_logins: int
    ) -> str:
        # This is "explainable AI" — telling the user WHY the risk is high

        if risk == "Low":
            return "Student is performing well. Encourage continued engagement."

        issues = []

        if attendance < 60:
            issues.append("attendance is critically low")
        elif attendance < 75:
            issues.append("attendance needs improvement")

        if test_avg < 50:
            issues.append("test performance is below the pass mark")
        elif test_avg < 60:
            issues.append("test performance needs attention")

        if assignment_avg < 50:
            issues.append("assignment marks are below the pass mark")

        if lms_logins < 20:
            issues.append("very low engagement on the LMS platform")

        issue_text = "; ".join(issues) if issues else "overall performance is declining"

        if risk == "High":
            return (
                f"URGENT: Student is at high risk of failing. "
                f"Key concerns: {issue_text}. "
                f"Recommended actions: immediate lecturer consultation, "
                f"weekly tutoring sessions, academic advisor referral, "
                f"and structured study plan."
            )
        else: 
            return (
                f"Student is at moderate risk. "
                f"Key concerns: {issue_text}. "
                f"Recommended actions: attend extra revision sessions, "
                f"increase LMS engagement, and schedule a check-in with academic advisor."
            )