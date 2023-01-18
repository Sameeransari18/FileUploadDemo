using FileDemo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using FileDemo.SPClass;
using Microsoft.Net.Http.Headers;
using System.Net.Mail;
using System;

namespace FileDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly AppDbContext _appDbContext;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment hostingEnvironment, AppDbContext appDbContext)
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
            _appDbContext = appDbContext;
        }

        public IActionResult Index()
        {

            attachmentModel model = new attachmentModel();
            model.attachments = _appDbContext.attachments.Select(m => m).ToList();  
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(attachmentModel model)
        {
            if (model.attachment != null)
            {
                try
                {
                    // Saving file to physical storage
                    var uniqueFileName = SPClass.SPClass.CreateUniqueFileExtension(model.attachment.FileName);
                    var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "attachment");
                    var filePath = Path.Combine(uploads, uniqueFileName);
                    model.attachment.CopyTo(new FileStream(filePath, FileMode.Create));

                    // Save file to database
                    attachment attachment = new attachment();
                    attachment.FileName = uniqueFileName;
                    attachment.Description = model.Description;
                    attachment.file = SPClass.SPClass.GetArrayFromImage(model.attachment);

                    _appDbContext.attachments.Add(attachment);
                    _appDbContext.SaveChanges();
                }   
                catch (Exception)
                {

                    throw;
                }
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public FileResult GetFileResultDemo(string filename)
        {
            string path = "/attachment/" + filename;
            string contentType = SPClass.SPClass.GetContentType(filename);
            return File(path, contentType);
        }

        [HttpGet]
        public FileContentResult GetFileContentResultDemo(string filename)
        {
            string path = "wwwroot/attachment/" + filename;
            byte[] fileContent = System.IO.File.ReadAllBytes(path);
            string contentType = SPClass.SPClass.GetContentType(filename);
            return new FileContentResult(fileContent, contentType);
        }

        [HttpGet]
        public FileStreamResult GetFileStreamResultDemo(string filename) //download file
        {
            string path = "wwwroot/attachment/" + filename;
            var stream = new MemoryStream(System.IO.File.ReadAllBytes(path));
            string contentType = SPClass.SPClass.GetContentType(filename);
            return new FileStreamResult(stream, new MediaTypeHeaderValue(contentType))
            {
                FileDownloadName = filename
            };
        }

        [HttpGet]
        public VirtualFileResult GetVirtualFileResultDemo(string filename)
        {
            string path = "attachment/" + filename;
            string contentType = SPClass.SPClass.GetContentType(filename);
            return new VirtualFileResult(path, contentType);
        }

        [HttpGet]
        public PhysicalFileResult GetPhysicalFileResultDemo(string filename)
        {
            string path = "/wwwroot/attachment/" + filename;
            string contentType = SPClass.SPClass.GetContentType(filename);
            return new PhysicalFileResult(_hostingEnvironment.ContentRootPath
                + path, contentType);
        }

        [HttpGet]
        public ActionResult GetAttachment(int ID)
        {
            byte[] fileContent;
            string fileName = string.Empty;
            attachment attachment = new attachment();
            attachment = _appDbContext.attachments.Select(m => m).Where(m => m.ID == ID).FirstOrDefault();

            string contentType = SPClass.SPClass.GetContentType(attachment.FileName);
            fileContent = (byte[])attachment.file;
            return new FileContentResult(fileContent, contentType);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        
    }
}