namespace connectasys_api.Core.Application.Common
{
    // Granularidade dos horários de início de Agendamento: 3 minutos.
    // Truncar garante que a checagem de conflito e a gravação sempre
    // comparam o mesmo grão de tempo, independente de segundos/ms
    // recebidos do cliente.
    public static class SlotAgendamento
    {
        public static readonly TimeSpan Duracao = TimeSpan.FromMinutes(3);

        public static DateTime Truncar(DateTime dataHora)
        {
            var ticksSlot = Duracao.Ticks;
            var ticksTruncados = dataHora.Ticks - (dataHora.Ticks % ticksSlot);
            return new DateTime(ticksTruncados, dataHora.Kind);
        }
    }
}
