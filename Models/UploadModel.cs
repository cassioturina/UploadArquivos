using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UploadArquivos.Models
{
    public class UploadModel
    {
        public IList<ArquivoUploads> Arquivos { get; set; }
    }

    public class ArquivoUploads
    {
        [Required(ErrorMessage = "Documento obrigatório")]
        public IFormFile File { get; set; }
        public string Descricao { get; set; }
        public bool Requerido { get; set; }
    }
}
