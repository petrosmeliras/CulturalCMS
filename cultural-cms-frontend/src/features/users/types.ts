export type UserRole = "Admin" | "Curator" | "Contributor";

export type User = {
  id: number;
  username: string;
  email: string;
  firstname: string;
  lastname: string;
  userRole: UserRole;
};

export type UserTableProps = {
  users: User[];
  onRoleChange: (userId: number, newRole: UserRole) => void;
};