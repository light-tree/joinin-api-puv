﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.Google
{
    public class GoogleUserResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("verified_email")]
        public bool VerifiedEmail { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("given_name")]
        public string GivenName { get; set; }

        [JsonPropertyName("family_name")]
        public string FamilyName { get; set; }

        [JsonPropertyName("picture")]
        public string Picture { get; set; }

        [JsonPropertyName("locale")]
        public string Locale { get; set; }

        [JsonPropertyName("hd")]
        public string Hd { get; set; }
    }
}
