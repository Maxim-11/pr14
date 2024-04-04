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
    public class TestResultsController : ControllerBase
    {
        private readonly ClassContext _context;

        public TestResultsController(ClassContext context)
        {
            _context = context;
        }

        // GET: api/TestResults
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TestResult>>> GetTestResults()
        {
            return await _context.TestResults.ToListAsync();
        }

        // GET: api/TestResults/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TestResult>> GetTestResult(int id)
        {
            var testResult = await _context.TestResults.FindAsync(id);

            if (testResult == null)
            {
                return NotFound();
            }

            return testResult;
        }

        // GET: api/TestResults/user/{userID}/assignment/{assignmentID}
        [HttpGet("user/{userID}/assignment/{assignmentID}")]
        public async Task<ActionResult<IEnumerable<TestResult>>> GetTestResultsForUserAndAssignment(int userID, int assignmentID)
        {
            Console.WriteLine($"Received GET request for test results of user with ID {userID} for assignment with ID {assignmentID}");

            var testResults = await _context.TestResults.Where(tr => tr.UserID == userID && tr.AssignmentID == assignmentID).ToListAsync();

            if (testResults == null || !testResults.Any())
            {
                Console.WriteLine($"No test results found for user with ID {userID} and assignment with ID {assignmentID}");
                return NotFound();
            }

            Console.WriteLine($"Returning test results for user with ID {userID} and assignment with ID {assignmentID}");

            foreach (var result in testResults)
            {
                Console.WriteLine($"Test result: {result}");
            }

            return testResults;
        }


        // PUT: api/TestResults/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTestResult(int id, TestResult testResult)
        {
            if (id != testResult.TestResultID)
            {
                return BadRequest();
            }

            _context.Entry(testResult).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TestResultExists(id))
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

        // POST: api/TestResults
        [HttpPost]
        public async Task<ActionResult<TestResult>> PostTestResult(TestResult testResult)
        {
            _context.TestResults.Add(testResult);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTestResult", new { id = testResult.TestResultID }, testResult);
        }

        // DELETE: api/TestResults/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTestResult(int id)
        {
            var testResult = await _context.TestResults.FindAsync(id);
            if (testResult == null)
            {
                return NotFound();
            }

            _context.TestResults.Remove(testResult);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TestResultExists(int id)
        {
            return _context.TestResults.Any(e => e.TestResultID == id);
        }
    }
}
