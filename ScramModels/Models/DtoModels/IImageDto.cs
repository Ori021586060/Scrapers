namespace ScraperModels.Model
{
    public interface IImageDto
    {
        string description { get; set; }
        string full { get; set; }
        string thumbnail { get; set; }
    }
}