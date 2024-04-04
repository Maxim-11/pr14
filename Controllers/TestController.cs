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
    public class TestsController : ControllerBase
    {
        private readonly ClassContext _context;

        public TestsController(ClassContext context)
        {
            _context = context;
        }

        // GET: api/Tests
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Test>>> GetTests()
        {
            return await _context.Tests.ToListAsync();
        }

        // GET: api/Tests/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Test>> GetTest(int id)
        {
            var test = await _context.Tests.FindAsync(id);

            if (test == null)
            {
                return NotFound();
            }

            return test;
        }

        // PUT: api/Tests/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTest(int id, Test test)
        {
            if (id != test.TestID)
            {
                return BadRequest();
            }

            _context.Entry(test).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TestExists(id))
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

        // POST: api/Tests
        [HttpPost]
        public async Task<ActionResult<Test>> PostTest(Test test)
        {
            _context.Tests.Add(test);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTest", new { id = test.TestID }, test);
        }

        // GET: api/Tests/Assignment/5
        [HttpGet("Assignment/{assignmentID}")]
        public async Task<ActionResult<IEnumerable<Test>>> GetTestsForAssignment(int assignmentID)
        {
            var tests = await _context.Tests.Where(t => t.AssignmentID == assignmentID).ToListAsync();

            if (tests == null || tests.Count == 0)
            {
                return NotFound();
            }

            return tests;
        }

        // GET: api/Tests/ByAssignment?assignmentId=5
        [HttpGet("ByAssignment")]
        public async Task<ActionResult<int>> GetNumberOfQuestionsByAssignment(int assignmentId)
        {
            var numberOfQuestions = await _context.Tests.CountAsync(test => test.AssignmentID == assignmentId);
            return numberOfQuestions;
        }


        // DELETE: api/Tests/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTest(int id)
        {
            var test = await _context.Tests.FindAsync(id);
            if (test == null)
            {
                return NotFound();
            }

            _context.Tests.Remove(test);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TestExists(int id)
        {
            return _context.Tests.Any(e => e.TestID == id);
        }
    }
}
