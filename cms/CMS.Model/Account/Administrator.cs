using Dapper.Contrib.Extensions;

namespace Model.Account
{
    [Table("Account_Administrator")]
    public class Administrator
    {
        [Key]
        public int Id { get; set; }
        
        public string Num { get; set; }
    }
}