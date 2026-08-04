using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace thefirey33_backend.Types.Database.Context
{
    public class QuestionContext(DbContextOptions<QuestionContext> options) : DbContext(options)
    {

    }
}