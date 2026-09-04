export type PaginatedResult<T> = {
  data: T[];
  totalRecords: number;
  pageNumber: number;
  pageSize: number;
};

export type LoadingSpinnerProps = {
  minHeight?: string | number;
};

export type PaginationControlsProps = {
  pageNumber: number;
  pageSize: number;
  totalRecords: number;
  onPageChange: (page: number) => void;
};