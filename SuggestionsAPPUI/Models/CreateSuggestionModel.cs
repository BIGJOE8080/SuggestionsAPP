using System.ComponentModel.DataAnnotations;

namespace SuggestionsAPPUI.Models
{
    public class CreateSuggestionModel
    {
        [Required]
        [MaxLength(length: 75)]
        public string suggestion { get; set; }
        [Required]
        [MinLength(length: 1)]
        [Display(Name = "Category")]   

        public string CategoryId { get; set; }

        [MaxLength(length: 500)]
        public string Description{ get; set; }

    }
}
