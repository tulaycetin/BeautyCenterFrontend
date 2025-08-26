using Microsoft.AspNetCore.Components.Server.Circuits;

namespace BeautyCenterFrontend.Services
{
    public class CustomCircuitHandler : CircuitHandler
    {
        private readonly ILogger<CustomCircuitHandler> _logger;

        public CustomCircuitHandler(ILogger<CustomCircuitHandler> logger)
        {
            _logger = logger;
        }

        public override Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Circuit {circuit.Id} opened");
            return base.OnCircuitOpenedAsync(circuit, cancellationToken);
        }

        public override Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Circuit {circuit.Id} closed");
            return base.OnCircuitClosedAsync(circuit, cancellationToken);
        }

        public override Task OnConnectionDownAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            _logger.LogWarning($"Circuit {circuit.Id} connection down");
            return base.OnConnectionDownAsync(circuit, cancellationToken);
        }

        public override Task OnConnectionUpAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Circuit {circuit.Id} connection up");
            return base.OnConnectionUpAsync(circuit, cancellationToken);
        }
    }
}