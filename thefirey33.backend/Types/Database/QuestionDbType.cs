using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace thefirey33_backend.Types.Database
{
    public class QuestionDbType
    {
        /// <summary>
        /// The ID of the question asked on the website.
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// The question that the user asks.
        /// </summary>
        [MaxLength(1024)]
        public required string Question { get; init; }

        /// <summary>
        /// The response from Thefirey33.
        /// </summary>
        [MaxLength(1024)]
        public string? Response { get; init; }
    }
}