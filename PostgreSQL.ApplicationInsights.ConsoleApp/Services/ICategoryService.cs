using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PostgreSQL.ApplicationInsights.ConsoleApp.Models;

namespace PostgreSQL.ApplicationInsights.ConsoleApp.Services
{
    public interface ICategoryService
    {
        Category GetCategoryById(int id);
        IEnumerable<Category> GetAll();
    }
}