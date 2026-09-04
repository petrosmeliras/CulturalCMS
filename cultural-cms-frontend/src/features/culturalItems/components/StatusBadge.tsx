import { Chip } from "@mui/material";
import t from "@/locales/el";
import type {StatusBadgeProps} from "@/features/culturalItems/types/props";

export default function StatusBadge({ status }: StatusBadgeProps) {
  return (
    <Chip
      label={t.status[status] ?? status}
      variant="outlined"
      size="small"
      sx={{ fontWeight: "bold" }}
    />
  );
}