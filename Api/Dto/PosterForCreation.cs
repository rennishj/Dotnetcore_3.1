using System.ComponentModel.DataAnnotations;

namespace Api.Dto
{
    /// <summary>
    /// Just used for creating a poster
    /// </summary>
    public class PosterForCreation
    {        
        public int Id { get; set; }

        [Required]
        public int MovieId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [Required]
        public byte[] Bytes { get; set; }
    }

    /// <summary>
    /// This gets sent to the clients
    /// </summary>
    public class Poster
    {
        public int Id { get; set; }        
        public int MovieId { get; set; }        
        public string Name { get; set; }      
        public byte[] Bytes { get; set; }
    }
}


