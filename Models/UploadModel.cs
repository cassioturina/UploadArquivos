using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace UploadArquivos.Models
{
    public class UploadModel
    {
        public IList<FileModel> Files { get; set; } = new List<FileModel>()
        {
            new FileModel() { Tipo = "RG" , Descricao = "Documento identificação", Obrigatoria = true},
            new FileModel() { Tipo = "CPF" , Descricao = "Documento CPF", Obrigatoria = true},
            new FileModel() { Tipo = "CONTRATRO" , Descricao = "Documento CONTRATRO", Obrigatoria = false},
            new FileModel() { Tipo = "IMAGEM" , Descricao = "Documento IMAGEM", Obrigatoria = false},

        };
    }

    public class FileModel
    {
        public string Tipo { get; set; }
        public string Descricao { get; set; }
        public bool Obrigatoria { get; set; } = true;

        public IFormFile File { get; set; }
    }
}
