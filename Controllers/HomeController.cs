using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using UploadArquivos.Models;

namespace UploadArquivos.Controllers
{
    public class HomeController : BaseController
    {

        private static IList<Arquivo> ArquivosUpload;

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment webHostEnvironment) : base(webHostEnvironment)
        {
            _logger = logger;
        }

        [HttpGet()]
        public IActionResult Index()
        {
            return View(new UploadModel());
        }

        [HttpPost()]
        public IActionResult Index(UploadModel model)
        {
            ArquivosUpload = new List<Arquivo>();
            foreach (var arquivo in model?.Files)
            {
                if (!ArquivoEhValido(arquivo.File))
                {
                    //
                    continue;
                }

                var nomeNoDiretorio = RenomearArquivo(arquivo.File);
                var path = PathTo("upload", nomeNoDiretorio);
                var caminhoNoSevidor = CriarDiretorioPara("arquivos/upload");
                GravaArquivoNaPasta(arquivo.File, $"{caminhoNoSevidor}/{nomeNoDiretorio}");

                ArquivosUpload.Add(new Arquivo( $"{path}", nomeNoDiretorio, arquivo.File.FileName ));
            }


            return RedirectToAction(nameof(Arquivos));
        }

        [HttpGet("arquivos")]
        public IActionResult Arquivos()
        {
            return View(ArquivosUpload);
        }
    }
}
