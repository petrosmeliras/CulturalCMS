import type {ItemTableProps} from "@/features/culturalItems/types/props";
import {Paper, Table, TableBody, TableCell, TableContainer, TableHead, TableRow} from "@mui/material";
import {useNavigate} from "react-router";
import StatusBadge from "@/features/culturalItems/components/StatusBadge.tsx";
import t from "@/locales/el"

export default function ItemTable({ items }: ItemTableProps) {
  const navigate = useNavigate();

  return (
    <TableContainer component={Paper} sx={{ boxShadow: 3, borderRadius: 2 }}>
      <Table sx={{ minWidth: 650 }} aria-label="cultural items table">
        <TableHead sx={{ backgroundColor: "#f5f5f5" }}>
          <TableRow>
            <TableCell sx={{ fontWeight: "bold" }}>{t.field.title}</TableCell>
            <TableCell sx={{ fontWeight: "bold" }}>{t.field.category}</TableCell>
            <TableCell sx={{ fontWeight: "bold" }}>{t.field.historicalPeriod}</TableCell>
            <TableCell sx={{ fontWeight: "bold" }}>{t.field.status}</TableCell>
            <TableCell align="right" sx={{ fontWeight: "bold" }}>
              {t.field.views}
            </TableCell>
          </TableRow>
        </TableHead>

        <TableBody>
          {items.map((item) => (
            <TableRow
              key={item.id}
              hover
              onClick={() => navigate(`/cultural-items/${item.id}`)}
              sx={{
                "&:last-child td, &:last-child th": { border: 0 },
                cursor: "pointer",
              }}
            >
              <TableCell sx={{ wordBreak: "break-word" }}>{item.title}</TableCell>
              <TableCell sx={{ wordBreak: "break-word" }}>{item.category}</TableCell>
              <TableCell sx={{ wordBreak: "break-word" }}>{item.historicalPeriod}</TableCell>
              <TableCell>
                <StatusBadge status={item.status} />
              </TableCell>
              <TableCell align="right">{item.viewCount}</TableCell>
            </TableRow>
          ))}

          {items.length === 0 && (
            <TableRow>
              <TableCell colSpan={5} align="center" sx={{ py: 3 }}>
                {t.items.noItems}
              </TableCell>
            </TableRow>
          )}
        </TableBody>
      </Table>
    </TableContainer>
  );
}