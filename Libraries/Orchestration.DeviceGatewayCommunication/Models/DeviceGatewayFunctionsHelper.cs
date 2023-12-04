namespace Orchestration.DeviceGatewayCommunication.Models;

public static class DeviceGatewayFunctionsHelper
{
    public enum Function
    {
        SetCert,
        ResetCert,
        TestConnection,
        Status,
        AddDeleteDevice,
        GetDevices
    }
    
    public enum OrchestrationFunction
    {
        SlotInfo,
        Create,
        Delete,
        Snapshot

    }
    
    public static string Commands(Function function)
    {
        var commands = new Dictionary<Function, string>
        { 
            { Function.SetCert, "set-cert" },
            { Function.ResetCert, "reset-cert" },
            { Function.TestConnection, "test-conn" },
            { Function.Status, "status" },
            { Function.AddDeleteDevice, "load-devices" },
            { Function.GetDevices, "db-info" },
        };

     
        return commands[function];
    }

    public static string OrchestrationCommands(OrchestrationFunction function)
    {
        var commands = new Dictionary<OrchestrationFunction, string>
        { 
            { OrchestrationFunction.SlotInfo, "system-info" },
            { OrchestrationFunction.Create, "chsm-create" },
            { OrchestrationFunction.Delete, "chsm-delete" },
            { OrchestrationFunction.Snapshot, "chsm-snapshot" },
        };

     
        return commands[function];
    }


}