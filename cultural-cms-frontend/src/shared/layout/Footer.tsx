import { Box, Typography } from "@mui/material";
import t from "@/locales/el"

export default function Footer() {
  return (
    <Box
      component="footer"
      sx={{
        py: 3,
        px: 2,
        mt: "auto",
        backgroundColor: (theme) => theme.palette.grey[200]
      }}
    >
      <Typography variant="body2" color="text.secondary" align="center">
        {t.copyright}
      </Typography>
    </Box>
  );
}