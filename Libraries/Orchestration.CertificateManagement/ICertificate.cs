namespace Orchestration.CertificateManagement;

public interface ICertificate
{
    Task<Dictionary<string,object>> GenerateDeviceGatewayCertificates(string password, string hostName);
}