namespace IGDiscord.Models.Discord
{
    public class ButtonComponent
    {
        public int Type { get; set; }

        public int Style { get; set; }

        public string Label { get; set; }

        public Emoji Emoji { get; set; }

        public string CustomId{ get; set; }

        public string Url { get; set; }

        public bool Disabled { get; set; }
    }
}