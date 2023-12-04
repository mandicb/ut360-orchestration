using MediatR;
using Orchestration.Models.Device.ViewModels;

namespace Orchestration.Queries.QueryHandlers.Credential;

public class GetCredentialRequest : IRequest<CreateCredentialRequestModel>
{
    public string? CredentialName { get; }
    //Decrypt or encrypted data returned
    public bool Decrypt { get; } 

    public GetCredentialRequest(string? credentialName, bool decrypt)
    {
        Decrypt = decrypt;
        CredentialName = credentialName;
    }
}

    
