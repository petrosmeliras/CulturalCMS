import {useAuth} from "@/features/auth";
import {Navigate, Outlet, useLocation} from "react-router";


interface ProtectedRouteProps {
  allowedRoles?: string[];
}

const ProtectedRoute = ({ allowedRoles }: ProtectedRouteProps) => {
  const { isAuthenticated, userRole } = useAuth();
  const location = useLocation();

  if (!isAuthenticated) {
    return <Navigate to="/login" state={{from: location}} />;
  }

  if (allowedRoles && !allowedRoles.includes(userRole!)) {
    return <Navigate to="/" replace />;
  }
  return <Outlet />
}

export default ProtectedRoute;