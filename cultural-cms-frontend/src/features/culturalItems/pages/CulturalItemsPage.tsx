import { useNavigate } from "react-router";
import { Container, Typography, Box, Button, Alert } from "@mui/material";
import { useItemSearch } from "@/features/culturalItems/hooks/useItemSearch";
import LoadingSpinner from "@/shared/ui/LoadingSpinner";
import ItemTable from "@/features/culturalItems/components/ItemTable";
import SearchFilters from "@/features/culturalItems/components/SearchFilters";
import PaginationControls from "@/shared/ui/PaginationControls";
import t from "@/locales/el";

export default function CulturalItemsPage() {
  const { items, totalRecords, loading, error, pageNumber, pageSize, sortBy, sortOrder,
    applyFilters, applySort, setPageNumber }
    = useItemSearch("all");
  const navigate = useNavigate();

  return (
    <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
      <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "center", mb: 3 }}>
        <Typography variant="h4" component="h1" sx={{ fontWeight: "bold" }}>
          {t.items.manageTitle}
        </Typography>
        <Button variant="contained" onClick={() => navigate("/cultural-items/new")}>
          {t.items.addNewItem}
        </Button>
      </Box>

      <SearchFilters showStatusFilter onSearch={applyFilters} sortBy={sortBy} sortOrder={sortOrder} onSortChange={applySort} />
      {error && <Alert severity="error" sx={{ mb: 3 }}>{error}</Alert>}

      {loading ? (
        <LoadingSpinner />
      ) : (
        <>
          <ItemTable items={items} />
          <PaginationControls pageNumber={pageNumber} pageSize={pageSize} totalRecords={totalRecords} onPageChange={setPageNumber} />
        </>
      )}
    </Container>
  );
}