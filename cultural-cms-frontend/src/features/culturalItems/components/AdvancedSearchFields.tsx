import { Box, Button, FormControl, InputLabel, MenuItem, Select, TextField } from "@mui/material";
import type { AdvancedSearchFieldsProps } from "../types/props";
import t from "@/locales/el";

const STATUS_OPTIONS = ["Draft", "ForReview", "Published"];

export default function AdvancedSearchFields({
                                               filters,
                                               onFieldChange,
                                               onStatusChange,
                                               showStatusFilter = false,
                                               onReset,
                                             }: AdvancedSearchFieldsProps) {
  return (
    <Box sx={{ display: "flex", flexWrap: "wrap", alignItems: "center", gap: 2, mt: 2 }}>
      <TextField
        label={t.field.category}
        size="small"
        value={filters.category}
        onChange={(e) => onFieldChange("category", e.target.value)}
        sx={{ minWidth: 160 }}
      />
      <TextField
        label={t.field.historicalPeriod}
        size="small"
        value={filters.historicalPeriod}
        onChange={(e) => onFieldChange("historicalPeriod", e.target.value)}
        sx={{ minWidth: 160 }}
      />

      <TextField
        label={t.search.metaKey}
        size="small"
        value={filters.metadataKey}
        onChange={(e) => onFieldChange("metadataKey", e.target.value)}
        sx={{ minWidth: 140 }}
      />
      <TextField
        label={t.search.metaValue}
        size="small"
        value={filters.metadataValue}
        onChange={(e) => onFieldChange("metadataValue", e.target.value)}
        sx={{ minWidth: 140 }}
      />

      {showStatusFilter && (
        <FormControl size="small" sx={{ minWidth: 140 }}>
          <InputLabel>{t.field.status}</InputLabel>
          <Select label={t.field.status} value={filters.status} onChange={(e) => onStatusChange(e.target.value)}>
            <MenuItem value="">{t.search.allStatuses}</MenuItem>
            {STATUS_OPTIONS.map((s) => (
              <MenuItem key={s} value={s}>{t.status[s]}</MenuItem>
            ))}
          </Select>
        </FormControl>
      )}

      <Button variant="outlined" color="error" onClick={onReset} sx={{ ml: "auto" }}>
        {t.search.reset}
      </Button>
    </Box>
  );
}