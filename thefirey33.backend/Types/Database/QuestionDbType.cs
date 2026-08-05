using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace thefirey33_backend.Types.Database
{
    public class QuestionDbType
    {
        /// <summary>
        /// The ID of the question asked on the website.
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; init; }


        /// <summary>
        /// The post time of the question.
        /// </summary>
        [JsonPropertyName("time")]
        public DateTime QuestionPostTime { get; init; }

        /// <summary>
        /// The question that the user asks.
        /// </summary>
        [MaxLength(1024)]
        [JsonPropertyName("question")]
        public required string Question { get; init; }

        /// <summary>
        /// The attached image.
        /// </summary>
        [JsonPropertyName("attachment")]
        [MaxLength(256)]
        public required string Attachment { get; set; }

        /// <summary>
        /// The response from Thefirey33.
        /// </summary>
        [MaxLength(1024)]
        public string? Response { get; init; }
    }
}