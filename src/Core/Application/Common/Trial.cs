namespace connectasys_api.Core.Application.Common
{
    // "3 dias de teste" contados por dia calendário, incluindo o próprio dia
    // do cadastro como dia 1 — cadastrou dia 13: dia 13 (1) e dia 14 (2) têm
    // acesso garantido; a partir da meia-noite UTC do dia 15 (3) já bloqueia.
    // Não é "72 horas exatas a partir do horário do cadastro".
    public static class Trial
    {
        private const int DiasAdicionaisAposCadastro = 2;

        public static DateTime CalcularExpiracao(DateTime dataCadastroUtc) =>
            dataCadastroUtc.Date.AddDays(DiasAdicionaisAposCadastro);
    }
}
