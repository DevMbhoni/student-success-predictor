import pandas as pd
import numpy as np

np.random.seed(42) 
NUM_STUDENTS = 2000

def generate_dataset(n=NUM_STUDENTS):
    data = []

    for i in range(n):
        profile = np.random.choice(
            ["low_risk", "medium_risk", "high_risk"],
            p=[0.50, 0.30, 0.20]
        )

        if profile == "low_risk":
            attendance     = np.random.normal(82, 8)
            assignment_avg = np.random.normal(74, 8)
            test_avg       = np.random.normal(72, 9)
            lms_logins     = int(np.random.normal(65, 15))

        elif profile == "medium_risk":
            attendance     = np.random.normal(65, 10)
            assignment_avg = np.random.normal(58, 10)
            test_avg       = np.random.normal(55, 11)
            lms_logins     = int(np.random.normal(35, 12))

        else:  
            attendance     = np.random.normal(42, 12)
            assignment_avg = np.random.normal(40, 12)
            test_avg       = np.random.normal(37, 13)
            lms_logins     = int(np.random.normal(14, 8))

        attendance     = np.clip(attendance, 0, 100)
        assignment_avg = np.clip(assignment_avg, 0, 100)
        test_avg       = np.clip(test_avg, 0, 100)
        lms_logins     = max(0, lms_logins)

        score = (
            attendance     * 0.30 +
            assignment_avg * 0.35 +
            test_avg       * 0.35
        )

        noise = np.random.normal(0, 3)
        outcome = 1 if (score + noise) >= 50 else 0 

        if attendance >= 75 and test_avg >= 60:
            risk_level = "Low"
        elif attendance >= 55 and test_avg >= 45:
            risk_level = "Medium"
        else:
            risk_level = "High"

        data.append({
            "attendance_percentage": round(attendance, 2),
            "assignment_average":    round(assignment_avg, 2),
            "test_average":          round(test_avg, 2),
            "lms_login_count":       lms_logins,
            "risk_level":            risk_level,
            "outcome":               outcome  
        })

    df = pd.DataFrame(data)
    df.to_csv("../data/student_performance.csv", index=False)
    print(f"Dataset generated: {len(df)} rows")
    print(f"\nOutcome distribution:\n{df['outcome'].value_counts()}")
    print(f"\nRisk distribution:\n{df['risk_level'].value_counts()}")
    return df

if __name__ == "__main__":
    generate_dataset()