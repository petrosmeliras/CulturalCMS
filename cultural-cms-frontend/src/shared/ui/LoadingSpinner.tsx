import { Box, CircularProgress } from "@mui/material";
import type {LoadingSpinnerProps} from "@/shared/types.ts";

export default function LoadingSpinner({ minHeight = "50vh" }: LoadingSpinnerProps) {
  return (
    <Box
      sx={{
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
        minHeight,
      }}
    >
      <CircularProgress />
    </Box>
  );
}