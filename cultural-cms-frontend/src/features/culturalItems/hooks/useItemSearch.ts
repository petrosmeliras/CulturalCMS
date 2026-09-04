import t from "@/locales/el";
import { useEffect, useState } from "react";
import { useSearchParams } from "react-router";
import type { CulturalItem } from "@/features/culturalItems/types/domain";
import type { SearchFilterFields } from "@/features/culturalItems/types/props";
import type { PaginatedResult } from "@/shared/types";
import { searchCulturalItems, searchCulturalItemsAllStatuses, searchMyItems } from "@/features/culturalItems/api/culturalItems";

const PAGE_SIZE = 10;

const ERROR_MESSAGE_BY_SCOPE = {
  public: t.items.listError,
  all: t.items.listError,
  mine: t.items.myItemsError,
} as const;

export function useItemSearch(scope: "public" | "all" | "mine") {
  const [searchParams, setSearchParams] = useSearchParams();

  const searchTerm = searchParams.get("searchTerm") || "";
  const category = searchParams.get("category") || "";
  const historicalPeriod = searchParams.get("historicalPeriod") || "";
  const status = searchParams.get("status") || "";
  const metadataKey = searchParams.get("metadataKey") || "";
  const metadataValue = searchParams.get("metadataValue") || "";
  const sortBy = searchParams.get("sortBy") || "CreatedAt";
  const sortOrder = (searchParams.get("sortOrder") as "asc" | "desc") || "desc";
  const pageNumber = Number(searchParams.get("pageNumber")) || 1;

  const [items, setItems] = useState<CulturalItem[]>([]);
  const [totalRecords, setTotalRecords] = useState(0);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchItems = async () => {
      setLoading(true);
      setError(null);

      const baseParams = {
        searchTerm: searchTerm || undefined,
        category: category || undefined,
        historicalPeriod: historicalPeriod || undefined,
        metadataKey: metadataKey || undefined,
        metadataValue: metadataValue || undefined,
        sortBy: sortBy,
        sortOrder: sortOrder,
        pageNumber: pageNumber,
        pageSize: PAGE_SIZE,
      };

      try {
        const result: PaginatedResult<CulturalItem> = scope === "all"
          ? await searchCulturalItemsAllStatuses({ ...baseParams, status: status || undefined })
          : scope === "mine"
            ? await searchMyItems({ ...baseParams, status: status || undefined })
            : await searchCulturalItems(baseParams);

        setItems(result.data);
        setTotalRecords(result.totalRecords);
      } catch (error) {
        console.error(error);
        setError(ERROR_MESSAGE_BY_SCOPE[scope]);
      } finally {
        setLoading(false);
      }
    };

    fetchItems();
  }, [scope, searchTerm, category, historicalPeriod, status, metadataKey, metadataValue, sortBy, sortOrder, pageNumber]);

  const updateParams = (updates: Record<string, string | undefined>) => {
    setSearchParams((prev) => {
      const next = new URLSearchParams(prev);
      for (const [key, value] of Object.entries(updates)) {
        if (value) {
          next.set(key, value);
        } else {
          next.delete(key);
        }
      }
      return next;
    });
  };

  const applyFilters = (filters: SearchFilterFields) => {
    updateParams({ ...filters, pageNumber: "1" });
  };

  const applySort = (newSortBy: string, newSortOrder: "asc" | "desc") => {
    updateParams({ sortBy: newSortBy, sortOrder: newSortOrder, pageNumber: "1" });
  };

  const setPageNumber = (page: number) => {
    updateParams({ pageNumber: String(page) });
  };

  return {
    items, totalRecords, loading, error,
    pageNumber, pageSize: PAGE_SIZE,
    sortBy, sortOrder,
    setPageNumber,
    applyFilters,
    applySort,
  };
}