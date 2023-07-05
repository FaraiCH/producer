using Aspose.Cells;
using Aspose.Pdf;
using Aspose.Pdf.Text;
using Aspose.Slides;
using Aspose.Words;
using Aspose.Words.Drawing;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using producer.Models;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Document = Aspose.Words.Document;

namespace producer.Controllers
{
    [EnableCors("MyPolicy")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {


            _logger = logger;
        }
        public static void Exec(string cmd)
        {
            var escapedArgs = cmd.Replace("\"", "\\\"");

            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escapedArgs}\""
                }
            };

            process.Start();
            process.WaitForExit();
        }

        public IActionResult Index()
        {
     
            return View();
        }
        [HttpGet]
        public IActionResult JsonToExcel(string jsonData)
        {
            string LData = "PExpY2Vuc2U+CjxEYXRhPgo8TGljZW5zZWRUbz5BdmVQb2ludDwvTGljZW5zZWRUbz4KPEVtYWlsVG8+aXRfYmlsbGluZ0BhdmVwb2ludC5jb208L0VtYWlsVG8+CjxMaWNlbnNlVHlwZT5EZXZlbG9wZXIgT0VNPC9MaWNlbnNlVHlwZT4KPExpY2Vuc2VOb3RlPkxpbWl0ZWQgdG8gMSBkZXZlbG9wZXIsIHVubGltaXRlZCBwaHlzaWNhbCBsb2NhdGlvbnM8L0xpY2Vuc2VOb3RlPgo8T3JkZXJJRD4xOTA1MjAwNzE1NDY8L09yZGVySUQ+CjxVc2VySUQ+MTU0ODI2PC9Vc2VySUQ+CjxPRU0+VGhpcyBpcyBhIHJlZGlzdHJpYnV0YWJsZSBsaWNlbnNlPC9PRU0+CjxQcm9kdWN0cz4KPFByb2R1Y3Q+QXNwb3NlLlRvdGFsIGZvciAuTkVUPC9Qcm9kdWN0Pgo8L1Byb2R1Y3RzPgo8RWRpdGlvblR5cGU+RW50ZXJwcmlzZTwvRWRpdGlvblR5cGU+CjxTZXJpYWxOdW1iZXI+Y2JmMzVkNWYtOWE2Ni00ZTI4LTg1ZGQtM2ExN2JiZTM0MTNhPC9TZXJpYWxOdW1iZXI+CjxTdWJzY3JpcHRpb25FeHBpcnk+MjAyMDA2MDQ8L1N1YnNjcmlwdGlvbkV4cGlyeT4KPExpY2Vuc2VWZXJzaW9uPjMuMDwvTGljZW5zZVZlcnNpb24+CjxMaWNlbnNlSW5zdHJ1Y3Rpb25zPmh0dHBzOi8vcHVyY2hhc2UuYXNwb3NlLmNvbS9wb2xpY2llcy91c2UtbGljZW5zZTwvTGljZW5zZUluc3RydWN0aW9ucz4KPC9EYXRhPgo8U2lnbmF0dXJlPnpqZDMrdWgzNTdiZHhqR3JWTTZCN3I2c250TkRBTlRXU2MyQi9RWS9hdmZxTnA0VHk5Z0kxR2V1NUdOaWVwRHArY1JrRFBMdjBDRTZ2MHNjYVZwK1JNTkF5SzdiUzdzeGZSL205Z0NtekFNUlptdUxQTm1laEtZVTNvOGJWVDJvWmRJeEY2dVRTMDhIclJxUnk5SWt6c3BxYmRrcEZFY0lGcHlLbDF2NlF2UT08L1NpZ25hdHVyZT4KPC9MaWNlbnNlPg==";

            Stream stream4 = new MemoryStream(Convert.FromBase64String(LData));
            new Aspose.Cells.License().SetLicense(stream4);

            // URL decode the JSON data
            string decodedJsonData = HttpUtility.UrlDecode(jsonData);

            // Parse the JSON data
            dynamic parsedJsonData = Newtonsoft.Json.JsonConvert.DeserializeObject(decodedJsonData);

            // Create a new workbook
            Workbook workbook = new Workbook();

            // Access the first worksheet
            Worksheet worksheet = workbook.Worksheets[0];

            // Convert the JSON data to a two-dimensional object array
            string[,] data = GetObjectArrayFromJson(parsedJsonData);

            // Populate the worksheet with the data
            int rowCount = data.GetLength(0);
            int columnCount = data.GetLength(1);
            worksheet.Cells.ImportArray(data, rowCount, columnCount);

            // Save the workbook to a memory stream
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            workbook.Save(stream, Aspose.Cells.SaveFormat.Xlsx);

            // Set the position of the stream back to the beginning
            stream.Position = 0;

            // Create a response with the Excel file
            var fileContentResult = new FileContentResult(stream.ToArray(), "application/octet-stream")
            {
                FileDownloadName = "converted_file.xlsx"
            };

            return fileContentResult;
  
        }

        private string[,] GetObjectArrayFromJson(dynamic jsonData)
        {
            // Assuming your JSON has an array of objects with consistent structure
            // Modify this method as per your JSON structure
            var jsonArray = jsonData as Newtonsoft.Json.Linq.JArray;

            int rowCount = jsonArray.Count;
            int columnCount = jsonArray[0].Count();

            string[,] data = new string[rowCount, columnCount];

            for (int i = 0; i < rowCount; i++)
            {
                var item = jsonArray[i];
                for (int j = 0; j < columnCount; j++)
                {
                    data[i, j] = item[j].ToString();
                }
            }

            return data;
        }

        [HttpGet]
        public IActionResult PDFConvert(string fullName)
        {
            Exec("sudo chmod 775 -R /var/www/html/imspulse/bunch-box");
            string LData = "PExpY2Vuc2U+CjxEYXRhPgo8TGljZW5zZWRUbz5BdmVQb2ludDwvTGljZW5zZWRUbz4KPEVtYWlsVG8+aXRfYmlsbGluZ0BhdmVwb2ludC5jb208L0VtYWlsVG8+CjxMaWNlbnNlVHlwZT5EZXZlbG9wZXIgT0VNPC9MaWNlbnNlVHlwZT4KPExpY2Vuc2VOb3RlPkxpbWl0ZWQgdG8gMSBkZXZlbG9wZXIsIHVubGltaXRlZCBwaHlzaWNhbCBsb2NhdGlvbnM8L0xpY2Vuc2VOb3RlPgo8T3JkZXJJRD4xOTA1MjAwNzE1NDY8L09yZGVySUQ+CjxVc2VySUQ+MTU0ODI2PC9Vc2VySUQ+CjxPRU0+VGhpcyBpcyBhIHJlZGlzdHJpYnV0YWJsZSBsaWNlbnNlPC9PRU0+CjxQcm9kdWN0cz4KPFByb2R1Y3Q+QXNwb3NlLlRvdGFsIGZvciAuTkVUPC9Qcm9kdWN0Pgo8L1Byb2R1Y3RzPgo8RWRpdGlvblR5cGU+RW50ZXJwcmlzZTwvRWRpdGlvblR5cGU+CjxTZXJpYWxOdW1iZXI+Y2JmMzVkNWYtOWE2Ni00ZTI4LTg1ZGQtM2ExN2JiZTM0MTNhPC9TZXJpYWxOdW1iZXI+CjxTdWJzY3JpcHRpb25FeHBpcnk+MjAyMDA2MDQ8L1N1YnNjcmlwdGlvbkV4cGlyeT4KPExpY2Vuc2VWZXJzaW9uPjMuMDwvTGljZW5zZVZlcnNpb24+CjxMaWNlbnNlSW5zdHJ1Y3Rpb25zPmh0dHBzOi8vcHVyY2hhc2UuYXNwb3NlLmNvbS9wb2xpY2llcy91c2UtbGljZW5zZTwvTGljZW5zZUluc3RydWN0aW9ucz4KPC9EYXRhPgo8U2lnbmF0dXJlPnpqZDMrdWgzNTdiZHhqR3JWTTZCN3I2c250TkRBTlRXU2MyQi9RWS9hdmZxTnA0VHk5Z0kxR2V1NUdOaWVwRHArY1JrRFBMdjBDRTZ2MHNjYVZwK1JNTkF5SzdiUzdzeGZSL205Z0NtekFNUlptdUxQTm1laEtZVTNvOGJWVDJvWmRJeEY2dVRTMDhIclJxUnk5SWt6c3BxYmRrcEZFY0lGcHlLbDF2NlF2UT08L1NpZ25hdHVyZT4KPC9MaWNlbnNlPg==";

            Stream stream4 = new MemoryStream(Convert.FromBase64String(LData));

            new Aspose.Pdf.License().SetLicense(stream4);
            try
            {

                Aspose.Pdf.Document pdfDocument = new Aspose.Pdf.Document("/var/www/html/imspulse/bunch-box" + fullName);
                DocSaveOptions saveOptions = new DocSaveOptions
                {
                    Format = DocSaveOptions.DocFormat.Doc,
                    // Set the recognition mode as Flow
                    Mode = DocSaveOptions.RecognitionMode.Flow,
                    // Set the Horizontal proximity as 2.5
                    RelativeHorizontalProximity = 2.5f,
                    // Enable the value to recognize bullets during conversion process
                    RecognizeBullets = true,   
                };        
                pdfDocument.Save("/var/www/html/imspulse/bunch-box/tata.doc", saveOptions);
                return new JsonResult("Save");

            }
            catch (Exception ex)
            {

                return new JsonResult(ex.ToString());
            }
         
        }

        public IActionResult PDFCreate(string fullName)
        {
            Exec("sudo chmod 775 -R /var/www/html/imspulse/bunch-box");
            string LData = "PExpY2Vuc2U+CjxEYXRhPgo8TGljZW5zZWRUbz5BdmVQb2ludDwvTGljZW5zZWRUbz4KPEVtYWlsVG8+aXRfYmlsbGluZ0BhdmVwb2ludC5jb208L0VtYWlsVG8+CjxMaWNlbnNlVHlwZT5EZXZlbG9wZXIgT0VNPC9MaWNlbnNlVHlwZT4KPExpY2Vuc2VOb3RlPkxpbWl0ZWQgdG8gMSBkZXZlbG9wZXIsIHVubGltaXRlZCBwaHlzaWNhbCBsb2NhdGlvbnM8L0xpY2Vuc2VOb3RlPgo8T3JkZXJJRD4xOTA1MjAwNzE1NDY8L09yZGVySUQ+CjxVc2VySUQ+MTU0ODI2PC9Vc2VySUQ+CjxPRU0+VGhpcyBpcyBhIHJlZGlzdHJpYnV0YWJsZSBsaWNlbnNlPC9PRU0+CjxQcm9kdWN0cz4KPFByb2R1Y3Q+QXNwb3NlLlRvdGFsIGZvciAuTkVUPC9Qcm9kdWN0Pgo8L1Byb2R1Y3RzPgo8RWRpdGlvblR5cGU+RW50ZXJwcmlzZTwvRWRpdGlvblR5cGU+CjxTZXJpYWxOdW1iZXI+Y2JmMzVkNWYtOWE2Ni00ZTI4LTg1ZGQtM2ExN2JiZTM0MTNhPC9TZXJpYWxOdW1iZXI+CjxTdWJzY3JpcHRpb25FeHBpcnk+MjAyMDA2MDQ8L1N1YnNjcmlwdGlvbkV4cGlyeT4KPExpY2Vuc2VWZXJzaW9uPjMuMDwvTGljZW5zZVZlcnNpb24+CjxMaWNlbnNlSW5zdHJ1Y3Rpb25zPmh0dHBzOi8vcHVyY2hhc2UuYXNwb3NlLmNvbS9wb2xpY2llcy91c2UtbGljZW5zZTwvTGljZW5zZUluc3RydWN0aW9ucz4KPC9EYXRhPgo8U2lnbmF0dXJlPnpqZDMrdWgzNTdiZHhqR3JWTTZCN3I2c250TkRBTlRXU2MyQi9RWS9hdmZxTnA0VHk5Z0kxR2V1NUdOaWVwRHArY1JrRFBMdjBDRTZ2MHNjYVZwK1JNTkF5SzdiUzdzeGZSL205Z0NtekFNUlptdUxQTm1laEtZVTNvOGJWVDJvWmRJeEY2dVRTMDhIclJxUnk5SWt6c3BxYmRrcEZFY0lGcHlLbDF2NlF2UT08L1NpZ25hdHVyZT4KPC9MaWNlbnNlPg==";
            string basepath = "/var/www/html/imspulse/bunch-box";
            Stream stream4 = new MemoryStream(Convert.FromBase64String(LData));

            new Aspose.Pdf.License().SetLicense(stream4);
            try
            {

                Aspose.Pdf.Document pdfDocument = new Aspose.Pdf.Document();
                // Add page
                Page page = pdfDocument.Pages.Add();
                pdfDocument.Save(basepath + fullName);
                return new JsonResult("Document Created Successfully");

            }
            catch (Exception ex)
            {

                return new JsonResult(ex.ToString());
            }

        }

        public IActionResult PowerCreate(string fullName)
        {
            //Exec("sudo chmod 775 -R /var/www/html/imspulse/bunch-box");
            string LData = "PExpY2Vuc2U+CjxEYXRhPgo8TGljZW5zZWRUbz5BdmVQb2ludDwvTGljZW5zZWRUbz4KPEVtYWlsVG8+aXRfYmlsbGluZ0BhdmVwb2ludC5jb208L0VtYWlsVG8+CjxMaWNlbnNlVHlwZT5EZXZlbG9wZXIgT0VNPC9MaWNlbnNlVHlwZT4KPExpY2Vuc2VOb3RlPkxpbWl0ZWQgdG8gMSBkZXZlbG9wZXIsIHVubGltaXRlZCBwaHlzaWNhbCBsb2NhdGlvbnM8L0xpY2Vuc2VOb3RlPgo8T3JkZXJJRD4xOTA1MjAwNzE1NDY8L09yZGVySUQ+CjxVc2VySUQ+MTU0ODI2PC9Vc2VySUQ+CjxPRU0+VGhpcyBpcyBhIHJlZGlzdHJpYnV0YWJsZSBsaWNlbnNlPC9PRU0+CjxQcm9kdWN0cz4KPFByb2R1Y3Q+QXNwb3NlLlRvdGFsIGZvciAuTkVUPC9Qcm9kdWN0Pgo8L1Byb2R1Y3RzPgo8RWRpdGlvblR5cGU+RW50ZXJwcmlzZTwvRWRpdGlvblR5cGU+CjxTZXJpYWxOdW1iZXI+Y2JmMzVkNWYtOWE2Ni00ZTI4LTg1ZGQtM2ExN2JiZTM0MTNhPC9TZXJpYWxOdW1iZXI+CjxTdWJzY3JpcHRpb25FeHBpcnk+MjAyMDA2MDQ8L1N1YnNjcmlwdGlvbkV4cGlyeT4KPExpY2Vuc2VWZXJzaW9uPjMuMDwvTGljZW5zZVZlcnNpb24+CjxMaWNlbnNlSW5zdHJ1Y3Rpb25zPmh0dHBzOi8vcHVyY2hhc2UuYXNwb3NlLmNvbS9wb2xpY2llcy91c2UtbGljZW5zZTwvTGljZW5zZUluc3RydWN0aW9ucz4KPC9EYXRhPgo8U2lnbmF0dXJlPnpqZDMrdWgzNTdiZHhqR3JWTTZCN3I2c250TkRBTlRXU2MyQi9RWS9hdmZxTnA0VHk5Z0kxR2V1NUdOaWVwRHArY1JrRFBMdjBDRTZ2MHNjYVZwK1JNTkF5SzdiUzdzeGZSL205Z0NtekFNUlptdUxQTm1laEtZVTNvOGJWVDJvWmRJeEY2dVRTMDhIclJxUnk5SWt6c3BxYmRrcEZFY0lGcHlLbDF2NlF2UT08L1NpZ25hdHVyZT4KPC9MaWNlbnNlPg==";
            string basepath = "/var/www/html/imspulse/bunch-box";
            Stream stream1 = new MemoryStream(Convert.FromBase64String(LData));
            Stream stream2 = new MemoryStream(Convert.FromBase64String(LData));

            new Aspose.Words.License().SetLicense(stream1);
            new Aspose.Slides.License().SetLicense(stream2);
            try
            {
                using var presentation = new Presentation(basepath + fullName);
                var doc = new Document();
                var builder = new DocumentBuilder(doc);
                foreach (var slide in presentation.Slides)
                {
                    // generates and inserts slide image
                    using var bitmap = slide.GetThumbnail(1, 1);
                    using var stream = new MemoryStream();
                    bitmap.Save(stream, ImageFormat.Png);
                    stream.Seek(0, SeekOrigin.Begin);
                    using (SKBitmap skBitmap = SKBitmap.Decode(stream))
                    {
                        using (SKImage image = SKImage.FromBitmap(skBitmap))
                        {
                            using (SKData data = image.Encode())
                            {
                                byte[] imageByteArray = data.ToArray();

                                builder.InsertImage(imageByteArray);
                            }
                        }
                    }
                    // inserts slide's texts
                    foreach (var shape in slide.Shapes)
                    {
                        if (shape is AutoShape autoShape)
                        {
                            builder.Writeln(autoShape.TextFrame.Text);
                        }
                    }
                    builder.InsertBreak(BreakType.PageBreak);
                }
                doc.Save("/var/www/html/imspulse/bunch-box/Power.docx");
                return new JsonResult("Document Created Successfully");
            }
            catch (Exception ex)
            {
                return new JsonResult(ex.ToString());
            }
        }

        private static Stream GetContentFromUrlAsStream(string url, ICredentials credentials = null)
        {
            using (var handler = new HttpClientHandler { Credentials = credentials })
            using (var httpClient = new HttpClient(handler))
            {
                return httpClient.GetStreamAsync(url).GetAwaiter().GetResult();
            }
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
