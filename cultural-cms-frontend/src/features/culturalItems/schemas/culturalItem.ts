import {z} from "zod";
import t from "@/locales/el"

export const metadataItemSchema = z.object({
  key: z.string()
    .min(1, { message: t.validation.required })
    .max(100, { message: t.validation.metadataKey }),
  value: z.string()
    .min(1, { message: t.validation.required })
    .max(500, { message: t.validation.metadataValue }),
});

export const culturalItemSchema = z.object({
  title: z.string()
    .min(1, { message: t.validation.required })
    .min(2, { message: t.validation.title })
    .max(200, { message: t.validation.title }),
  description: z.string()
    .min(1, { message: t.validation.required })
    .max(2000, { message: t.validation.description }),
  category: z.string()
    .min(1, { message: t.validation.required })
    .max(100, { message: t.validation.category }),
  historicalPeriod: z.string()
    .min(1, { message: t.validation.required })
    .max(100, { message: t.validation.historicalPeriod }),
  imageUrl: z.string().optional(),
  metadata: z.array(metadataItemSchema),
});

export type CulturalItemFormData = z.infer<typeof culturalItemSchema>;