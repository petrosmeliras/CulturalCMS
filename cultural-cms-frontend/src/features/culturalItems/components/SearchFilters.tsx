import { useState } from "react";
import { Box, Button, TextField, Collapse, FormControl, InputLabel, MenuItem, Select } from "@mui/material";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import ExpandLessIcon from "@mui/icons-material/ExpandLess";
import type { SearchFiltersProps } from "../types/props";
import AdvancedSearchFields from "./AdvancedSearchFields";
import t from "@/locales/el";

const EMPTY_FILTERS = {
  searchTerm: "",
  category: "",
  historicalPeriod: "",
  status: "",
  metadataKey: "",
  metadataValue: "",
};

const SORT_BY_OPTIONS = [
  { value: "CreatedAt", label: t.search.sort.date },
  { value: "Title", label: t.search.sort.title },
  { value: "ViewCount", label: t.search.sort.popularity },
];

function normalizeMetadataKey(key: string): string {
  return key.trim().toLowerCase() === t.items.tagLabel.toLowerCase() ? "Tag" : key;
}

export default function SearchFilters({
                                        showStatusFilter = false,
                                        showSortControls = true,
                                        onSearch,
                                        sortBy,
                                        sortOrder,
                                        onSortChange,
                                      }: SearchFiltersProps) {
  const [filters, setFilters] = useState(EMPTY_FILTERS);
  const [showAdvanced, setShowAdvanced] = useState(false);

  const handleFieldChange = (field: keyof typeof filters, value: string) => {
    setFilters((prev) => ({ ...prev, [field]: value }));
  };

  const handleStatusChange = (value: string) => {
    const updated = { ...filters, status: value };
    setFilters(updated);
    onSearch({ ...updated, metadataKey: normalizeMetadataKey(updated.metadataKey) });
  };

  const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    onSearch({ ...filters, metadataKey: normalizeMetadataKey(filters.metadataKey) });
  };

  const handleReset = () => {
    setFilters(EMPTY_FILTERS);
    onSearch(EMPTY_FILTERS);
  };

  return (
    <Box component="form" onSubmit={handleSubmit} sx={{ mb: 4 }}>
      <Box sx={{ display: "flex", flexWrap: "wrap", gap: 2, alignItems: "center" }}>
        <TextField
          label={t.search.label}
          size="small"
          value={filters.searchTerm}
          onChange={(e) => handleFieldChange("searchTerm", e.target.value)}
          sx={{ flex: 1, minWidth: 200 }}
        />

        {showSortControls && (
          <>
            <FormControl size="small" sx={{ minWidth: 150 }}>
              <InputLabel>{t.search.sort.by}</InputLabel>
              <Select
                label={t.search.sort.by}
                value={sortBy}
                onChange={(e) => onSortChange(e.target.value, sortOrder)}
              >
                {SORT_BY_OPTIONS.map((opt) => (
                  <MenuItem key={opt.value} value={opt.value}>{opt.label}</MenuItem>
                ))}
              </Select>
            </FormControl>

            <FormControl size="small" sx={{ minWidth: 130 }}>
              <InputLabel>{t.search.sort.order}</InputLabel>
              <Select
                label={t.search.sort.order}
                value={sortOrder}
                onChange={(e) => onSortChange(sortBy, e.target.value as "asc" | "desc")}
              >
                <MenuItem value="asc">{t.search.sort.asc}</MenuItem>
                <MenuItem value="desc">{t.search.sort.desc}</MenuItem>
              </Select>
            </FormControl>
          </>
        )}

        <Button
          variant="text"
          onClick={() => setShowAdvanced((prev) => !prev)}
          endIcon={showAdvanced ? <ExpandLessIcon /> : <ExpandMoreIcon />}
        >
          {showAdvanced ? t.search.hideFilters : t.search.advancedFilters}
        </Button>
        <Button type="submit" variant="contained">
          {t.search.button}
        </Button>
      </Box>

      <Collapse in={showAdvanced}>
        <AdvancedSearchFields
          filters={filters}
          onFieldChange={handleFieldChange}
          onStatusChange={handleStatusChange}
          showStatusFilter={showStatusFilter}
          onReset={handleReset}
        />
      </Collapse>
    </Box>
  );
}