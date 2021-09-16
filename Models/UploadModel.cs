using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace UploadArquivos.Models
{
    public class UploadModel
    {
        public IList<IFormFile> Files { get; set; }
    }
}
