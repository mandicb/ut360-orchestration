using Administration.FunctionalTests;
using Microsoft.Extensions.DependencyInjection;
using Orchestration.Models.Credential;
using Utrust360.Crypto.Services.Abstraction;


namespace Orchestration.UnitTests.Scenarios;

public class CredentialValidation
{
    private ICryptoService _cryptoService;
    
    [SetUp]
    public void Setup()
    {
        var fact = new CustomWebApplicationFactory();
        var scopeFactory = fact.Services.GetRequiredService<IServiceScopeFactory>(); 
        var scope = scopeFactory.CreateScope();
        _cryptoService = scope.ServiceProvider.GetRequiredService<ICryptoService>();

    }

    [Test]
    [TestCaseSource(nameof(MockCredential))]
    public void check_KeyDataValidation(CreatCredentialCommandModel model)
    {
        Assert.DoesNotThrowAsync(() =>  Task.FromResult(DataContext.Models.Credential.Create(model, _cryptoService)));
    }
    
    private static IEnumerable<TestCaseData> MockCredential
    {
        get
        {
            yield return new TestCaseData(new CreatCredentialCommandModel()
            {
                Name = "Test",
                UserName = "admin",
                KeyPassword = "password",
                KeyData = @"-----BEGIN EC PRIVATE KEY-----
                    MIHcAgEBBEIBFLQzRgfISKgu2DxtAEYDYDt52SGx2Y1sDorLIYWmQFuQjN5UIxBV
                    nwve/gYlS2y8c8ZScvmcd8xWk2KlGpzn9M+gBwYFK4EEACOhgYkDgYYABAG6/WHw
                    zz/3J22NDC5BJIRe/f1BaSHN5mwxpBRU5V4+xtS5xSxACZy0Rud+6pbUjIm8fEWj
                    SVp86h608xpxeO1lMQGZ9D1tW19kmRtTw8/Bf55M/KohX1xOJccSPQ7SLVbZezmG
                    6G69QXkVCzsDhG34PCRQdje0bZWDIr1Pw5pLRYmBrg==
                    -----END EC PRIVATE KEY-----"
                    
            }).SetName("credential-valid-EC");
            yield return new TestCaseData(new CreatCredentialCommandModel()
            {
                Name = "Test",
                UserName = "admin",
                KeyPassword = "123456",
                KeyData = @"-----BEGIN ENCRYPTED PRIVATE KEY-----
                    MIIFLTBXBgkqhkiG9w0BBQ0wSjApBgkqhkiG9w0BBQwwHAQIqudM4c3v4HUCAggA
                    MAwGCCqGSIb3DQIJBQAwHQYJYIZIAWUDBAEqBBAk/khz9d3q7pZO+4AiJLM+BIIE
                    0IWxPLzoheWDbFU6Es2MHiJ28S+O15T1lQkVBBOzjqH7kndQhEszLQoz+kiVYDUa
                    NHDre/1AWexZUzPH3c86oV+HUuglgf/2WvtVzAvNVj1myTQmNPxpCnusb+NbfmeT
                    4EiZfIHrEy4lTqxniP5pfhDDVjO4OprlEiBeG9rgl9ip6S2FtEeBbG039tcXzNfA
                    lgONMNeXnCUvv2Od5tHhIAVMNOR0H8uvRN5r6JxtTu4Ec2H1r48tMuejjTamPXf9
                    8OyH7JLJfZESvlKR1J44WDWQiCOdwNEHNF7+qvs3/69iniguZeQpgbabnxx0KkTo
                    cmZAUvkP8i/X2qwcaJ4Qz17Jhh+g7Zo+H2cFZFhuxhIixcWSxpyLCNxRrUnFZe0S
                    uCBeHhBfP5Xfq9e8pqK+tgmRbTI6H3JnL5hLJQ/8KtiY1q4dtojlYsUtq3abf587
                    nA7Gh4qGSYAI8uEXqfHKfnYbK9jlm2Hq1+pHYg1uLilw5Em/0xxowqLA2tPRFWdq
                    i/EVtzDxB22WIWOPRd/LBg+Zxft3kmZiz17sgdK1u6F3nzxkP9KfzaOqqPUVs/2A
                    r2N80KBuZXg/FqZVAu4QsWSABMf44wzLSd9/DX6Qp9furkv4P2eFVIP42owpJRz4
                    rh1caSE0YYA3ldIazabW/VsNdL7FxYadpHF49hq3E0eupB6bNVA4GZgp4M8ggJb4
                    Vx+U5eNGSeJlIMuRaIgGzneqnmvU4XoMYWzdIMNDKv16XZH2jwzMPMFR97J0oaqH
                    bA7WkHplu7WBkJHI/HRQUcZAt3AqadqrNCf2sdeYsKaNy/66Xg+GAObnA4Kq/s/s
                    4vAz3hpvLksUVX37lvhd2xu1aj7vTl69Z/3B14ibsjRNoCzvZgzY/s3Ec/57UIzU
                    z+smZ8JGx1uilHgVsh4d6wI7oNHHkaRMZXN9E6t4pe4q+BP2C1hdeUOMvR7N2Aw7
                    g9/tEeO2L8DbkSYOelzfySz2esV2tOUaBU1w1v64sGJrBSnwGT1V0qwIVRRzKO2I
                    yWHAoxCYfeBW9N0+rPgQro1z5+iu4rZ8BYvwRV78OdGIBdV8MGB/1nJKHlk4hbZ2
                    EgYIl7DuzGcdMs+HfQSSGDiccUdYFYXygQ+/pKPIHokiCI0GojYq6A66XwsNv7nH
                    y+lYOEHOfBW+LsSDf99QvGNzyyAvTEM9tlwsYwOzUlPnq7E8vk66ZOvRF0Sxuaao
                    QkcHTs1YHsdZavubMhqc4upn7y/pYGS64U05RwB0mpVdUlxSVqpEoD/AMc1y3Q6J
                    SAYz8NGc1QQ3pEV39u9WUnvcoHCMaSB5EZ3/fluGFVe43XCTbLMYTGyWkO12aNWa
                    7jGX6sUue7T6AHboMLQ4wzYzLRghqIS+3tLDJJXr5oi6J/C/2fYQLrT2cB0JFJ84
                    VXrjSTwuEvrZbwtVGqc2bT6g6M8j/YDKhXCRR9nItzzAfvNGd5jtxYGVBaigtkld
                    e6UFEux27JvN6jqCRNxBAxE0ICXNDMwBffSnXzw5Z8ffqalDizbecJ4CFMyrbwbU
                    ykgdNxmJIbGKVOioHaD2CS13ybj52HrNba+Diiz66JGKR+LxdW4B0EP7Mt581tuN
                    VrP9VTLQFXuU5aCMkM3ujxbWsN/DNVyNTunzEDJAxXMa
                    -----END ENCRYPTED PRIVATE KEY-----"
                
            }).SetName("credential-valid-encrypted-RSA");
        }
    }
}