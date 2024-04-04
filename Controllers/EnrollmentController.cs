using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Class.Models;

namespace Class.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly ClassContext _context;

        public EnrollmentsController(ClassContext context)
        {
            _context = context;
        }

        // GET: api/Enrollments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Enrollment>>> GetEnrollments()
        {
            return await _context.Enrollments.ToListAsync();
        }

        // GET: api/Enrollments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Enrollment>> GetEnrollment(int id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);

            if (enrollment == null)
            {
                return NotFound();
            }

            return enrollment;
        }

        // PUT: api/Enrollments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEnrollment(int id, Enrollment enrollment)
        {
            if (id != enrollment.EnrollmentID)
            {
                return BadRequest();
            }

            _context.Entry(enrollment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EnrollmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Enrollments
        [HttpPost]
        public async Task<ActionResult<Enrollment>> PostEnrollment(Enrollment enrollment)
        {
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEnrollment", new { id = enrollment.EnrollmentID }, enrollment);
        }

        // DELETE: api/Enrollments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEnrollment(int id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }

            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EnrollmentExists(int id)
        {
            return _context.Enrollments.Any(e => e.EnrollmentID == id);
        }



        [HttpGet("ByUserId/{userId}")]
        public async Task<ActionResult<IEnumerable<Course>>> GetCoursesByUserId(int userId)
        {
            var enrollments = await _context.Enrollments
                .Where(e => e.UserID == userId)
                .ToListAsync();

            if (enrollments == null)
            {
                return NotFound();
            }

            var courseIds = enrollments.Select(e => e.CourseID).Distinct();

            var courses = await _context.Courses
                .Where(c => courseIds.Contains(c.CourseID))
                .ToListAsync();

            return courses;
        }
    }
}
