using System;
using System.Collections.Generic;

namespace dotnetapp.Models
{
    public class SuitRecommendationModel
    {
        // Personal Information
        public string FullName { get; set; }
        public string Email { get; set; }
        
        // Style Preferences
        public string Occasion { get; set; }             // e.g., "Work", "Casual", "Formal", "Party"
        public string PreferredStyle { get; set; }         // e.g., "Classic", "Trendy", "Minimalist", "Bold"
        public string ColorPreference { get; set; }        // e.g., "Black", "Navy", "Beige", "Bright Colors"
        public string FabricPreference { get; set; }       // e.g., "Cotton", "Wool", "Linen", "Velvet"

        // Suit Combination Recommendations
        public string SuggestedPairings { get; set; }  // e.g., "Pair with jeans and a heel", "Add statement jewelry"

        // Additional Fields
        public string CreatedAt { get; set; }
    }
}
