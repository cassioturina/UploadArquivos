using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UploadArquivos.Controllers
{
    public abstract class BaseController : Controller
    {
        private readonly IWebHostEnvironment webHostEnvironment;

        private readonly string[] TIPOS_ARQUIVO_DOC_ACEITOS = new[] { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".bmp", ".png", ".xlsx", ".xls" };

        private readonly int TAMANHO_MAX_ARQUIVO = 10 * 1024 * 1024;

        protected readonly IList<string> ERROS_ARQUIVOS;

        public BaseController(IWebHostEnvironment webHostEnvironment)
        {
            this.webHostEnvironment = webHostEnvironment;
            ERROS_ARQUIVOS = new List<string>();
        }

        protected void GravaArquivoNaPasta(IFormFile file, string path)
        {
            using var stream = new FileStream(path, FileMode.Create);
            file.CopyTo(stream);
        }

        protected string PathTo(string pasta, string nome, string folder = "ARQUIVOS")
        {
            var path = Path.Combine($"{folder}/{pasta}/{nome}");
            return path;
        }

        protected void DeletarArquivo(string pasta, string nome, string folder = "ARQUIVOS")
        => System.IO.File.Delete(PathTo(pasta, nome, folder));
        protected void DeletarArquivo(string path)
        {
            var t = webHostEnvironment.WebRootPath;
            System.IO.File.Delete($"{t}/{path}");
        }
        protected bool ArquivoEhValido(IFormFile file)
        {
            if (file == null)
            {
                ERROS_ARQUIVOS.Add("Arquivo Nulo Ou Vazio");
                return false;
            }

            ValidarArquivo(file);
            ValidarTamanhoArquivo(file);
            ValidarTipoDocumento(file);
            ValidarTamanhoNome(file);

            return ERROS_ARQUIVOS.Count == 0;
        }
        protected void ValidarTamanhoNome(IFormFile file)
        {
            if (file.FileName.Length > 120)
            {
                ERROS_ARQUIVOS.Add($"O nome do arquivo {file.FileName} deve ter no máximo 120 Caracteres");
            }
        }

        protected string RenomearArquivo(IFormFile file)
        => $"{Guid.NewGuid():D}{Path.GetExtension(file.FileName)}";
        public string CriarDiretorioPara(string tipo)
        {
            var caminhoParaUpload = Path.Combine(webHostEnvironment.WebRootPath, $"{tipo}");

            if (!Directory.Exists(caminhoParaUpload))
            {
                Directory.CreateDirectory(caminhoParaUpload);
            }

            return caminhoParaUpload;
        }

        private void ValidarArquivo(IFormFile file)
        {
            if (file?.Length <= 0) ERROS_ARQUIVOS.Add("Arquivo Inválido");
        }

        private void ValidarTamanhoArquivo(IFormFile file)
        {
            if (file.Length > TAMANHO_MAX_ARQUIVO) ERROS_ARQUIVOS.Add($"{file.FileName} excede o tamanho máximo de {TAMANHO_MAX_ARQUIVO / 1000000}MB");
        }

        private void ValidarTipoDocumento(IFormFile file)
        {
            if (!TIPOS_ARQUIVO_DOC_ACEITOS.Any(s => s == Path.GetExtension(file.FileName.ToLower())))
                ERROS_ARQUIVOS.Add($"{file.FileName} Tipo de Documento não aceito");

        }

    }
}
