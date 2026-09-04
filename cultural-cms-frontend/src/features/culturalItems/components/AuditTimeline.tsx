import type { AuditTimelineProps } from "../types/props";
import type { AuditLog } from "../types/domain";
import { useEffect, useState } from "react";
import { useAuth } from "@/features/auth";
import { getAuditLogs } from "../api/culturalItems";
import { getChangeSummary } from "../utils/auditFormat";
import { Box, Divider, List, ListItem, ListItemText, Paper, Typography } from "@mui/material";
import t from "@/locales/el";

export default function AuditTimeline({ entityId, lastUpdate }: AuditTimelineProps) {
  const [logs, setLogs] = useState<AuditLog[]>([]);
  const { userRole } = useAuth();

  useEffect(() => {
    if (userRole !== "Admin" && userRole !== "Curator") return;

    const fetchLogs = async () => {
      try {
        const data = await getAuditLogs(entityId);
        setLogs(data);
      } catch (error) {
        // Audit trail is a secondary feature; a failed fetch shouldn't break the page.
        console.error(error);
      }
    };
    fetchLogs();
  }, [entityId, userRole, lastUpdate]);

  if (userRole !== "Admin" && userRole !== "Curator") return null;

  return (
    <Paper sx={{ p: 2, mt: 3 }}>
      <Typography variant="h6" gutterBottom>{t.audit.title}</Typography>
      <List>
        {logs.length === 0 ? (
          <Typography variant="body2" color="text.secondary">{t.audit.empty}</Typography>
        ) : (
          logs.map((log, index) => {
            const summary = getChangeSummary(log);

            return (
              <Box key={log.id}>
                <ListItem>
                  <ListItemText
                    primary={`${t.audit[log.action]} - ${log.username}`}
                    secondary={
                      <>
                        {summary.transition && (
                          <Box component="span" sx={{ display: "block" }}>
                            {summary.transition}
                          </Box>
                        )}

                        {summary.fields.map((f, i) => (
                          <Box component="span" key={i} sx={{ display: "block", wordBreak: "break-word" }}>
                            <Box component="span" sx={{ fontWeight: "bold" }}>{f.label}:</Box> {f.detail}
                          </Box>
                        ))}

                        {summary.metadataChanges.length > 0 && (
                          <Box component="span" sx={{ display: "block", mt: 0.5 }}>
                            <Box component="span" sx={{ fontWeight: "bold" }}>{t.items.metadata}:</Box>
                            {summary.metadataChanges.map((change, i) => (
                              <Box component="span" key={i} sx={{ display: "block", pl: 1.5, wordBreak: "break-word" }}>
                                {change}
                              </Box>
                            ))}
                          </Box>
                        )}

                        <Box component="span" sx={{ display: "block", mt: 0.5 }}>
                          {new Date(log.timestamp).toLocaleString()}
                        </Box>
                      </>
                    }
                  />
                </ListItem>
                {index < logs.length - 1 && <Divider />}
              </Box>
            );
          })
        )}
      </List>
    </Paper>
  );
}