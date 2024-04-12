using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Class.Models;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using System.Text.Json;
using Microsoft.VisualBasic;
using Aspose.Words;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using Aspose.Cells;

namespace Class.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubmissionsController : ControllerBase
    {
        private readonly ClassContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

        public SubmissionsController(ClassContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: api/Submissions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Submission>>> GetSubmissions()
        {
            return await _context.Submissions.ToListAsync();
        }

        // GET: api/Submissions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Submission>> GetSubmission(int id)
        {
            var submission = await _context.Submissions.FindAsync(id);

            if (submission == null)
            {
                return NotFound();
            }

            return submission;
        }


        // POST: api/Submissions
        [HttpPost]
        public async Task<ActionResult<Submission>> PostSubmission(Submission submission)
        {
            _context.Submissions.Add(submission);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSubmission", new { id = submission.SubmissionID }, submission);
        }

        // DELETE: api/Submissions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubmission(int id)
        {
            var submission = await _context.Submissions.FindAsync(id);
            if (submission == null)
            {
                return NotFound();
            }

            _context.Submissions.Remove(submission);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Submissions/DeletePreviousSubmissions/{assignmentID}/{userID}
        [HttpDelete("DeletePreviousSubmissions/{assignmentID}/{userID}")]
        public async Task<IActionResult> DeletePreviousSubmissions(int assignmentID, int userID)
        {
            var submissions = await _context.Submissions
                .Where(s => s.AssignmentID == assignmentID && s.UserID == userID)
                .ToListAsync();

            if (submissions == null || submissions.Count == 0)
            {
                return NotFound();
            }

            _context.Submissions.RemoveRange(submissions);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        // GET: api/Submissions/{assignmentId}
        [HttpGet("ByAssignment/{assignmentId}")]
        public async Task<ActionResult<IEnumerable<Submission>>> GetSubmissionsByAssignmentId(int assignmentId)
        {
            var submissions = await _context.Submissions
                .Where(s => s.AssignmentID == assignmentId)
                .ToListAsync();

            if (submissions == null)
            {
                return NotFound();
            }

            return submissions;
        }


        // PUT: api/Submissions/UpdateSubmissionGrade/{submissionID}
        [HttpPut("UpdateSubmissionGrade/{submissionID}")]
        public async Task<IActionResult> UpdateSubmissionGrade(int submissionID, int grade)
        {
            try
            {
                var submission = await _context.Submissions.FindAsync(submissionID);
                if (submission == null)
                {
                    return NotFound();
                }

                // Добавим логирование перед обновлением оценки
                Console.WriteLine($"Updating submission grade for ID: {submissionID}, new grade: {grade}");

                submission.Grade = grade;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception e)
            {
                // В случае ошибки также добавим логирование
                Console.WriteLine($"Error updating submission grade: {e.Message}");
                return StatusCode(500, $"Error updating submission grade: {e.Message}");
            }
        }

        // GET: api/Submissions/ByUserId/{userID}
        [HttpGet("ByUserId/{userID}")]
        public async Task<ActionResult<IEnumerable<Submission>>> GetSubmissionsByUserId(int userID)
        {
            var submissions = await _context.Submissions
                .Where(s => s.UserID == userID)
                .ToListAsync();

            if (submissions == null)
            {
                return NotFound();
            }

            return submissions;
        }


        // GET: api/Submissions/ByUserAndAssignment?userId={userId}&assignmentId={assignmentId}
        [HttpGet("ByUserAndAssignment")]
        public async Task<ActionResult<Submission>> GetSubmissionForUser(int userId, int assignmentId)
        {
            var submission = await _context.Submissions.FirstOrDefaultAsync(s => s.UserID == userId && s.AssignmentID == assignmentId);

            if (submission == null)
            {
                return NotFound();
            }

            return submission;
        }

        // PUT: api/Submissions/UpdateSubmissionPath/{submissionID}
        [HttpPut("UpdateSubmissionPath/{submissionID}")]
        public async Task<IActionResult> UpdateSubmissionPath(int submissionID, string filePath)
        {
            try
            {
                var submission = await _context.Submissions.FindAsync(submissionID);
                if (submission == null)
                {
                    return NotFound();
                }

                // Обновляем только путь файла
                submission.FilePath = filePath;

                // Сохраняем изменения в базе данных
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception e)
            {
                // В случае ошибки добавим логирование
                Console.WriteLine($"Error updating submission path: {e.Message}");
                return StatusCode(500, $"Error updating submission path: {e.Message}");
            }
        }

        // GET: api/Submissions/ByFileName/{fileName}
        [HttpGet("ByFileName/{fileName}")]
        public IActionResult GetFile(string fileName)
        {
            // Получаем путь к файлу на сервере
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            // Определяем тип MIME файла
            string contentType = "application/octet-stream"; // По умолчанию

            // Определяем тип MIME на основе расширения файла
            if (fileName.EndsWith(".pdf"))
            {
                contentType = "application/pdf";
            }
            else if (fileName.EndsWith(".docx"))
            {
                contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            }
            // Добавьте другие типы MIME по мере необходимости

            // Отправляем файл клиенту с правильным типом MIME и заголовком Content-Disposition
            return PhysicalFile(filePath, contentType, fileName);
        }


        [HttpPost("UploadFile")]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is empty");
            }

            try
            {
                // Путь для сохранения файла
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Оригинальное имя файла
                string fileName = file.FileName;
                string filePath = Path.Combine(uploadsFolder, fileName);
                // Сохраняем файл на сервере
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                Console.WriteLine($"Путь файла: {fileName}");

                string extension = Path.GetExtension(fileName).ToLower();
                if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".gif")
                {
                    // Показать изображение
                    return Ok(new { fileName, filePath });
                }
                else if (extension != ".pdf" && extension != ".mp4" && extension != ".avi" && extension != ".mov")
                {
                    // Если файл не в формате PDF, конвертируем его
                    string pdfFilePath = await ConvertToPdf(filePath);
                    if (pdfFilePath == null)
                    {
                        return BadRequest("Failed to convert file to PDF");
                    }

                    // Удаляем исходный файл
                    System.IO.File.Delete(filePath);

                    // Обновляем путь и имя файла
                    fileName = Path.GetFileName(pdfFilePath);
                    filePath = pdfFilePath;
                    Console.WriteLine($"fileName: {fileName}");
                    Console.WriteLine($"filePath: {filePath}");
                }

                // Возвращаем путь к загруженному файлу и имя файла
                return Ok(new { fileName, filePath });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }




        private async Task<string> ConvertToPdf(string filePath)
        {
            try
            {
                // Проверяем расширение файла
                string extension = Path.GetExtension(filePath).ToLower();
                if (extension == ".docx" || extension == ".doc")
                {
                    // Создаем документ Word из полученного файла
                    Document doc = new Document(filePath);

                    // Определяем путь для сохранения PDF
                    string pdfFilePath = Path.ChangeExtension(filePath, ".pdf");

                    // Сохраняем документ в формате PDF
                    doc.Save(pdfFilePath, Aspose.Words.SaveFormat.Pdf);

                    // Возвращаем путь к новому PDF файлу
                    return pdfFilePath;
                }
                else if (extension == ".xlsx" || extension == ".xls")
                {
                    // Открываем документ Excel
                    Workbook workbook = new Workbook(filePath);

                    // Определяем путь для сохранения PDF
                    string pdfFilePath = Path.ChangeExtension(filePath, ".pdf");

                    // Сохраняем документ в формате PDF
                    workbook.Save(pdfFilePath, Aspose.Cells.SaveFormat.Pdf);

                    // Возвращаем путь к новому PDF файлу
                    return pdfFilePath;
                }
                else
                {
                    // Если файл уже в формате PDF или не является Word или Excel файлом, просто возвращаем исходный путь
                    return filePath;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error converting file to PDF: {ex.Message}");
                return null;
            }
        }



        [HttpPut("updateUserFilePath/{submissionId}")]
        public async Task<IActionResult> UpdateUserFilePath(int submissionId, [FromBody] Submission submissionData)
        {
            try
            {
                var submission = await _context.Submissions.FindAsync(submissionId);
                if (submission == null)
                {
                    return NotFound();
                }

                // Update user file path
                submission.FilePath = submissionData.FilePath;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception e)
            {
                // Log error
                Console.WriteLine($"Error updating user file path: {e.Message}");
                return StatusCode(500, $"Error updating user file path: {e.Message}");
            }
        }


       



        private bool SubmissionExists(int id)
        {
            return _context.Submissions.Any(e => e.SubmissionID == id);
        }
    }
}
