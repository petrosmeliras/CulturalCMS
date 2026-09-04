import type { AuditLog } from "../types/domain";
import t from "@/locales/el";

// oldValues/newValues are serialized server-side with raw JsonSerializer.Serialize(),
// which uses PascalCase (C# property names), unlike the rest of the API which is camelCase.
type RawItemSnapshot = {
  Title?: string;
  Description?: string;
  Category?: string;
  HistoricalPeriod?: string;
  Status?: string;
  Metadata?: { Key: string; Value: string }[];
};

const FIELD_LABELS: Record<string, string> = {
  Title: t.field.title,
  Description: t.field.description,
  Category: t.field.category,
  HistoricalPeriod: t.field.historicalPeriod,
};

function metadataEntryText(entry: { Key: string; Value: string }): string {
  const label = entry.Key === "Tag" ? t.items.tagLabel : entry.Key;
  return `${label}: ${entry.Value}`;
}

function diffMetadata(oldList: RawItemSnapshot["Metadata"] = [], newList: RawItemSnapshot["Metadata"] = []): string[] {
  const oldEntries = (oldList ?? []).map(metadataEntryText);
  const newEntries = (newList ?? []).map(metadataEntryText);

  const remaining = [...oldEntries];
  const added: string[] = [];

  newEntries.forEach((entry) => {
    const idx = remaining.indexOf(entry);
    if (idx !== -1) {
      remaining.splice(idx, 1);
    } else {
      added.push(entry);
    }
  });

  return [
    ...added.map((e) => `${e}: ${t.audit.metadataAdded}`),
    ...remaining.map((e) => `${e}: ${t.audit.metadataRemoved}`),
  ];
}

export type FieldChange = {
  label: string;
  detail: string;
};

export type AuditChangeSummary = {
  transition: string | null;
  fields: FieldChange[];
  metadataChanges: string[];
};

export function getChangeSummary(log: AuditLog): AuditChangeSummary {
  const empty: AuditChangeSummary = { transition: null, fields: [], metadataChanges: [] };

  if (log.action === "StatusChange") {
    const oldObj: RawItemSnapshot = log.oldValues ? JSON.parse(log.oldValues) : {};
    const newObj: RawItemSnapshot = log.newValues ? JSON.parse(log.newValues) : {};
    const transition = t.statusTransitions[`${oldObj.Status}->${newObj.Status}`] ?? null;
    return { ...empty, transition };
  }

  if (log.action === "Update" && log.changedColumns) {
    const oldObj: RawItemSnapshot = log.oldValues ? JSON.parse(log.oldValues) : {};
    const newObj: RawItemSnapshot = log.newValues ? JSON.parse(log.newValues) : {};
    const columns = log.changedColumns.split(",").map((f) => f.trim());

    const fields: FieldChange[] = [];
    let metadataChanges: string[] = [];

    columns.forEach((field) => {
      if (field === "Metadata") {
        metadataChanges = diffMetadata(oldObj.Metadata, newObj.Metadata);
      } else {
        const label = FIELD_LABELS[field] ?? field;
        fields.push({
          label,
          detail: `${oldObj[field as keyof RawItemSnapshot]} → ${newObj[field as keyof RawItemSnapshot]}`,
        });
      }
    });

    return { transition: null, fields, metadataChanges };
  }

  return empty;
}