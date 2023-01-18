using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileDemo.Models
{
    public class attachment
    {
        [Key]
        public int ID { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public string FileName { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public string Description { get; set; }
        [Column(TypeName = "varbinary(MAX)")]
        public byte[] file { get; set; }
    }
}
