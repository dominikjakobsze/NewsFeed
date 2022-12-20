namespace NewsFeed.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int News_Id { get; set; }
        public string User_Id { get; set; }
    }
}
