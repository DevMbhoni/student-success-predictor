import { create } from "zustand";
import { persist } from "zustand/middleware";
import type { AuthResponse, UserRole } from "../types";

interface AuthState {
    token: string | null;
    user: {
        userId: string;
        email: string;
        fullName: string;
        role: UserRole;
    } | null;
    isAuthenticated: boolean;
    setAuth: (response: AuthResponse) => void;
    logout: () => void;
}

export const useAuthStore = create<AuthState>()(
    persist(
        (set) => ({
            token: null,
            user: null,
            isAuthenticated: false,

            setAuth: (response: AuthResponse) =>
                set({
                    token: response.token,
                    user: {
                        userId: response.userId,
                        email: response.email,
                        fullName: response.fullName,
                        role: response.role,
                    },
                    isAuthenticated: true,
                }),

            logout: () =>
                set({ token: null, user: null, isAuthenticated: false }),
        }),
        { name: "auth-storage" }
    )
);