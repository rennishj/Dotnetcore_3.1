namespace Api.Dto
{
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


