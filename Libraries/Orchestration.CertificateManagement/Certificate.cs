using System.Collections;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Orchestration.Exceptions;

namespace Orchestration.CertificateManagement;

public class Certificate : ICertificate
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<Certificate> _logger;
    private const string Seed = "JIE)sUhjdjn09";
    private const string PathAppCert = @"/app/cert/app-cert";

    public Certificate(IConfiguration configuration, ILogger<Certificate> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }
    
    
    
    /// <summary>
    ///     Exports to pem.
    /// </summary>
    /// <param name="cert">The cert.</param>
    /// <returns>ResponseServices&lt;System.String&gt;.</returns>
    private static string ExportToPem(X509Certificate cert)
    {
        var builder = new StringBuilder();

        builder.AppendLine("-----BEGIN CERTIFICATE-----");
        builder.AppendLine(Convert.ToBase64String(cert.Export(X509ContentType.Cert),
            Base64FormattingOptions.InsertLineBreaks));
        builder.AppendLine("-----END CERTIFICATE-----");

        return builder.ToString();
    }
    
    public async Task<Dictionary<string, object>> GenerateDeviceGatewayCertificates(string password, string hostName)
    {
        var caCert = _configuration["DefaultCAPath"];
        var passwordCa = _configuration["ASPNETCORE_Kestrel:Certificates:Default:Password"];
        var collection = new X509Certificate2Collection();
        try
        {
            if (caCert != null)
            {
                collection.Import(caCert, passwordCa, X509KeyStorageFlags.PersistKeySet);
            }
        }
        catch (Exception e)
        {
            _logger.LogError($"{this.GetType()}: {e.Message} : {e.StackTrace}");
            throw new CustomException($"CERTIFICATE_ERROR: {e.Message} : {e.StackTrace}");
        }

        // SAN
        var sanBuilder = new SubjectAlternativeNameBuilder();
        var ipsEnvVariables = new List<string> { "IP", "IP1", "IP2", "IP3", "IP4" };
        
        foreach (DictionaryEntry  entry in Environment.GetEnvironmentVariables() )
        {
            if (ipsEnvVariables.Any(x => x == (string)entry.Key) && (string)entry.Value! != hostName)
            {
                var validIp = IPAddress.Parse((string)entry.Value!);
                sanBuilder.AddIpAddress(validIp);
            }
        }
            
        sanBuilder.AddDnsName(hostName);

        var dnBuilder = new X500DistinguishedNameBuilder();
        dnBuilder.AddCommonName("deviceGateway.utrust360.local");
        dnBuilder.AddOrganizationName("Utimaco");
        dnBuilder.AddOrganizationalUnitName("utrust360");
        dnBuilder.AddLocalityName("Campbell");
        dnBuilder.AddStateOrProvinceName("CA");
        dnBuilder.AddCountryOrRegion("US");
        dnBuilder.AddEmailAddress("hsm@utimaco.com");
            
            
        var uniqueId = Guid.NewGuid();
        var distinguishedName = dnBuilder.Build();

        var rsa = RSA.Create(2048);
        var request = new CertificateRequest(distinguishedName, rsa, HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);
        request.CertificateExtensions.Add(new X509KeyUsageExtension(
            X509KeyUsageFlags.KeyAgreement | X509KeyUsageFlags.DataEncipherment |
            X509KeyUsageFlags.KeyEncipherment | X509KeyUsageFlags.DigitalSignature, true));
        request.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(
            new OidCollection { new Oid("1.3.6.1.5.5.7.3.1"), new Oid("1.3.6.1.5.5.7.3.2") }, true));
        request.CertificateExtensions.Add(sanBuilder.Build());

        var certificate = request.Create(collection[0], DateTimeOffset.Now.ToUniversalTime(),collection[0].NotAfter, uniqueId.ToByteArray());
        
        var newCert = certificate.CopyWithPrivateKey(rsa);

        var certificatesCollection = new Dictionary<string, object>
        {
            { "CACert", ExportToPem(collection[0]) }, { "ServerCert", ExportToPem(newCert) }
        };
        var builder = new StringBuilder();
        if (!string.IsNullOrEmpty(password))
        {
            builder.AppendLine("-----BEGIN ENCRYPTED PRIVATE KEY-----");
            builder.AppendLine(Convert.ToBase64String(
                rsa.ExportEncryptedPkcs8PrivateKey((password + Seed).ToCharArray(),
                    new PbeParameters(PbeEncryptionAlgorithm.TripleDes3KeyPkcs12, HashAlgorithmName.SHA1, 1)),
                Base64FormattingOptions.InsertLineBreaks));
            builder.AppendLine("-----END ENCRYPTED PRIVATE KEY-----");
            certificatesCollection.Add("ServerCertKey", builder.ToString());
            var filePath = $"{PathAppCert}/{newCert.SerialNumber}";

            await File.WriteAllBytesAsync(filePath + ".pfx", newCert.Export(X509ContentType.Pfx, password + Seed))
                .ConfigureAwait(false);
        }

        using var store = new X509Store(StoreName.My, StoreLocation.CurrentUser, OpenFlags.ReadWrite);
        store.Add(newCert);


        return certificatesCollection;

    }
}