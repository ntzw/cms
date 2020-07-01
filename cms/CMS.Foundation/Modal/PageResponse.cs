using System.Collections.Generic;

namespace Foundation.Modal
{
    public class PageResponse
    {
        public PageResponse(IEnumerable<dynamic> data, long total)
        {
            Total = total;
            Data = data;
        }
        
        public long Total { get; set; }

        public int Current { get; set; }

        public int Size { get; set; }

        public IEnumerable<dynamic> Data { get; set; }
    }
}