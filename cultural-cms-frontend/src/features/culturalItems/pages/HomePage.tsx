import { Container, Typography, Box, Grid, Alert } from "@mui/material";
import { useItemSearch } from "../hooks/useItemSearch";
import LoadingSpinner from "@/shared/ui/LoadingSpinner";
import ItemCard from "../components/ItemCard";
import SearchFilters from "../components/SearchFilters";
import PaginationControls from "../../../shared/ui/PaginationControls";
import t from "@/locales/el"

export default function Homepage() {
  const { items, totalRecords, loading, error, pageNumber, pageSize, sortBy, sortOrder,
    applyFilters, applySort, setPageNumber }
    = useItemSearch("public");

  return (
    <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
      <Box sx={{ textAlign: "center", mb: 4 }}>
        <Typography variant="h3" component="h1" gutterBottom sx={{ fontWeight: "bold" }}>
          {t.home.title}
        </Typography>
        <Typography variant="subtitle1" color="text.secondary">
          {t.home.subtitle}
        </Typography>
      </Box>

      <SearchFilters onSearch={applyFilters} sortBy={sortBy} sortOrder={sortOrder} onSortChange={applySort} showSortControls={false} />

      {error && <Alert severity="error" sx={{ mb: 3 }}>{error}</Alert>}

      {loading ? (
        <LoadingSpinner minHeight="30vh" />
      ) : (
        <>
          <Grid container spacing={3}>
            {items.map((item) => (
              <Grid size={{ xs: 12, sm: 6, md: 4 }} key={item.id}>
                <ItemCard item={item} />
              </Grid>
            ))}
          </Grid>
          <PaginationControls pageNumber={pageNumber} pageSize={pageSize} totalRecords={totalRecords} onPageChange={setPageNumber} />
        </>
      )}
    </Container>
  );
}