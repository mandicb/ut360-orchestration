using System.Security.Cryptography.X509Certificates;
using Administration.Infrastructure.Model;
using Microsoft.Extensions.Logging;
using Orchestration.DeviceGatewayCommunication.Models;
using Orchestration.Exceptions;
using Orchestration.Models.DeviceGateway;
using Utrust360.CommunicationManager.Responses;
using Utrust360.CommunicationManager.Services;
using Utrust360.Crypto.Services.Abstraction;
using Utrust360.DAL.Enums;

namespace Orchestration.DeviceGatewayCommunication;

//TODO: Check loging!!!
public class DeviceGatewayManagement : IDeviceGatewayManagement
{
    private readonly ICryptoService _cryptoService;
    private readonly ILogger<DeviceGatewayManagement> _deviceGatewayLogger;
    private readonly ILogger<TcpClientService> _tcpClientLogger;
    private const string Seed = "JIE)sUhjdjn09";


    public DeviceGatewayManagement(ICryptoService cryptoService, ILogger<DeviceGatewayManagement> deviceGatewayLogger, ILogger<TcpClientService> tcpClientLogger )
    {
        _cryptoService = cryptoService;
        _tcpClientLogger = tcpClientLogger;
        _deviceGatewayLogger = deviceGatewayLogger;
    }
    
    
    public async Task DeviceGatewayLink(CreateDeviceGatewayCommandModel model, Dictionary<string, object> parameters, int? connectionTimeout = null, int? receiveTimeout = 30000, int checkInterval = 60)
    {
        
        try
        {
            
            //TODO: add retry mechanism !!!

            var deviceGatewayTcpClientTest = new TcpClientService(new TcpClientServiceModel
            {
                HostName = model.HostName,
                Port = model.Port,
                DeviceGatewayId = model.Id,
                CryptoService = _cryptoService,
                Logger = _tcpClientLogger,
                ConnectionTimeout = connectionTimeout
            });
            

           var response = await deviceGatewayTcpClientTest.Send(new TcpClientServiceSendCommand
            {
                Function = DeviceGatewayFunctionsHelper.Commands(DeviceGatewayFunctionsHelper.Function.SetCert),
                ReceiveTimeout = receiveTimeout,
                Params = parameters,
                CheckInterval = checkInterval
            });

               if (!response.Valid)
               {
                   _deviceGatewayLogger.LogError($"SendCommand SET-CERT failed:  : {response.Message}");
                   throw new CustomException("Linking Device gateway to Manager failed");
               }
        }
        catch (Exception e)
        {
            _deviceGatewayLogger.LogError($"SendCommand: {e.Message} : {e.StackTrace}");
            throw new CustomException("Can not connect to device gateway");
        }


    }

    
    public async Task DeviceGatewayUnlink(DeviceGatewayViewModel model, int? receiveTimeout, int checkInterval = 60,  int? connectionTimeout = null)
    {
        
        var deviceGatewayTcpClientTest = new TcpClientService(new TcpClientServiceModel
        {
            HostName = model.HostName,
            Port = model.Port,
            DeviceGatewayId = model.Id,
            CryptoService = _cryptoService,
            CertificateSerialNumber =  new X509Certificate2(model.Certificate, model.Password + Seed).SerialNumber,
            CertificatePassword = model.Password,
            Logger = _tcpClientLogger,
            ConnectionTimeout = connectionTimeout
        });

        try
        {
            var response = await deviceGatewayTcpClientTest.Send(new TcpClientServiceSendCommand
            {
                Function = DeviceGatewayFunctionsHelper.Commands(DeviceGatewayFunctionsHelper.Function.ResetCert),
                ReceiveTimeout = receiveTimeout,
                CheckInterval = checkInterval
            });
            
            if (!response.Valid)
            {
                _deviceGatewayLogger.LogWarning($"SendCommand RESET-CERT failed: : {response.Message}");
                throw new CustomException("Unlinking Device gateway to Manager failed");
            }
        }
        catch (Exception e)
        {
            _deviceGatewayLogger.LogWarning($"SendCommand: {e.Message} : {e.StackTrace}");
            throw new CustomException("Cen not remove device gateway");
        }
        
    }

    
    public async Task<BaseDeviceGatewayResponse> DeviceGatewayTestConnection(CreateDeviceGatewayCommandModel model, int? receiveTimeout, int checkInterval = 60,
        int? connectionTimeout = null)
    {

        try
        {
            var deviceGatewayTcpClientTest = new TcpClientService(new TcpClientServiceModel
            {
                HostName = model.HostName,
                Port = model.Port,
                DeviceGatewayId = model.Id, //TODO: check if its unique!!
                CryptoService = _cryptoService,
                Logger = _tcpClientLogger,
                ConnectionTimeout = connectionTimeout
            });
            
            var response = await deviceGatewayTcpClientTest.Send(new TcpClientServiceSendCommand
            {
                Function = DeviceGatewayFunctionsHelper.Commands(DeviceGatewayFunctionsHelper.Function.Status),
                ReceiveTimeout = receiveTimeout,
                CheckInterval = checkInterval
            });


            if (!response.Valid)
            {
                _deviceGatewayLogger.LogWarning($"SendCommand Status failed: : {response.Message}");
                throw new CustomException("Test connection to Device gateway");
            }

            var responseObject = new BaseDeviceGatewayResponse()
            {
                Version = response.Results.FirstOrDefault(r => r.Name == "Version")?.Value,
                Status = response.Results.FirstOrDefault(r => r.Name == "Status")?.Value,
                BuildDate = response.Results.FirstOrDefault(r => r.Name == "BuildDate")?.Value
            };
            
            return responseObject;
        }
        catch (Exception e)
        {
            _deviceGatewayLogger.LogWarning($"SendCommand: {e.Message} : {e.StackTrace}");
            throw new CustomException($"Can not connect to device gateway");
        }
        
    }

    
    public async Task<BaseDeviceResponse> DeviceTestConnection(DeviceCommand command, Dictionary<string, object>? parameters = null, int? receiveTimeout = null, int checkInterval = 60, int? connectionTimeout = null)
    {
        
        //Test connection device
        var defaultTestConnectionParams = new Dictionary<string, object>()
        {
            { "Content",  TestConnectionCommand.Content.info_group.ToString() },
        };

        
        var deviceGatewayTcpClientTest = new TcpClientService(new TcpClientServiceModel
        {
            HostName = command.DeviceGateway.HostName,
            Port = command.DeviceGateway.Port,
            DeviceGatewayId = command.DeviceGateway.Id,
            CryptoService = _cryptoService,
            CertificateSerialNumber =  new X509Certificate2(command.DeviceGateway.Certificate, command.DeviceGateway.Password + Seed).SerialNumber,
            CertificatePassword = command.DeviceGateway.Password,
            Logger = _tcpClientLogger,
            ConnectionTimeout = connectionTimeout
        });
        
        try
        {
            var response = await deviceGatewayTcpClientTest.Send(new TcpClientServiceSendCommand
            {
                Function = DeviceGatewayFunctionsHelper.Commands(DeviceGatewayFunctionsHelper.Function.TestConnection),
                Params = parameters ?? defaultTestConnectionParams,
                ReceiveTimeout = receiveTimeout,
                CheckInterval = checkInterval,
                Machines = new []{DeviceCommand.PrepareDeviceModel(command.Device, Machine.MachineStatus.Online, _cryptoService, false)} 
            });

            if (response.Valid)
            {
                
                var responseObject = new BaseDeviceResponse()
                {
                    Version = response.Results.FirstOrDefault(r => r.Name == "VersionId")?.Value,
                    SerialNumber = response.Results.FirstOrDefault(r => r.Name == "SerialNumber")?.Value,
                    HardwareVersion = response.Results.FirstOrDefault(r => r.Name == "HardwareVersion")?.Value
                };
                return responseObject;
            }
            _deviceGatewayLogger.LogError($"SendCommand TEST-CONN failed: : {response.Message}");
            throw new CustomException("test connection to device failed");

        }
        catch (Exception e)
        {
            _deviceGatewayLogger.LogError($"SendCommand TEST-CONN failed: : {e.Message}");
            throw new CustomException("Device test connection failed");
        }
        
    }

    
    public async Task<BaseTcpResponse> GetDevices(DeviceGatewayViewModel command, int? receiveTimeout = null, int checkInterval = 60,
        int? connectionTimeout = null)
    {
        var deviceGatewayTcpClientTest = new TcpClientService(new TcpClientServiceModel
        {
            HostName = command.HostName,
            Port = command.Port,
            DeviceGatewayId = command.Id,
            CryptoService = _cryptoService,
            CertificateSerialNumber =  new X509Certificate2(command.Certificate, command.Password + Seed).SerialNumber,
            CertificatePassword = command.Password,
            Logger = _tcpClientLogger,
            ConnectionTimeout = connectionTimeout
        });

        try
        {
            var response = await deviceGatewayTcpClientTest.Send(new TcpClientServiceSendCommand
            {
                Function = DeviceGatewayFunctionsHelper.Commands(DeviceGatewayFunctionsHelper.Function.GetDevices),
                ReceiveTimeout = receiveTimeout,
                CheckInterval = checkInterval,
            });
            
            if (!response.Valid)
            {
                _deviceGatewayLogger.LogError($"SendCommand load-devices failed: : {response.Message}");
                throw new CustomException("Can not get list of devices from device gateway");
            }
            return response;

            
        }
        catch (Exception e)
        {
            _deviceGatewayLogger.LogError($"SendCommand db-info  failed: : {e.Message}");
            throw new CustomException(e.Message);

        }
    }

    
    public async Task<BaseTcpResponse> AddDeleteDevice(DeviceCommand command, Machine.MachineStatus status, int? receiveTimeout = null, int checkInterval = 60,
        int? connectionTimeout = null)
    {
        var deviceGatewayTcpClientTest = new TcpClientService(new TcpClientServiceModel
        {
            HostName = command.DeviceGateway.HostName,
            Port = command.DeviceGateway.Port,
            DeviceGatewayId = command.DeviceGateway.Id,
            CryptoService = _cryptoService,
            CertificateSerialNumber =  new X509Certificate2(command.DeviceGateway.Certificate, command.DeviceGateway.Password + Seed).SerialNumber,
            CertificatePassword = command.DeviceGateway.Password,
            Logger = _tcpClientLogger,
            ConnectionTimeout = connectionTimeout
        });

        try
        {
            //Add device to dg
            var response = await deviceGatewayTcpClientTest.Send(new TcpClientServiceSendCommand
            {
                Function = DeviceGatewayFunctionsHelper.Commands(DeviceGatewayFunctionsHelper.Function.AddDeleteDevice),
                ReceiveTimeout = receiveTimeout,
                CheckInterval = checkInterval,
                Machines = new[] { DeviceCommand.PrepareDeviceModel(command.Device,  status, _cryptoService, false) }
            });

            if (response.Valid)
            {
                return response;
            }
            _deviceGatewayLogger.LogError($"SendCommand load-devices failed: : {response.Message}");
            throw new CustomException("Can not add device to device gateway");
        }
        catch (Exception e)
        {   
            _deviceGatewayLogger.LogError($"SendCommand load-devices failed: : {e.Message}");
            throw new CustomException(e.Message);
        }
    }
}