using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Luyentap.Data;
using Luyentap.Models.Process;
using Luyentap.Models;


namespace Luyentap.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private ExcelsProcess _excelProcess = new ExcelsProcess();
        public StudentController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Students.ToListAsync());

        }
        public async Task<IActionResult> Upload()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file != null)
            {
                string fileExtension = Path.GetExtension(file.FileName);
                if (fileExtension != ".xls" && fileExtension != ".xlsx")
                {
                    ModelState.AddModelError("", "Please choose excel file to upload!");
                }
                else
                {
                    //rename
                    var fileName = DateTime.Now.ToShortTimeString() + fileExtension;
                    var filePath = Path.Combine(Directory.GetCurrentDirectory() + "/Uploads_Excels", fileName);
                    var fileLocation = new FileInfo(filePath).ToString();
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        //save file to server
                        await file.CopyToAsync(stream);

                        // read data from
                        var dt = _excelProcess.ExcelToDataTable(fileLocation);
                        //using for loop to read data from dt
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            //create a new Employee object
                            var std = new Student();
                            // set values for attrinutes
                            std.StudentID = dt.Rows[i][0].ToString();
                            std.StudentName = dt.Rows[i][1].ToString();

                            //add object to Context
                            _context.Students.Add(std);
                        }
                        //save to database 
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                }
            }

            return View();
        }
        private bool StudentExists(string id)
        {
            return _context.Students.Any(e => e.StudentID == id);
        }
    }
}