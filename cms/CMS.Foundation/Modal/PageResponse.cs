using System.Collections.Generic;

namespace Foundation.Modal
{
    public class PageResponse
    {
        public PageResponse()
        {
        }

        public PageResponse(IEnumerable<dynamic> data, long total)
        {
            Total = total;
            Data = data;
            IsSuccess = true;
        }

        public long Total { get; set; }

        public int Current { get; set; }

        public int Size { get; set; }

        public string Message { get; set; }

        public bool IsSuccess { get; set; }

        public IEnumerable<dynamic> Data { get; set; }

        public static PageResponse Error(string message)
        {
            return new PageResponse
            {
                Message = message,
                IsSuccess = false,
            };
        }
    }
}