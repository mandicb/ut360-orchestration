using MediatR;
using Microsoft.EntityFrameworkCore;
using Orchestration.DataContext;
using Orchestration.Exceptions;
using Orchestration.Models.Credential.ViewModel;
using Orchestration.Models.Device.ViewModels;
using Orchestration.Queries.Infrastructure;
using Utrust360.Crypto.Model;
using Utrust360.Crypto.Services.Abstraction;

namespace Orchestration.Queries.QueryHandlers.Credential;

public class GetCredentialQueryHandler :  QueryHandlerBase, IRequestHandler<GetCredentialRequest, CreateCredentialRequestModel>
{
    private readonly ICryptoService _cryptoService;

    public GetCredentialQueryHandler(OrchestrationDataContext context, ICryptoService cryptoService) : base(context)
    {
        _cryptoService = cryptoService;
    }

    public async Task<CreateCredentialRequestModel> Handle(GetCredentialRequest request, CancellationToken cancellationToken)
    {
        var credential = await context.Credentials.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Name == request.CredentialName, cancellationToken);

        if (credential == null)
        {
            throw new QueryException($"Credential {request.CredentialName} does not exists");
        }

        var response = new CreateCredentialRequestModel()
        {
            Id = credential.Id,
            Name = credential.Name,
            KeyData = request.Decrypt == false ? credential.KeyData : _cryptoService.Decrypt(credential.KeyData, ProtectionKeys.KeyType.General.ToString()),
            KeyPassword = !string.IsNullOrEmpty(credential.KeyPassword) && request.Decrypt ? _cryptoService.Decrypt(credential.KeyPassword, ProtectionKeys.KeyType.General.ToString()) : credential.KeyPassword,
            UserName = credential.UserName
        };

        return response;
    }
}