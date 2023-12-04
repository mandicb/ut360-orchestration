using Orchestration.DataContext;
using Orchestration.Exceptions;

namespace Orchestration.Queries.Infrastructure;

public class QueryHandlerBase
{
    internal readonly OrchestrationDataContext context;

    public QueryHandlerBase(OrchestrationDataContext context)
    {
        this.context = context ?? throw new QueryException("DB context is null.");
    }
}