using MediatR;

namespace Orchestration.Models.DeviceGateway;

public class CreateDeviceGatewayCommandModel  : BaseDeviceGatewayCommandModel, IRequest<int> {}
