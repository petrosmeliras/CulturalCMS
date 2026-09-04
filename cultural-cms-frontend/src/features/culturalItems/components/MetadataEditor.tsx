import { Box, Button, TextField } from "@mui/material";
import type { MetadataEditorProps } from "@/features/culturalItems/types/props";
import t from "@/locales/el";

export default function MetadataEditor({ fields, register, errors, onAdd, onRemove }: MetadataEditorProps) {
  return (
    <>
      {fields.map((field, index) => (
        <Box key={field.id} sx={{ display: "flex", gap: 2, mb: 2, alignItems: "flex-start" }}>
          <TextField
            label={t.field.key}
            size="small"
            {...register(`metadata.${index}.key`)}
            error={!!errors?.metadata?.[index]?.key}
            helperText={errors?.metadata?.[index]?.key?.message}
          />
          <TextField
            label={t.field.value}
            size="small"
            {...register(`metadata.${index}.value`)}
            error={!!errors?.metadata?.[index]?.value}
            helperText={errors?.metadata?.[index]?.value?.message}
          />
          <Button color="error" onClick={() => onRemove(index)}>
            {t.items.removeMetadata}
          </Button>
        </Box>
      ))}

      <Button variant="text" onClick={onAdd}>
        {t.items.addMetadata}
      </Button>
    </>
  );
}