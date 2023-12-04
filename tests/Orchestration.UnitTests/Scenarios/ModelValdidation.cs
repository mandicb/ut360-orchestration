using System.ComponentModel.DataAnnotations;
using Orchestration.Models.DeviceGateway;

namespace Orchestration.UnitTests.Scenarios;

public class ModelValidation
{
 
    [Test]
    public void Device_gateway_Null_Model()
    {
        Assert.That(ValidateModel(new CreateDeviceGatewayCommandModel()).Any(x => x.MemberNames.Contains("Name") || x.MemberNames.Contains("Password")  ||  x.MemberNames.Contains("HostName")), Is.True);
    }
    
    
    [Test]
    [TestCaseSource(nameof(MockDeviceGatewayData))]
    public void Device_gateway_Name_Prop_Validation(CreateDeviceGatewayCommandModel model)
    {
        Assert.Multiple(() =>
        {
            Assert.That(ValidateModel(model).Count(), Is.EqualTo(2));
            Assert.That(ValidateModel(model).Any(x => x.ErrorMessage != null && x.MemberNames.Contains("Name") && x.ErrorMessage.Contains("Must be at least 1 and at most 150 characters long.")), Is.True);
        });
    }

    [Test]
    [TestCaseSource(nameof(MockDeviceGatewayData))]
    public void Device_gateway_NamePassword_Prop_Validation(CreateDeviceGatewayCommandModel model)
    {
        Assert.That(ValidateModel(model).Any(x => 
            x.ErrorMessage != null && ((x.MemberNames.Contains("Name") && x.ErrorMessage.Contains("Must be at least 1 and at most 150 characters long.")) ||
                                       (x.MemberNames.Contains("Password") && x.ErrorMessage.Contains("Must be at least 1 and at most 150 characters long.")))
            ), Is.True);
    }

    [Test]
    [TestCaseSource(nameof(MockDeviceGatewayDataPortValidation))]
    public void Device_gateway_PortHostName_Prop_Validation(CreateDeviceGatewayCommandModel model)
    {
        Assert.Multiple(() =>
        {
            Assert.That(ValidateModel(model).Count(), Is.EqualTo(2));
            Assert.That(ValidateModel(model).Any(x =>
                x.ErrorMessage != null && ((x.MemberNames.Contains("Port") && x.ErrorMessage.Contains("Port number must be between 1 to 65536.")))
            ), Is.True);
        });
    }
    
    [Test]
    [TestCaseSource(nameof(MockDeviceGatewayDataValid))]
    public void Device_gateway_Validation_Success(CreateDeviceGatewayCommandModel model)
    {
        Assert.That(ValidateModel(model).Count(), Is.EqualTo(0));

    }

