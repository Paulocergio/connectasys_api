namespace connectasys_api.Core.Application.Exceptions
{
    public class DocumentoDuplicadoException : Exception
    {
        public DocumentoDuplicadoException()
            : base("Já existe um cliente cadastrado com este CPF ou CNPJ.")
        {
        }
    }
}
