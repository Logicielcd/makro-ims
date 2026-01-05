using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Xml.Serialization;
using DR.Models;

namespace DR.Controllers
{
    //[Route("v1/drs/[controller]")]
    //[ApiController]    
    public class HomeController : Controller
    {
        
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("v1/drs")]
        public async Task<IActionResult> DR()
        {
            var request = this.HttpContext.Request;
            var contentType = this.HttpContext.Request.ContentType;


            if (string.IsNullOrEmpty(contentType) || contentType == "text/xml")
            {
                using (var reader = new StreamReader(request.Body))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Message));
                    string content = await reader.ReadToEndAsync();  
                    using(StringReader str_reader = new StringReader(content)){
                        Message msg = (Message)(serializer.Deserialize(str_reader));  
                        var result = msg.Rsr.Rsr_detail.Status; 
                        var status = msg.Rsr.Rsr_detail.Code;
                        var messageId = msg.Id;


                        //put your code here//
                        using (var writer = new StreamWriter(@"C:\DRLog\Log.txt", true))
                        {
                            writer.WriteLine("Result Status : " + result + " => Code : " + status + " => Message ID : " + messageId);
                        }

                    }
                }              
            }
            return new OkObjectResult("");
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
