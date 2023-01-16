using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Budget_Manager.Models
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "Nazwa jest wymagana!")]
        public string Title { get; set; }

        [Column(TypeName = "nvarchar(10)")]
        public string Icon { get; set; } = "";

        [Column(TypeName = "nvarchar(10)")]
        public string Type { get; set; } = "Expenses";

        [NotMapped]
        public string? TitleWithIcon 
        {
            get
            {
                return this.Icon + " " + this.Title;
            }
        }
    }
}
