import { useState } from "react";
import { useForm, useFieldArray } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Box, TextField, Typography, Button, Divider } from "@mui/material";
import { type CulturalItemFormData, culturalItemSchema } from "@/features/culturalItems/schemas/culturalItem";
import type { CulturalItemFormProps } from "@/features/culturalItems/types/props";
import MetadataEditor from "@/features/culturalItems/components/MetadataEditor";
import { getImageUrl } from "@/shared/utils/imageUtils";
import t from "@/locales/el";

export default function CulturalItemForm({
                                           mode,
                                           defaultValues,
                                           existingImageUrl,
                                           submitLabel,
                                           submitting,
                                           onCancel,
                                           onSubmit,
                                         }: CulturalItemFormProps) {
  const [imageFile, setImageFile] = useState<File | null>(null);

  const {
    register,
    handleSubmit,
    control,
    formState: { errors },
  } = useForm<CulturalItemFormData>({
    resolver: zodResolver(culturalItemSchema),
    defaultValues: {
      title: "",
      description: "",
      category: "",
      historicalPeriod: "",
      metadata: [],
      ...defaultValues,
    },
  });

  const { fields, append, remove } = useFieldArray({ control, name: "metadata" });

  const submitForm = handleSubmit((data) => onSubmit(data, imageFile));
  const previewUrl = imageFile ? URL.createObjectURL(imageFile) : getImageUrl(existingImageUrl);

  return (
    <Box component="form" onSubmit={submitForm}>
      <Typography variant="h6" sx={{ mb: 2 }}>{t.items.basicInfo}</Typography>
      <TextField
        fullWidth
        label={t.field.title}
        margin="normal"
        {...register("title")}
        error={!!errors.title}
        helperText={errors.title?.message}
      />
      <TextField
        fullWidth
        label={t.field.category}
        margin="normal"
        {...register("category")}
        error={!!errors.category}
        helperText={errors.category?.message}
      />
      <TextField
        fullWidth
        label={t.field.historicalPeriod}
        margin="normal"
        {...register("historicalPeriod")}
        error={!!errors.historicalPeriod}
        helperText={errors.historicalPeriod?.message}
      />
      <TextField
        fullWidth
        label={t.field.description}
        margin="normal"
        multiline
        rows={4}
        {...register("description")}
        error={!!errors.description}
        helperText={errors.description?.message}
      />

      <Divider sx={{ my: 3 }} />

      <Typography variant="h6" sx={{ mb: 2 }}>
        {mode === "update" ? t.items.photo : t.items.photoOptional}
      </Typography>
      {previewUrl && (
        <Box
          component="img"
          src={previewUrl}
          alt={t.items.preview}
          sx={{ width: "100%", maxHeight: 250, objectFit: "contain", borderRadius: 1, mb: 2 }}
        />
      )}
      <Box sx={{ mb: 2 }}>
        <Button variant="outlined" component="label">
          {existingImageUrl ? t.items.changePhoto : t.items.selectFile}
          <input
            type="file"
            hidden
            accept="image/*"
            onChange={(e) => {
              if (e.target.files && e.target.files[0]) {
                setImageFile(e.target.files[0]);
              }
            }}
          />
        </Button>
        {imageFile && <Typography component="span" sx={{ ml: 2 }}>{imageFile.name}</Typography>}
      </Box>

      <Divider sx={{ my: 3 }} />

      <Typography variant="h6" sx={{ mb: 2 }}>
        {mode === "update" ? t.items.metadata : t.items.metadataOptional}
      </Typography>
      <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
        {t.items.metadataHint}
      </Typography>
      <MetadataEditor
        fields={fields}
        register={register}
        errors={errors}
        onAdd={() => append({ key: "", value: "" })}
        onRemove={remove}
      />

      <Divider sx={{ my: 3 }} />

      <Box sx={{ display: "flex", gap: 2, justifyContent: "flex-end", mt: 2 }}>
        <Button variant="outlined" onClick={onCancel} disabled={submitting}>
          {t.items.cancel}
        </Button>
        <Button type="submit" variant="contained" disabled={submitting}>
          {submitting ? t.items.saving : submitLabel}
        </Button>
      </Box>
    </Box>
  );
}