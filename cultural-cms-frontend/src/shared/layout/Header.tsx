import { AppBar, Toolbar, Typography, Button, Box } from "@mui/material";
import { useNavigate } from "react-router";
import { useAuth } from "@/features/auth";
import t from "@/locales/el";

export default function Header() {
  const { isAuthenticated, logoutUser } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logoutUser();
    navigate("/login");
  };

  return (
    <AppBar position="static">
      <Toolbar
        sx={{height: '50px'}}>
        <Typography
          variant="h6"
          component="div"
          sx={{ flexGrow: 1, cursor: "pointer" }}
          onClick={() => navigate("/")}
        >
          {t.appName}
        </Typography>

        <Box>
          {isAuthenticated ? (
            <Button color="inherit" onClick={handleLogout}>
              {t.nav.logout}
            </Button>
          ) : (
           <>
            <Button color="inherit" onClick={() => navigate("/login")}>
              {t.nav.login}
            </Button>
            <Button variant="outlined" color="inherit" onClick={() => navigate("/signup")}>
              {t.nav.signup}
            </Button>
           </>
          )}
        </Box>
      </Toolbar>
    </AppBar>
  );
}