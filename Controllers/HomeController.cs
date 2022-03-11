using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
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
            return View(new UploadModel
            {
                Arquivos = new List<ArquivoUploads>()
                {
                    new ArquivoUploads { Descricao = "RG", Requerido = true},
                    new ArquivoUploads { Descricao = "CPF", Requerido = true},
                    new ArquivoUploads { Descricao = "CNH", Requerido = false},
                    new ArquivoUploads { Descricao = "Comprovante de endereço", Requerido = false},
                }
            });
        }

        [HttpPost()]
        public IActionResult Index(UploadModel model)
        {
            

            var naoRequeridos = model.Arquivos.Where(x => !x.Requerido);

            for (int i = 0; i < model.Arquivos.Count; i++)
            {
                if (!model.Arquivos[i].Requerido)
                {
                    var key = $"Arquivos[{i}].File";
                    ModelState.Remove(key);
                }
            }

            var valid = ModelState.IsValid;


            //foreach (var item in model.Arquivos.Where(x=>!x.Requerido))
            //{
            //    ModelState.Remove(item.File);
            //}


            ArquivosUpload = new List<Arquivo>();
            foreach (var arquivo in model?.Arquivos)
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

                ArquivosUpload.Add(new Arquivo($"{path}", nomeNoDiretorio, arquivo.File.FileName));
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
