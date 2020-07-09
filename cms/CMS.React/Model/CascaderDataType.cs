using System.Collections.Generic;

namespace CMS.React.Model
{
    public class CascaderDataType : SelectDataType
    {
        public List<CascaderDataType> Children { get; set; }
    }
}