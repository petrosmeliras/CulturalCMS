using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CulturalCMS.Application.Common
{
    public class PaginatedResult<T>
    {
        public List<T> Data { get; set; } = [];
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);

        public PaginatedResult()
        { 
        }

        public PaginatedResult(List<T> data, int  totalRecords, int pageNumber, int pageSize)
        {
            Data = data;
            TotalRecords = totalRecords;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
  
    }
}
