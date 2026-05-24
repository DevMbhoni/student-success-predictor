export type UserRole = "Student" | "Lecturer" | "AcademicAdvisor" | "Administrator";
export type RiskLevel = "Low" | "Medium" | "High";

export interface AuthResponse {
    token: string;
    email: string;
    fullName: string;
    role: UserRole;
    userId: string;
    expiresAt: string;
}

export interface LoginRequest {
    email: string;
    password: string;
}

export interface RegisterRequest {
    firstName: string;
    lastName: string;
    email: string;
    password: string;
    role: number;
    studentNumber?: string;
    programme?: string;
    yearOfStudy?: number;
}

export interface StudentDto {
    id: string;
    userId: string;
    studentNumber: string;
    fullName: string;
    email: string;
    programme: string;
    yearOfStudy: number;
    enrolledAt: string;
}

export interface ModuleDto {
    id: string;
    code: string;
    name: string;
    credits: number;
    department: string;
    enrolledStudentCount: number;
}

export interface EnrollmentDto {
    id: string;
    studentId: string;
    studentNumber: string;
    studentName: string;
    moduleId: string;
    moduleCode: string;
    moduleName: string;
    year: number;
    semester: string;
    attendancePercentage: number;
    assignmentAverage: number;
    testAverage: number;
    lmsLoginCount: number;
    status: string;
}

export interface PredictionResultDto {
    id: string;
    studentId: string;
    studentName: string;
    studentNumber: string;
    moduleCode: string;
    moduleName: string;
    passProbability: number;
    failProbability: number;
    riskLevel: RiskLevel;
    predictedOutcome: string;
    recommendation: string;
    modelVersion: string;
    predictedAt: string;
    attendancePercentage: number;
    assignmentAverage: number;
    testAverage: number;
    lmsLoginCount: number;
}