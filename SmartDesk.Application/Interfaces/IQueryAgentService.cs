public interface IQueryAgentService
{
    Task<QueryResponseDto> ProcessQueryAsync(QueryRequestDto request);
}
