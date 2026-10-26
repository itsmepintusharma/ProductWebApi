namespace ProductWebApi.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }

        public string Token { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime ExpiresAt { get; set; }

        public bool IsRevoked { get; set; }

        public string UserId { get; set; } = string.Empty;

        public ApplicationUser User { get; set; } = null!;
    }
}
