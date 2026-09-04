import { Outlet } from "react-router";
import { Box } from "@mui/material";
import Header from "./Header";
import Footer from "./Footer";
import Sidebar from "@/shared/layout/Sidebar";

export default function RouterLayout() {
  return (
    <Box sx={{ display: "flex", flexDirection: "column", minHeight: "100vh", bgcolor: "background.default" }}>

      <Header />

      <Box sx={{ display: "flex", flexGrow: 1, overflow: "hidden" }}>
        <Box component="nav" sx={{ width: 240, flexShrink: 0, borderRight: 1, borderColor: 'divider' }}>
          <Sidebar />
        </Box>

        <Box
          component="main"
          sx={{
            flexGrow: 1,
            display: "flex",
            flexDirection: "column",
            pt: 4,
            pb: 4,
            px: 3,
            overflowY: "auto"
          }}
        >
          <Outlet />
        </Box>
      </Box>

      <Footer />

    </Box>
  );
}