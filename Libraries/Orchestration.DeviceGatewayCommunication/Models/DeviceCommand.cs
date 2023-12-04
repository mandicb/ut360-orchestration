using Administration.Infrastructure.Model;
using Orchestration.Models.Device;
using Orchestration.Models.Device.ViewModels;
using Orchestration.Models.DeviceGateway;
using Utrust360.Crypto.Model;
using Utrust360.Crypto.Services.Abstraction;
using Utrust360.DAL.Enums;
using Utrust360.ViewModels;
using DeviceGatewayViewModel = Orchestration.Models.DeviceGateway.DeviceGatewayViewModel;


namespace Orchestration.DeviceGatewayCommunication.Models;

public class DeviceCommand
{
    //TODO: change Create device command model
    public DeviceViewModel Device { get; set; }
    public DeviceGatewayViewModel DeviceGateway { get; set; }


    public static MachineViewModel PrepareDeviceModel(DeviceViewModel model, Machine.MachineStatus  status, ICryptoService cryptoService, bool encryptedData)
    {
        var m = new MachineViewModel()
        {
            Id = model.Id,
            Name = model.Name,
            Status = status,
            MachineModel = new MachineModelViewModel() { Code = MachineModelEnum.MachineModel.ANCHOR_CSAR.ToString() },
            Setting = new SettingView()
            {
                SettingProperties = new List<SettingPropertyViewModel>()
                {
                    new()
                    {
                        Code = SettingEnums.SettingProp.GENERAL_ADDRESS.ToString(),
                        Value = model.HostName
                    },
                    new()
                    {
                        Code = SettingEnums.SettingProp.GENERAL_PORT.ToString(),
                        Value = model.Port.ToString()
                    }
                }
            },
            
            Credentials = new List<CredentialView>() {
                new() 
                {
                    Setting = new SettingView()  
                    {
                        SettingProperties = new List<SettingPropertyViewModel>()
                        {
                            new()
                            {
                                Code = "CREDENTIAL_USERNAME",
                                Value = model.Credential.UserName
                            },
                            
                            new()
                            {
                                Code = "GENERAL_KEY",
                                Value = encryptedData ? cryptoService.Decrypt(model.Credential.KeyData, ProtectionKeys.KeyType.General.ToString()) : model.Credential.KeyData
                            }
                        }
                    }
                }
            }

        };

        if (!string.IsNullOrEmpty(model.Credential.KeyPassword))
        { 
            m.Credentials.First().Setting.SettingProperties.Add(new SettingPropertyViewModel()
            {
               
               Code = "GENERAL_PASSWORD",
               Value =  encryptedData ? cryptoService.Decrypt(model.Credential.KeyPassword, ProtectionKeys.KeyType.General.ToString()) : model.Credential.KeyPassword
               
           });
        }

        return m;
    }
    
}