import { Box, Pagination } from "@mui/material";
import type {PaginationControlsProps} from "@/shared/types.ts";

export default function PaginationControls({ pageNumber, pageSize, totalRecords, onPageChange }: PaginationControlsProps) {
  const pageCount = Math.max(1, Math.ceil(totalRecords / pageSize));
  if (pageCount <= 1) return null;

  return (
    <Box sx={{ display: "flex", justifyContent: "center", mt: 4 }}>
      <Pagination count={pageCount} page={pageNumber} onChange={(_, page) =>
        onPageChange(page)} color="primary" />
    </Box>
  );
}