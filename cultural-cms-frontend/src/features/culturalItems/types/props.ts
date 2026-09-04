import type { CulturalItem } from "./domain";
import type { CulturalItemFormData } from "../schemas/culturalItem";
import type { UseFormRegister, FieldErrors, FieldArrayWithId } from "react-hook-form";

export type StatusBadgeProps = {
  status: string;
};

export type AuditTimelineProps = {
  entityId: number;
  lastUpdate: string;
};

export type ItemCardProps = {
  item: CulturalItem;
};

export type ItemTableProps = {
  items: CulturalItem[];
};

export type SearchFilterFields = {
  searchTerm: string;
  category: string;
  historicalPeriod: string;
  status: string;
  metadataKey: string;
  metadataValue: string;
};

export type SearchFiltersProps = {
  showStatusFilter?: boolean;
  showSortControls?: boolean;
  onSearch: (filters: SearchFilterFields) => void;
  sortBy: string;
  sortOrder: "asc" | "desc";
  onSortChange: (sortBy: string, sortOrder: "asc" | "desc") => void;
};

export type AdvancedSearchFieldsProps = {
  filters: SearchFilterFields;
  onFieldChange: (field: keyof SearchFilterFields, value: string) => void;
  onStatusChange: (value: string) => void;
  showStatusFilter?: boolean;
  onReset: () => void;
};

export type CulturalItemFormProps = {
  mode: "create" | "update";
  defaultValues?: Partial<CulturalItemFormData>;
  existingImageUrl?: string;
  submitLabel: string;
  submitting: boolean;
  onCancel: () => void;
  onSubmit: (data: CulturalItemFormData, imageFile: File | null) => void;
};

export type MetadataEditorProps = {
  fields: FieldArrayWithId<CulturalItemFormData, "metadata">[];
  register: UseFormRegister<CulturalItemFormData>;
  errors?: FieldErrors<CulturalItemFormData>;
  onAdd: () => void;
  onRemove: (index: number) => void;
};