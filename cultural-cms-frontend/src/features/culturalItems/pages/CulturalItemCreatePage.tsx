import { useState } from "react";
import { useNavigate } from "react-router";
import { Container, Typography, Paper } from "@mui/material";
import { toast } from "sonner";
import { createCulturalItem } from "../api/culturalItems";
import { uploadImage } from "../api/images";
import CulturalItemForm from "../components/CulturalItemForm";
import type { CulturalItemFormData } from "../schemas/culturalItem";
import t from "@/locales/el";

export default function CulturalItemCreatePage() {
  const navigate = useNavigate();
  const [submitting, setSubmitting] = useState(false);

  const handleSubmit = async (data: CulturalItemFormData, imageFile: File | null) => {
    setSubmitting(true);
    try {
      const imageUrl = imageFile ? await uploadImage(imageFile) : undefined;
      await createCulturalItem({ ...data, imageUrl });
      toast.success(t.items.createSuccess);
      navigate("/cultural-items");
    } catch (error){
      console.error(error);
      toast.error(t.items.createError);
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <Container maxWidth="md" sx={{ mt: 4, mb: 4 }}>
      <Paper sx={{ p: 4, boxShadow: 2, borderRadius: 2 }}>
        <Typography variant="h4" component="h1" sx={{ mb: 3, fontWeight: "bold" }}>
          {t.items.createTitle}
        </Typography>
        <CulturalItemForm
          mode="create"
          submitLabel={t.items.saveDraft}
          submitting={submitting}
          onCancel={() => navigate("/cultural-items")}
          onSubmit={handleSubmit}
        />
      </Paper>
    </Container>
  );
}