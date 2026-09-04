import { List, ListItem, ListItemButton, ListItemText, Box, Divider } from "@mui/material";
import { useNavigate } from "react-router";
import { useAuth } from "@/features/auth";
import t from "@/locales/el";

export default function Sidebar() {
  const navigate = useNavigate();
  const { userRole, isAuthenticated, logoutUser } = useAuth();

  const handleLogout = () => {
    logoutUser();
    navigate("/login");
  };

  return (
    <Box sx={{ width: "100%", height: "100%", bgcolor: "background.paper" }}>
      <List>
        <ListItem disablePadding>
          <ListItemButton onClick={() => navigate("/")}>
            <ListItemText primary={t.nav.home} />
          </ListItemButton>
        </ListItem>

        {isAuthenticated && (
          <>
            <Divider />
            {(userRole === "Curator" || userRole === "Admin") && (
            <ListItem disablePadding>
              <ListItemButton onClick={() => navigate("/cultural-items")}>
                <ListItemText primary={t.nav.allItems} />
              </ListItemButton>
            </ListItem>
            )}

            {(userRole === "Contributor" || userRole === "Admin") && (
              <ListItem disablePadding>
                <ListItemButton onClick={() => navigate("/my-items")}>
                  <ListItemText primary={t.nav.myItems} />
                </ListItemButton>
              </ListItem>
            )}

            {(userRole === "Contributor" || userRole === "Admin") && (
              <ListItem disablePadding>
                <ListItemButton onClick={() => navigate("/cultural-items/new")}>
                  <ListItemText primary={t.nav.createItem} />
                </ListItemButton>
              </ListItem>
            )}

            {(userRole === "Curator" || userRole === "Admin") && (
              <ListItem disablePadding>
                <ListItemButton onClick={() => navigate("/cultural-items?status=ForReview")}>
                  <ListItemText primary={t.nav.pending} />
                </ListItemButton>
              </ListItem>
            )}

            {userRole === "Admin" && (
              <ListItem disablePadding>
                <ListItemButton onClick={() => navigate("/users")}>
                  <ListItemText primary={t.nav.userManagement} />
                </ListItemButton>
              </ListItem>
            )}
          </>
        )}
      </List>

      <Divider />

      <List>
        {!isAuthenticated ? (
          <ListItem disablePadding>
            <ListItemButton onClick={() => navigate("/login")}>
              <ListItemText primary={t.nav.login} />
            </ListItemButton>
          </ListItem>
        ) : (
          <ListItem disablePadding>
            <ListItemButton onClick={handleLogout}>
              <ListItemText primary={t.nav.logout} />
            </ListItemButton>
          </ListItem>
        )}
      </List>
    </Box>
  );
}