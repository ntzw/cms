using System.Collections.Generic;

namespace Foundation.Modal.RequestModal
{
    public interface IPageRequest
    {
        long Current { get; set; }

        long Size { get; set; }

        List<IQuery> Queries { get; set; }
        
        ISort Sort { get; set; }

        /// <summary>
        ///     分页开始
        /// </summary>
        long Begin { get; }

        /// <summary>
        ///     分页截止
        /// </summary>
        long End { get; }
    }
}