using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace WebBulky_RP.Models
{
    public class Category
    {
        [Key] //Data Annotation
        public int Id { get; set; }
        [Required] //Data Annotation
        [DisplayName("Category Name")] //Data Annotation {Actually looks like anything inside [..] is Data Annotation!}
        [MaxLength(30)]
        public string? Name { get; set; }
        [DisplayName("Display Order")]
        [Range(1, 100, ErrorMessage = "Display Order must be between 1-100!")]
        public int DisplayOrder { get; set; }
    }
}
