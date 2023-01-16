using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Budget_Manager.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionID { get; set; }

        // Specify Category
        [Range(1,int.MaxValue,ErrorMessage = "Wybierz kategorię!")]
        public int CategoryID { get; set; }
        public Category? Category { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Kwota nie może być pusta ani równa 0!")]
        public int Amount { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        [Column(TypeName = "nvarchar(80)")]
        public string? Description { get; set; }

        [NotMapped]
        public string? CategoryTitleWithIcon 
        { 
            get
            {
                return Category == null ? "" : Category.Icon + " " + Category.Title;
            }
        }

        [NotMapped]
        public string? FormattedAmount
        {
            get
            {
                return ((Category == null || Category.Type == "Wydatki") ? "- " : "+ ") + Amount.ToString("C0");
            }
        }
    }
}
