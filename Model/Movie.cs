using System;

namespace Entity
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Genre { get; set; }
        public DateTimeOffset ReleaseDate { get; set; }
        public string Director { get; set; }
        public DateTime CreatedOn { get; set; }

    }
}
