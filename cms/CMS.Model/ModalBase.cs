using System;
using Dapper.Contrib.Extensions;
using Extension;
using Helper;

namespace Model
{
    public class ModalBase
    {
        [Key] public int Id { get; set; }

        public string Num { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }
        
        public string CreateAccountNum { get; set; }
        
        public string UpdateAccountNum { get; set; }

        public virtual void Init()
        {
            if (this.Num.IsEmpty())
                this.Num = RandomHelper.CreateNum();
            
            if(CreateDate.Year == 1)
                CreateDate = DateTime.Now;
            
            if(UpdateDate.Year == 1)
                UpdateDate = DateTime.Now;
        }
    }
}