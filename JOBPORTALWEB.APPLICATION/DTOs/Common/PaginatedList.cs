using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace JOBPORTALWEB.APPLICATION.DTOs.Common
{
    // DTO này dùng để bọc dữ liệu trả về kèm thông tin phân trang
    public class PaginatedList<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }
        public int TotalCount { get; private set; }
        public int PageSize { get; private set; }
        public List<T> Items { get; private set; } = new List<T>();

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            TotalCount = count;
            PageSize = pageSize;
            Items = items;
        }
    }
}