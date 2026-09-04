import {useLocation, useNavigate, useParams} from "react-router";
import {useAuth} from "@/features/auth";
import {useEffect, useState } from "react";
import type { CulturalItem } from "@/features/culturalItems/types/domain";
import {
  approveItem,
  deleteCulturalItem,
  getCulturalItemById,
  rejectItem,
  submitItemForReview
} from "@/features/culturalItems/api/culturalItems";
import { toast } from "sonner";
import { getImageUrl } from "@/shared/utils/imageUtils";
import LoadingSpinner from "@/shared/ui/LoadingSpinner";
import {Box, Button, Container, Divider, Paper, Typography} from "@mui/material";
import StatusBadge from "@/features/culturalItems/components/StatusBadge";
import AuditTimeline from "@/features/culturalItems/components/AuditTimeline";
import t from "@/locales/el";

export default function CulturalItemDetailsPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const location = useLocation();
  const { userId, userRole, isAuthenticated } = useAuth();

  const [item, setItem] = useState<CulturalItem | null>(null);
  const [loading, setLoading] = useState(true);
  const [imageError, setImageError] = useState(false);
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    const fetchDetails = async () => {
      if (!id) {
        setLoading(false);
        return;
      }

      try {
        const data = await getCulturalItemById(Number(id));
        setItem(data);
      } catch (error) {
        console.error(error);
        toast.error(t.items.loadError);
        navigate("/");
      } finally {
        setLoading(false);
      }
    };

    fetchDetails();
  }, [id, navigate]);

  const handleSubmitForReview = async () => {
    if (!id) return;
    setSubmitting(true);
    try {
      await submitItemForReview(Number(id));
      toast.success(t.items.submitSuccess);
      const updatedData = await getCulturalItemById(Number(id));
      setItem(updatedData);
    } catch (error) {
      console.error(error);
      toast.error(t.items.submitError);
    } finally {
      setSubmitting(false);
    }
  };

  const handleApprove = async () => {
    if (!id) return;
    setSubmitting(true);
    try {
      await approveItem(Number(id));
      toast.success(t.items.approveSuccess);
      const updatedData = await getCulturalItemById(Number(id));
      setItem(updatedData);
    } catch (error) {
      console.error(error);
      toast.error(t.items.approveError);
    } finally {
      setSubmitting(false);
    }
  };

  const handleReject = async () => {
    if (!id) return;
    setSubmitting(true);
    try {
      await rejectItem(Number(id));
      toast.warning(t.items.rejectSuccess);
      const updatedData = await getCulturalItemById(Number(id));
      setItem(updatedData);
    } catch (error) {
      console.error(error);
      toast.error(t.items.rejectError);
    } finally {
      setSubmitting(false);
    }
  };

  const handleDelete = async () => {
    if (!window.confirm(t.items.deleteConfirm)) return;
    if (!id) return;

    setSubmitting(true);
    try {
      await deleteCulturalItem(Number(id));
      toast.success(t.items.deleteSuccess);
      navigate("/");
    } catch (error) {
      console.error(error);
      toast.error(t.items.deleteError);
    } finally {
      setSubmitting(false);
    }
  };

  if (loading) return <LoadingSpinner />;
  if (!item) return null;

  const imageUrl = getImageUrl(item.imageUrl);
  const showImage = imageUrl && !imageError;

  const isOwner = isAuthenticated && String(item.createdById) === userId;
  const canManageAsContributor = userRole === "Contributor" && isOwner;
  const canManageAsAdmin = userRole === "Admin";
  const canEdit = canManageAsAdmin || (item.status === "Draft" && canManageAsContributor);
  const canSubmitForReview = item.status === "Draft" && (canManageAsContributor || canManageAsAdmin);
  const canDelete = canManageAsAdmin;
  const canReview = item.status === "ForReview" && isAuthenticated && (userRole === "Curator" || userRole === "Admin");
  const canAccessManageList = userRole === "Curator" || userRole === "Admin";

  const handleBackToList = () => {
    if (location.key !== "default") {
      navigate(-1);
    } else {
      navigate(canAccessManageList ? "/cultural-items" : "/");
    }
  };

  return (
    <Container maxWidth="md" sx={{ mt: 4, mb: 4 }}>
      <Paper sx={{ p: 4, boxShadow: 1, borderRadius: 2 }}>
        {showImage && (
          <Box
            component="img"
            src={imageUrl}
            alt={item.title}
            sx={{
              width: "100%",
              maxHeight: 400,
              objectFit: "contain",
              borderRadius: 1,
              mb: 4,
            }}
            onError={() => setImageError(true)}
          />
        )}

        <Box
          sx={{
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center",
            mb: 3,
          }}
        >
          <Typography variant="h4" component="h1" sx={{ fontWeight: "bold", wordBreak: "break-word" }}>
            {item.title}
          </Typography>
          <StatusBadge status={item.status} />
        </Box>

        <Divider sx={{ mb: 3 }} />

        <Box sx={{ mb: 4 }}>
          <Box sx={{ mb: 1 }}>
            <Typography variant="subtitle1" color="text.secondary" component="span">
              {t.field.category}:{" "}
            </Typography>
            <Typography variant="subtitle1" color="text.primary" component="span" sx={{ fontWeight: 500, wordBreak: "break-word" }}>
              {item.category}
            </Typography>
          </Box>

          <Box sx={{ mb: 1 }}>
            <Typography variant="subtitle1" color="text.secondary" component="span">
              {t.field.historicalPeriod}:{" "}
            </Typography>
            <Typography variant="subtitle1" color="text.primary" component="span" sx={{ fontWeight: 500, wordBreak: "break-word" }}>
              {item.historicalPeriod}
            </Typography>
          </Box>

          <Typography variant="body1" sx={{ mt: 3, whiteSpace: "pre-line", lineHeight: 1.6, wordBreak: "break-word" }}>
            {item.description}
          </Typography>

          {item.metadata && item.metadata.length > 0 && (
            <Box sx={{ mt: 3, mb: 2, p: 2, bgcolor: "grey.50", borderRadius: 1 }}>
              <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                {t.items.metadata}
              </Typography>
              {item.metadata.map((meta, index) => (
                <Typography key={index} variant="body2" sx={{ wordBreak: "break-word" }}>
                  <strong>{meta.key === "Tag" ? t.items.tagLabel : meta.key}:</strong> {meta.value}
                </Typography>
              ))}
            </Box>
          )}
        </Box>

        <Divider sx={{ mb: 3 }} />

        <AuditTimeline
          entityId={Number(id)}
          lastUpdate={item.updatedAt ? item.updatedAt.toString() : ""}
        />

        <Box sx={{ display: "flex", gap: 2, justifyContent: "space-around" }}>
          <Button variant="outlined" onClick={handleBackToList}>
            {t.items.backToList}
          </Button>

          {canEdit && (
            <Button
              variant="outlined"
              onClick={() => navigate(`/cultural-items/edit/${item.id}`)}>
              {t.items.edit}
            </Button>
          )}

          {canSubmitForReview && (
            <Button variant="contained" onClick={handleSubmitForReview} disabled={submitting}>
              {t.items.submitForReview}
            </Button>
          )}

          {canReview && (
            <>
              <Button variant="outlined" color="error" onClick={handleReject} disabled={submitting}>
                {t.items.reject}
              </Button>
              <Button variant="contained" color="success" onClick={handleApprove} disabled={submitting}>
                {t.items.approve}
              </Button>
            </>
          )}

          {canDelete && (
            <Button variant="contained" color="error" onClick={handleDelete} disabled={submitting}>
              {t.items.delete}
            </Button>
          )}
        </Box>
      </Paper>
    </Container>
  );
}