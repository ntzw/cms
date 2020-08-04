using System.Collections.Generic;

namespace Foundation.Modal.RequestModal
{
    public interface ISelectRequest
    {
        public int Top { get; set; }
        
        List<IQuery> Queries { get; set; }
        
        ISort Sort { get; set; }
        
        bool ContainsQueryField(string field);

        IQuery GetQueryField(string filed);
    }
}