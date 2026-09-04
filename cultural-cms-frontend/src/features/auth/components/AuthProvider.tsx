import type { LoginFields } from "../schemas/auth";
import {createContext, useContext, useState} from "react";
import {jwtDecode} from "jwt-decode";
import {deleteCookie, getCookie, setCookie} from "@/shared/utils/cookies";
import {login} from "../api/auth";

const CLAIM_USER_ID = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
const CLAIM_ROLE = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";

type AuthContextProps = {
  isAuthenticated: boolean,
  accessToken: string | null,
  userId: string | null,
  userRole: string | null,
  loginUser: (fields: LoginFields) => Promise<void>;
  logoutUser: () => void;
}

type JwtPayload = {
  [CLAIM_USER_ID]?: string;
  [CLAIM_ROLE]?: string;
}

const AuthContext = createContext<AuthContextProps | undefined>(undefined);

function readPayloadFromToken(token: string | null) {
  if (!token) return { userId: null, userRole: null };
  try {
    const decoded = jwtDecode<JwtPayload>(token);
    return {
      userId: decoded[CLAIM_USER_ID] ?? null,
      userRole: decoded[CLAIM_ROLE] ?? null,
    };
  } catch {
    return { userId: null, userRole: null };
  }
}

export const AuthProvider = ({children}: {children: React.ReactNode}) => {
  const [accessToken, setAccessToken] = useState<string | null>(() =>
    getCookie("access_token") ?? null
  );

  const initialPayload = readPayloadFromToken(accessToken);
  const [userId, setUserId] = useState<string | null>(initialPayload.userId);
  const [userRole, setUserRole] = useState<string | null>(initialPayload.userRole);

  const loginUser = async (fields: LoginFields) => {
    const res = await login(fields);

    setCookie("access_token", res.token, {
      expires: 1,
      sameSite: "Lax",
      secure: false,
      path: "/"
    });

    setAccessToken(res.token);
    const newPayload = readPayloadFromToken(res.token);
    setUserId(newPayload.userId);
    setUserRole(newPayload.userRole);
  };

  const logoutUser = () => {
    deleteCookie("access_token");
    setAccessToken(null);
    setUserId(null);
    setUserRole(null);
  };

  return (
    <AuthContext.Provider
      value={{
        isAuthenticated: !!accessToken,
        accessToken,
        userId,
        userRole,
        loginUser,
        logoutUser,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used within the AuthProvider");
  return ctx;
}