using DinkToPdf;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace UploadArquivos.Services
{
    public interface IPdfServices
    {
        Task<byte[]> MergePdfs(IEnumerable<string> paths);
        Task<byte[]> HtmlToPdf(string html);
        Task<byte[]> AddImageToPdf(string pdfPath, string imagePath);
        Task<byte[]> AddImageToPdf(string pdfPath, IEnumerable<string> imagesPaths);
    }


    public class PdfServices : IPdfServices
    {
        private readonly string tempDir = PathHelper.GetInstance().RootDir;
        public async Task<byte[]> AddImageToPdf(string pdfPath, string imagePath)
        {
            using var fs = System.IO.File.OpenRead(pdfPath);
            var inputDocument = PdfReader.Open(fs, PdfDocumentOpenMode.Modify);


            var page = inputDocument.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            XImage image = XImage.FromFile(imagePath);
            gfx.DrawImage(image, 50, 50);


            var outFilePath = Path.Combine(tempDir, Path.GetFileName(pdfPath));
            inputDocument.Save(outFilePath);
            return await File.ReadAllBytesAsync(outFilePath);
        }

        public async Task<byte[]> AddImageToPdf(string pdfPath, IEnumerable<string> imagesPaths)
        {
            using var fs = File.OpenRead(pdfPath);
            var inputDocument = PdfReader.Open(fs, PdfDocumentOpenMode.Modify);

            foreach (var img in imagesPaths)
            {
                var page = inputDocument.AddPage();
                var gfx = XGraphics.FromPdfPage(page);
                XImage image = XImage.FromFile(img);
                gfx.DrawImage(image, 50, 50);
            }

            var outFilePath = Path.Combine(tempDir, Path.GetFileName(pdfPath));
            inputDocument.Save(outFilePath);
            return await File.ReadAllBytesAsync(outFilePath);
        }

        public async Task<byte[]> HtmlToPdf(string html)
        {

            var converter = new BasicConverter(new PdfTools());

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings =
                        {
                            ColorMode = ColorMode.Color,
                            Orientation = Orientation.Portrait,
                            PaperSize = PaperKind.A4Plus,
                        }
            };

            doc.Objects.Add(new ObjectSettings()
            {
                PagesCount = false,
                HtmlContent = html,
                WebSettings = { DefaultEncoding = "utf-8" },
            });

            return await Task.FromResult(converter.Convert(doc));
        }

        public async Task<byte[]> MergePdfs(IEnumerable<string> paths)
        {
            var outputDocument = new PdfDocument();


            foreach (var pdfPath in paths)
            {
                using var fs = File.OpenRead(pdfPath);
                var inputDocument = PdfSharpCore.Pdf.IO.PdfReader.Open(fs, PdfDocumentOpenMode.Import);
                var count = inputDocument.PageCount;
                for (var idx = 0; idx < count; idx++)
                {
                    var pages = inputDocument.Pages[idx];
                    outputDocument.AddPage(pages);
                }
            }

            var outFilePath = Path.Combine(tempDir, $"{DateTime.Now.ToShortTimeString()}.pdf");
            outputDocument.Save(outFilePath);
            return await File.ReadAllBytesAsync(outFilePath);

        }


    }

    public class PathHelper
    {
        public PathHelper() => RootDir = Path.GetDirectoryName(GetType().GetTypeInfo().Assembly.Location);

        public string GetAssetPath(string name) => Path.Combine(RootDir, "temp", name);

        public string RootDir { get; }

        public static PathHelper GetInstance() => _instance ??= new PathHelper();

        private static PathHelper _instance;
    }
}
