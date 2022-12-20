namespace NewsFeed.Models
{
    public class News
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ImgPath { get; set; }
        public string Article { get; set; }
        public int Category_Id { get; set; }
    }
}
