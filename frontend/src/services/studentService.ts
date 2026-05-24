import api from "./api";
import type{ StudentDto, EnrollmentDto, PredictionResultDto } from "../types";

export const studentService = {
    getAll: async (): Promise<StudentDto[]> => {
        const response = await api.get<StudentDto[]>("/api/students");
        return response.data;
    },

    getMe: async (): Promise<StudentDto> => {
        const response = await api.get<StudentDto>("/api/students/me");
        return response.data;
    },

    getEnrollments: async (studentId: string): Promise<EnrollmentDto[]> => {
        const response = await api.get<EnrollmentDto[]>(
            `/api/enrollments/student/${studentId}`
        );
        return response.data;
    },

    getPredictions: async (studentId: string): Promise<PredictionResultDto[]> => {
        const response = await api.get<PredictionResultDto[]>(
            `/api/predictions/student/${studentId}`
        );
        return response.data;
    },
};