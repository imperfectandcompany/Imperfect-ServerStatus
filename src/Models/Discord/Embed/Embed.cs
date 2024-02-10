namespace IGDiscord.Models.Discord
{
    public class Embed
    {
        public string? Title { get; set; }

        public string? Type { get; set; } = "rich";

        public string? Description  { get; set; }

        public string? Url { get; set; }

        public DateTime Timestamp { get; set; }

        public int? Color  { get; set; }

        public EmbedFooter? Footer { get; set; }

        public EmbedImage? Image { get; set; }

        public EmbedThumbnail? Thumbnail { get; set; }

        public EmbedVideo? Video { get; set; }

        public EmbedProvider? Provider { get; set; }

        public EmbedAuthor? Author { get; set; }

        public EmbedFields[]? Fields { get; set; }
    }
}
