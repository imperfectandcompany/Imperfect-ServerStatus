namespace ImperfectServerStatus.Models.Discord
{
    public class Emoji
    {
        public int? Id { get; set; }

        public string Name { get; set; }    

        public bool Animated { get; set; } = false;
    }
}