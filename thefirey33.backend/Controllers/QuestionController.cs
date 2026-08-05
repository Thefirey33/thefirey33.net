using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using thefirey33_backend.Types.Database;
using thefirey33_backend.Types.Database.Context;

namespace thefirey33_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QuestionController(QuestionContext questionContext) : ControllerBase
    {

        /// <summary>
        /// Get all the current questions that aren't pending.
        /// </summary>
        /// <returns>All questions that aren't pending.</returns>
        [HttpGet]
        public async Task<List<QuestionDbType>> GetAllAvailable()
        {
            return await questionContext
            .Questions
            .Where(predicate => predicate.Response != null)
            .ToListAsync();
        }


        /// <summary>
        /// Get all the questions.
        /// </summary>
        /// <returns>All questions.</returns>
        [Authorize]
        [HttpGet("all")]
        public async Task<List<QuestionDbType>> GetAll()
        {
            return await questionContext.Questions.ToListAsync();
        }
    }
}