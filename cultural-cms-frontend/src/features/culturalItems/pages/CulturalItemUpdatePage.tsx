import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router";
import { Container, Typography, Paper } from "@mui/material";
import { toast } from "sonner";
import { getCulturalItemById, updateItem } from "../api/culturalItems";
import { uploadImage } from "../api/images";
import CulturalItemForm from "../components/CulturalItemForm";
import LoadingSpinner from "@/shared/ui/LoadingSpinner";
import type { CulturalItem } from "@/features/culturalItems/types/domain";
import type { CulturalItemFormData } from "../schemas/culturalItem";
import t from "@/locales/el";

export default function CulturalItemUpdatePage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [item, setItem] = useState<CulturalItem | null>(null);
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    const fetchItem = async () => {
      try {
        const data = await getCulturalItemById(Number(id));
        setItem(data);
      } catch (error) {
        console.error(error);
        toast.error(t.items.loadError);
        navigate("/cultural-items");
      } finally {
        setLoading(false);
      }
    };
    if (id) fetchItem();
  }, [id, navigate]);

  const handleSubmit = async (data: CulturalItemFormData, imageFile: File | null) => {
    setSubmitting(true);
    try {
      const imageUrl = imageFile ? await uploadImage(imageFile) : item?.imageUrl;
      await updateItem(Number(id), { ...data, imageUrl });
      toast.success(t.items.updateSuccess);
      navigate(`/cultural-items/${id}`);
    } catch (error) {
      console.error(error);
      toast.error(t.items.updateError);
    } finally {
      setSubmitting(false);
    }
  };

  if (loading) return <LoadingSpinner />;
  if (!item) return null;

  return (
    <Container maxWidth="md" sx={{ mt: 4, mb: 4 }}>
      <Paper sx={{ p: 4, boxShadow: 2, borderRadius: 2 }}>
        <Typography variant="h4" component="h1" sx={{ mb: 3, fontWeight: "bold" }}>
          {t.items.editTitle}
        </Typography>
        <CulturalItemForm
          mode="update"
          defaultValues={item}
          existingImageUrl={item.imageUrl}
          submitLabel={t.items.updateItem}
          submitting={submitting}
          onCancel={() => navigate(`/cultural-items/${id}`)}
          onSubmit={handleSubmit}
        />
      </Paper>
    </Container>
  );
}