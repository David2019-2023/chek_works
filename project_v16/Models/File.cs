namespace project_v16.Models
{
    public class File
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string MimeType { get; set; }
        public byte[] Content { get; set; }
        public int StudentId { get; set; }  
    }
}
