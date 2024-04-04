using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace Class.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly string _uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");


        [HttpPost]
        public IActionResult UploadFile()
        {
            try
            {
                var file = Request.Form.Files[0]; // Получаем файл из запроса

                if (file.Length > 0)
                {
                    // Получаем оригинальное имя файла
                    var fileName = file.FileName;
                    var filePath = Path.Combine(_uploadsDirectory, fileName);

                    // Сохраняем файл на сервере
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    // Возвращаем путь к сохраненному файлу
                    return Ok(new { FilePath = filePath });
                }

                return BadRequest("File is empty");
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
