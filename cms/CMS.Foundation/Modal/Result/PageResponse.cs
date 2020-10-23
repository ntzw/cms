using System.Collections.Generic;

namespace Foundation.Modal.Result
{
    public class PageResponse : ResultBase
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

    public class PageResponse<T> : PageResponse
    {
        public PageResponse()
        {
        }
        
        public PageResponse(IEnumerable<T> data, long total)
        {
            Total = total;
            Data = data;
            IsSuccess = true;
        }

        public new IEnumerable<T> Data { get; set; }

        public new static PageResponse<T> Error(string message)
        {
            return new PageResponse<T>
            {
                Message = message,
                IsSuccess = false,
            };
        }
    }
}