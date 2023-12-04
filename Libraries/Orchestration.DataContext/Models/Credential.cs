using Microsoft.EntityFrameworkCore;
using Orchestration.Exceptions;
using Orchestration.Models.Credential;
using Utrust360.Crypto.Model;
using Utrust360.Crypto.Services.Abstraction;
using Utrust360.ViewModels;
using Utrust360.ViewModels.Helpers;

namespace Orchestration.DataContext.Models;

public class Credential : EntityBase
{
    //TODO: validation
    public Credential(){}

    private Credential(CreatCredentialCommandModel rm)
    {
        
        Name = rm.Name;
        UserName = rm.UserName;
        KeyPassword = rm.KeyPassword;
        KeyData = rm.KeyData;
    }
    
    public string Name { get; set; } = null!;
    public string UserName { get; set; }
    public string? KeyPassword { get; set; }
    public string KeyData { get; set; } = null!;

    private static void Validate(CreatCredentialCommandModel requestCredential)
    {
        
        var r = CredentialValidationHelper.ValidateKey(requestCredential.KeyData, requestCredential.KeyPassword);
        switch (r)
        {
            case CredentialView.KeyStatus.Invalid: throw new ModelException("Provided key data is invalid");
            case CredentialView.KeyStatus.NoPassword: throw new ModelException("Key Password is missing");

        }
       
    }
    
    public static Credential Create(CreatCredentialCommandModel requestCredential, ICryptoService cryptoService)
    {
        Validate(requestCredential);
        requestCredential.KeyData =
            cryptoService.Encrypt(requestCredential.KeyData, ProtectionKeys.KeyType.General.ToString());

        if (requestCredential.KeyPassword != null)
        {
            requestCredential.KeyPassword =
                cryptoService.Encrypt(requestCredential.KeyPassword, ProtectionKeys.KeyType.General.ToString());
        }
        
        return new Credential(requestCredential);
    }
    
}