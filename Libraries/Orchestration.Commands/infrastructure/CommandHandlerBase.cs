using Orchestration.DataContext;
using Orchestration.Exceptions;

namespace Orchestration.Commands.infrastructure;

 public class CommandHandlerBase
 {
        internal readonly OrchestrationDataContext context;

        public CommandHandlerBase(OrchestrationDataContext context)
        {
            this.context = context ?? throw new CommandException("DB context is null.");
        }
 }
