#nullable enable

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Shadop.Archmage
{
    /// <summary>
    /// Represents VCS version metadata.
    /// </summary>
    public class VersionInfo
    {
        /// <summary>The workspace name.</summary>
        [JsonProperty("workspace")]
        public string? Workspace { get; set; }

        /// <summary>Tags associated with this version.</summary>
        [JsonProperty("tags")]
        public List<string>? Tags { get; set; }

        /// <summary>The source control branch name.</summary>
        [JsonProperty("branch")]
        public string? Branch { get; set; }

        /// <summary>The full commit ID.</summary>
        [JsonProperty("id")]
        public string? ID { get; set; }

        /// <summary>The abbreviated commit ID.</summary>
        [JsonProperty("shortId")]
        public string? ShortID { get; set; }

        /// <summary>The commit timestamp.</summary>
        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>The commit message.</summary>
        [JsonProperty("message")]
        public string? Message { get; set; }

        /// <summary>The commit author.</summary>
        [JsonProperty("author")]
        public string? Author { get; set; }

        /// <summary>Working tree status entries.</summary>
        [JsonProperty("status")]
        public List<string>? Status { get; set; }

        /// <summary>Additional metadata.</summary>
        [JsonProperty("extra")]
        public Dictionary<string, object>? Extra { get; set; }
    }
}
