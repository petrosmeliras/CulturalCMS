import type {ItemCardProps} from "@/features/culturalItems/types/props";
import {useNavigate} from "react-router";
import {Box, Button, Card, CardActions, CardContent, Typography} from "@mui/material";
import t from "@/locales/el"

export default function ItemCard({ item }: ItemCardProps) {
  const navigate = useNavigate();

  return (
    <Card
      sx={{
        height: "100%",
        display: "flex",
        flexDirection: "column",
        boxShadow: 1,
        borderRadius: 2,
      }}
    >
      <CardContent sx={{ flexGrow: 1 }}>
        <Box
          sx={{
            display: "flex",
            justifyContent: "space-between",
            alignItems: "flex-start",
            mb: 1,
          }}
        >
          <Typography variant="h6" component="h2" sx={{ fontWeight: "bold", wordBreak: "break-word" }}>
            {item.title}
          </Typography>
        </Box>

        <Typography variant="body2" color="text.secondary" gutterBottom sx={{ wordBreak: "break-word" }}>
          {t.field.category}: {item.category} | {t.field.historicalPeriod}: {item.historicalPeriod}
        </Typography>

        <Typography
          variant="body2"
          color="text.primary"
          sx={{
            mt: 1.5,
            display: "-webkit-box",
            WebkitLineClamp: 3,
            WebkitBoxOrient: "vertical",
            overflow: "hidden",
            wordBreak: "break-word",
          }}
        >
          {item.description}
        </Typography>
      </CardContent>

      <CardActions sx={{ p: 2, pt: 0 }}>
        <Button
          size="small"
          variant="outlined"
          onClick={() => navigate(`/cultural-items/${item.id}`)}
        >
          {t.items.viewDetails}
        </Button>
      </CardActions>
    </Card>
  );
}