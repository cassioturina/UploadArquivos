namespace UploadArquivos.Models
{
    public class Arquivo
    {
        public string Caminho { get; }
        public string Nome { get; }
        public string NomeOriginal { get; }

        public Arquivo(string caminho, string nome, string nomeOriginal)
        {
            Caminho = caminho;
            Nome = nome;
            NomeOriginal = nomeOriginal;
        }

    }
}
