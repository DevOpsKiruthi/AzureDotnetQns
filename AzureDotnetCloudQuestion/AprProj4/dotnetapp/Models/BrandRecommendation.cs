using System.ComponentModel.DataAnnotations;

namespace dotnetapp.Models
{
    public class BrandRecommendation
    {
        public int ClientID { get; set; }
        public string ClientName { get; set; }
        public string Email { get; set; }
        public string CampaignType { get; set; }
        public string TargetAudience { get; set; }
        public string CoreMessage { get; set; }
        public string SuggestedChannels { get; set; }
        public string CreatedAt { get; set; }
    }
}
