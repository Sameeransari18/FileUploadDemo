using System.ComponentModel.DataAnnotations.Schema;

namespace FileDemo.Models
{
    public class attachmentModel
    {
        public string FileName { get; set; }
        public string Description { get; set; }
        [NotMapped]
        public IFormFile attachment { get; set; }
        public List<attachment> attachments { get; set; }
    }
}
    