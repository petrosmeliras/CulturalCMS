import { Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Paper, Select, MenuItem, type SelectChangeEvent } from "@mui/material";
import type { UserTableProps, UserRole } from "../types";
import t from "@/locales/el";

const AVAILABLE_ROLES: UserRole[] = ["Admin", "Curator", "Contributor"];

export default function UserTable({ users, onRoleChange }: UserTableProps) {
  const handleChange = (userId: number, event: SelectChangeEvent) => {
    onRoleChange(userId, event.target.value as UserRole);
  };

  return (
    <TableContainer component={Paper} sx={{ boxShadow: 2, borderRadius: 2 }}>
      <Table sx={{ tableLayout: "fixed" }}>
        <TableHead sx={{ backgroundColor: "#f5f5f5" }}>
          <TableRow>
            <TableCell sx={{ fontWeight: "bold", width: "20%" }}>{t.field.username}</TableCell>
            <TableCell sx={{ fontWeight: "bold", width: "30%" }}>{t.field.email}</TableCell>
            <TableCell sx={{ fontWeight: "bold", width: "30%" }}>{t.users.table.fullName}</TableCell>
            <TableCell sx={{ fontWeight: "bold", width: "20%" }}>{t.users.table.role}</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {users.map((user) => (
            <TableRow key={user.id} hover>
              <TableCell sx={{ overflow: "hidden", textOverflow: "ellipsis" }}>{user.username}</TableCell>
              <TableCell sx={{ overflow: "hidden", textOverflow: "ellipsis" }}>{user.email}</TableCell>
              <TableCell sx={{ overflow: "hidden", textOverflow: "ellipsis" }}>{`${user.firstname} ${user.lastname}`}</TableCell>
              <TableCell>
                <Select
                  value={user.userRole}
                  size="small"
                  onChange={(e) => handleChange(user.id, e)}
                  fullWidth
                >
                  {AVAILABLE_ROLES.map((role) => (
                    <MenuItem key={role} value={role}>
                      {t.roles[role]}
                    </MenuItem>
                  ))}
                </Select>
              </TableCell>
            </TableRow>
          ))}

          {users.length === 0 && (
            <TableRow>
              <TableCell colSpan={4} align="center" sx={{ py: 3 }}>
                {t.users.noUsers}
              </TableCell>
            </TableRow>
          )}
        </TableBody>
      </Table>
    </TableContainer>
  );
}