using System.Security.Cryptography.X509Certificates;
using Administration.Infrastructure.Model.Message;
using Microsoft.Extensions.Logging;
using Orchestration.DeviceGatewayCommunication.Models;
using Orchestration.Exceptions;
using Utrust360.CommunicationManager.Responses;
using Utrust360.CommunicationManager.Services;
using Utrust360.Crypto.Services.Abstraction;

namespace Orchestration.DeviceGatewayCommunication;

public class DeviceOrchestration : IDeviceOrchestration
{
    
    private readonly ICryptoService _cryptoService;
    private readonly ILogger<DeviceGatewayManagement> _deviceGatewayLogger;
    private readonly ILogger<TcpClientService> _tcpClientLogger;
    private const string Seed = "JIE)sUhjdjn09";


    public DeviceOrchestration(ICryptoService cryptoService, ILogger<DeviceGatewayManagement> deviceGatewayLogger, ILogger<TcpClientService> tcpClientLogger )
    {
        _cryptoService = cryptoService;
        _tcpClientLogger = tcpClientLogger;
        _deviceGatewayLogger = deviceGatewayLogger;
    }
    
    public async Task<Items> GetSlots(DeviceCommand command, Dictionary<string, object> parameters, int? receiveTimeout = null, int checkInterval = 60,
        int? connectionTimeout = null)
    {
        
        //TODO: code duplication??
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
        
        var response = await deviceGatewayTcpClientTest.Send(new TcpClientServiceSendCommand
        {
            Function = DeviceGatewayFunctionsHelper.OrchestrationCommands(DeviceGatewayFunctionsHelper.OrchestrationFunction.SlotInfo),
            ReceiveTimeout = receiveTimeout,
            Params = parameters,
            CheckInterval = checkInterval
        });

        if (!response.Valid)
        {
            _deviceGatewayLogger.LogError($"SendCommand system-info failed: : {response.Message}");
            throw new CustomException("Can not get list of slots");   
        }

        var slotInfo = response.Results.FirstOrDefault(x => x.Name == "SlotList");
        if (slotInfo != null)
        {
            return slotInfo;
        }
        _deviceGatewayLogger.LogError($"SendCommand system-info failed: : {response.Message}");
        throw new CustomException("List of slots does not exists");

    }

    public async Task<Items> GetTemplates(DeviceCommand command, Dictionary<string, object> parameters, int? receiveTimeout = null, int checkInterval = 60,
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
        
        var response = await deviceGatewayTcpClientTest.Send(new TcpClientServiceSendCommand
        {
            Function = DeviceGatewayFunctionsHelper.OrchestrationCommands(DeviceGatewayFunctionsHelper.OrchestrationFunction.SlotInfo),
            ReceiveTimeout = receiveTimeout,
            Params = parameters,
            CheckInterval = checkInterval
        });

        if (!response.Valid)
        {
            _deviceGatewayLogger.LogError($"SendCommand system-info failed: template list : {response.Message}");
            throw new CustomException("Can not get list of templates");   
        }

        var templateList = response.Results.FirstOrDefault(x => x.Name == "TemplateList");
        if (templateList != null)
        {
            return templateList;
        }
        _deviceGatewayLogger.LogError($"SendCommand system-info failed: template list : {response.Message}");
        throw new CustomException("List of templates does not exists");
    }

    public async Task<BaseTcpResponse> CreateChsm(DeviceCommand command, Dictionary<string, object> param, int? receiveTimeout = null, int checkInterval = 60,
        int? connectionTimeout = null)
    {
        //TODO: code duplication??
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
        
        //TODO check if slot is free!!!!
        
        var response = await deviceGatewayTcpClientTest.Send(new TcpClientServiceSendCommand
        {
            Function = DeviceGatewayFunctionsHelper.OrchestrationCommands(DeviceGatewayFunctionsHelper.OrchestrationFunction.Create),
            ReceiveTimeout = receiveTimeout,
            Params = param,
            CheckInterval = checkInterval
        });

        if (!response.Valid)
        {
            _deviceGatewayLogger.LogError($"SendCommand chsm-create failed: {response.Valid} : {response.Message}");
            throw new CustomException("Can not create new cshm");   
        }
        
        //TODO: store data to failes importent!!!!

        return response;
    }

    public async Task<BaseTcpResponse> DeleteChsm(DeviceCommand command, Dictionary<string, object> param,
        int? receiveTimeout = null, int checkInterval = 60,
        int? connectionTimeout = null)
    {
        //TODO: code duplication??
        var deviceGatewayTcpClientTest = new TcpClientService(new TcpClientServiceModel
        {
            HostName = command.DeviceGateway.HostName,
            Port = command.DeviceGateway.Port,
            DeviceGatewayId = command.DeviceGateway.Id,
            CryptoService = _cryptoService,
            CertificateSerialNumber =
                new X509Certificate2(command.DeviceGateway.Certificate, command.DeviceGateway.Password + Seed)
                    .SerialNumber,
            CertificatePassword = command.DeviceGateway.Password,
            Logger = _tcpClientLogger,
            ConnectionTimeout = connectionTimeout
        });
        
        var response = await deviceGatewayTcpClientTest.Send(new TcpClientServiceSendCommand
        {
            Function = DeviceGatewayFunctionsHelper.OrchestrationCommands(DeviceGatewayFunctionsHelper
                .OrchestrationFunction.Delete),
            ReceiveTimeout = receiveTimeout,
            Params = param,
            CheckInterval = checkInterval
        });

        if (!response.Valid)
        {
            _deviceGatewayLogger.LogError($"SendCommand chsm-delete failed: {response.Valid} : {response.Message}");
            throw new CustomException("Can not delete chsm");
        }

        return response;
    }

    public async Task<BaseTcpResponse> SnapshotChsm(DeviceCommand command, Dictionary<string, object> param, int? receiveTimeout = null, int checkInterval = 60,
        int? connectionTimeout = null)
    {
        var deviceGatewayTcpClientTest = new TcpClientService(new TcpClientServiceModel
        {
            HostName = command.DeviceGateway.HostName,
            Port = command.DeviceGateway.Port,
            DeviceGatewayId = command.DeviceGateway.Id,
            CryptoService = _cryptoService,
            CertificateSerialNumber =
                new X509Certificate2(command.DeviceGateway.Certificate, command.DeviceGateway.Password + Seed)
                    .SerialNumber,
            CertificatePassword = command.DeviceGateway.Password,
            Logger = _tcpClientLogger,
            ConnectionTimeout = connectionTimeout
        });
        
        var response = await deviceGatewayTcpClientTest.Send(new TcpClientServiceSendCommand
        {
            Function = DeviceGatewayFunctionsHelper.OrchestrationCommands(DeviceGatewayFunctionsHelper.OrchestrationFunction.Snapshot),
            ReceiveTimeout = receiveTimeout,
            Params = param,
            CheckInterval = checkInterval
        });
        
        if (!response.Valid)
        {
            _deviceGatewayLogger.LogError($"SendCommand chsm-snapshot failed: {response.Valid} : {response.Message}");
                throw new CustomException($"Can not create snapshot : There is no active Operator Secret present on the device." );
        }

        return response;
    }
}