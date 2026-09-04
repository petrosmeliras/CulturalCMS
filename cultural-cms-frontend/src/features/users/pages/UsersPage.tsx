import { useEffect, useState } from "react";
import { Container, Typography, Box, TextField, Button, Alert } from "@mui/material";
import { toast } from "sonner";
import { getAllUsers, updateUserRole } from "../api/users";
import type { User, UserRole } from "../types";
import UserTable from "../components/UserTable";
import PaginationControls from "@/shared/ui/PaginationControls";
import LoadingSpinner from "@/shared/ui/LoadingSpinner";
import t from "@/locales/el";

const PAGE_SIZE = 10;

export default function UsersPage() {
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [pageNumber, setPageNumber] = useState(1);
  const [totalRecords, setTotalRecords] = useState(0);

  const [inputValue, setInputValue] = useState("");
  const [usernameFilter, setUsernameFilter] = useState("");

  useEffect(() => {
    const fetchUsers = async () => {
      setLoading(true);
      setError("");
      try {
        const result = await getAllUsers(pageNumber, PAGE_SIZE, usernameFilter);
        setUsers(result.data);
        setTotalRecords(result.totalRecords);
      } catch (error) {
        console.error(error);
        setError(t.users.loadError);
      } finally {
        setLoading(false);
      }
    };
    fetchUsers();
  }, [pageNumber, usernameFilter]);

  const handleSearchSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setPageNumber(1);
    setUsernameFilter(inputValue);
  };

  const handleRoleChange = async (userId: number, newRole: UserRole) => {
    try {
      await updateUserRole(userId, newRole);
      toast.success(t.users.updateSuccess);
      setUsers((prev) =>
        prev.map((user) => (user.id === userId ? { ...user, userRole: newRole } : user))
      );
    } catch (error) {
      console.error(error);
      toast.error(t.users.updateError);
    }
  };

  return (
    <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
      <Box sx={{ mb: 4 }}>
        <Typography variant="h4" component="h1" sx={{ fontWeight: "bold" }}>
          {t.users.title}
        </Typography>
        <Typography variant="subtitle1" color="text.secondary">
          {t.users.subtitle}
        </Typography>
      </Box>

      <Box component="form" onSubmit={handleSearchSubmit} sx={{ display: "flex", gap: 2, mb: 3 }}>
        <TextField
          label={t.users.usernameFilter}
          size="small"
          value={inputValue}
          onChange={(e) => setInputValue(e.target.value)}
          sx={{ minWidth: 250 }}
        />
        <Button type="submit" variant="contained">{t.search.button}</Button>
      </Box>

      {error && <Alert severity="error" sx={{ mb: 3 }}>{error}</Alert>}

      {loading ? (
        <LoadingSpinner minHeight="50vh" />
      ) : (
        <>
          <UserTable users={users} onRoleChange={handleRoleChange} />
          <PaginationControls
            pageNumber={pageNumber}
            pageSize={PAGE_SIZE}
            totalRecords={totalRecords}
            onPageChange={setPageNumber}
          />
        </>
      )}
    </Container>
  );
}