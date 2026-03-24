using API_Workshop.Database;
using Microsoft.AspNetCore.Mvc;

namespace API_Workshop.Controllers
{
    [Route("api/[Controller]")]
    public class CourseController(AppDbContext _context) : ControllerBase
    {
     
    }
}