    private static IEnumerable<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var ctx = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, ctx, validationResults, true);
        return validationResults;
    }

    private static IEnumerable<TestCaseData> MockDeviceGatewayData
    {
         get
         {
             yield return new TestCaseData(new CreateDeviceGatewayCommandModel()
             {
                 Name = string.Empty,
                 Port = 79,
                 Password = string.Empty,
                 HostName = "172.168.1.1"
             }).SetName("deviceGateway-emptyProp");

             yield return new TestCaseData(new CreateDeviceGatewayCommandModel()
             {
                 Name =
                     "7OrXTWOMAqrwaatqjPRbphcrAe3NtmklUzhJF1o1Qx580AmHYiLVXlm9Szinp1uUjVmClv6chjrt7VBgAv36bAq17LWxi3SYX1Mngb7QVhgFJYYM2JCMdJ0itYN1i79wpBxXKb89iTm826wnrlqFBj1oeyEXxNLH18TixqJnyvaMCUGGXjYWoWAy3AYtxykqfjYp1TPxXidDDn5nFQumt0DK90wJP73mC3abLivMTu5beTXw0RnS4b6Fa2uVEihGF4DROx8HsO8wpm0QSvgb1LytdBx2lTP7ArjZbIZOisZV4FrityKKVzihL8IGK4wR2uqQj4WK5DmdJ6j5emKYKwcvetNVVHhJ1pMJ4scsUVct8s7jyJPzaam5S6wNwfpOo441sxzaShrx3nnEQVVS3hZecw1RYwaEMIw9JGgsyf7A1QqdfgGDZYVCAVg8MIfiZJckc0SSFFmHu9blTQla1tVUgu3bbBCb1GymIwQrrGYEkwou7KwVNqTF1IwohWi2zA7gWBr15HqWukWGHgEzFbSfsDOrPI56y6sedUD0MczZV2yHkqi6i5KQATl7gU9NiK6u3g8wSQYKukxcL0sodH5g45YfjzraXNkC9CQgw0e7Ujv8zr7QZXq2cROa4DQ4o6kWlKy5ebVCuqbdJpuwiGtH4E3AdLRrlTFynPsbeyiNASJcwDxOjit3uHojZ3Xdpn5SzIHu4YKc1k1evq8x8CLTbWk9iaNMiI6PBfMCoHrAUCY3wWhFa4u3VIDqV0zJIxmf2csVNo6h8ixfqqj8YRwhLHqPS7n1PL1jjWPlFG60mX32Otuj4VjNZP7wXIZjck3giNzc8E20fuTiMCOpQXCKjSeYUStZ6J45N88d4LtnFs8jxecTJyr2E1a9j4dWhvfNq7GfoYv4cDPROTJuYWD5SqBbuhB2gO5SWV55fd9DrOrWotJdMbwR5mE9fQBpV1L38GziDUEJTjmzFavhiHEfp50mTixp9XjLRI1sp8GcXfm8LEIp3xdO3GumNZuSbrCT9fbS4MQuDX8yG1dra48rNEkBhurN2DfJCh5PxMRykL5pv1XW3sm4ONKT5TB1ut1dQjAdu7uJ3UOGfwHUFIQG6AyIoyoWh799v9ag4JXtXeHBc8SQ7ejdWNIhrWst27kprZq9hn7GtudcIboLx2sBckLGF44OPdAcnTPYBXuPfRNmkiUshfZoBMBGmfjE5aaKsrSMtqHYk6t66EQzDZZ2O9gIM8ESV5OAXwV59bO23MKKAgbVfoSA1LB9JsBUwUdTZ8udzCcJ17CHffaKzTZHGRxJmy1S4ZlPqvgG7vlQ2JAdnvrL9IwzbprSjoBCCoIfrx1xG2exKHJ2uodGkooKOnU726dAOSzIZNd21PkRTmhjZkgQgWVGhWjAbsv1bOcMgfZUL64gV9lAXJ7w0epiTnBGxSoOJu8DdXoGaHSqjOWkWlmFH2RgHwwrGpLYsNv9tQrzamk4gBM3FDkynzqhoS8ldmrl26qsOu",
                 Port = 79,
                 Password = "",
                 HostName = "172.168.1.1"
             }).SetName("deviceGateway-toLogProp");
         }
    }

    private static IEnumerable<TestCaseData> MockDeviceGatewayDataPortValidation
    {
        get
         {
             yield return new TestCaseData(new CreateDeviceGatewayCommandModel
             {
                 Name = "Neki",
                 Port = 0,
                 Password = "asdasd",
                 HostName = "local@sd.com.asd"
             }).SetName("deviceGateway-zeroPort");

             yield return new TestCaseData(new CreateDeviceGatewayCommandModel
             {
                 Name =
                     "ASDASD",
                 Port = 70000,
                 Password = "asD",
                 HostName = "1.1.1"
             }).SetName("deviceGateway-toBigPort");
         }
    }



     private static IEnumerable<TestCaseData> MockDeviceGatewayDataValid
    {
         get
         {
             yield return new TestCaseData(new CreateDeviceGatewayCommandModel()
             {
                 Name = "asdasd",
                 Port = 79,
                 Password = "sd",
                 HostName = "172.168.1.1"
             }).SetName("deviceGateway-ip");

             yield return new TestCaseData(new CreateDeviceGatewayCommandModel()
             {
                 Name = "asdasd",
                 Port = 1,
                 Password = "asdad",
                 HostName = "localhost.local"
             }).SetName("deviceGateway-hostname");
             
             
             yield return new TestCaseData(new CreateDeviceGatewayCommandModel()
             {
                 Name = "asdasd",
                 Port = 243,
                 Password = "asd",
                 HostName = "::1"
             }).SetName("deviceGateway-ipV6");
         }
    }


}