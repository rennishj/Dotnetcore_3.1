using System;
using System.ComponentModel.DataAnnotations;

namespace Api.Dto
{
    public class MovieForCreation
    {       
        [Required]
        [MaxLength(500)]
        public string Title { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        [MaxLength(10)]
        public string Genre { get; set; }

        [Required]        
        public DateTimeOffset ReleaseDate { get; set; }

        [Required]
        public string Director { get; set; }        
    }
}
