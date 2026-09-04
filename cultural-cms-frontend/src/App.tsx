import { Route, Routes, Navigate } from "react-router";
import RouterLayout from "@/shared/layout/RouterLayout";
import { LoginPage, SignupPage, ProtectedRoute } from "@/features/auth";
import {
  HomePage,
  CulturalItemsPage,
  CulturalItemCreatePage,
  CulturalItemDetailsPage,
  CulturalItemUpdatePage,
  MyItemsPage,
} from "@/features/culturalItems";
import { UsersPage } from "@/features/users";

function App() {
  return (
    <Routes>
      <Route element={<RouterLayout />}>
        <Route path="/" element={<HomePage />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/signup" element={<SignupPage />} />
        <Route path="/cultural-items/:id" element={<CulturalItemDetailsPage />} />

        <Route element={<ProtectedRoute allowedRoles={["Curator", "Admin"]} />}>
          <Route path="/cultural-items" element={<CulturalItemsPage />} />
        </Route>

        <Route element={<ProtectedRoute allowedRoles={["Contributor", "Admin"]} />}>
          <Route path="/my-items" element={<MyItemsPage />} />
          <Route path="/cultural-items/new" element={<CulturalItemCreatePage />} />
          <Route path="/cultural-items/edit/:id" element={<CulturalItemUpdatePage />} />
        </Route>

        <Route element={<ProtectedRoute allowedRoles={["Admin"]} />}>
          <Route path="/users" element={<UsersPage />} />
        </Route>

        <Route path="*" element={<Navigate to="/" replace />} />
      </Route>
    </Routes>
  );
}

export default App;