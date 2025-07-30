using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using SmartDesk.Application.Configurations;

public class SmtpHealthCheck : IHealthCheck
{
    private readonly SmtpSettings _settings;
    public SmtpHealthCheck(IOptions<SmtpSettings> opts) => _settings = opts.Value;

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var tcp = new TcpClient();
            await tcp.ConnectAsync(_settings.Host, _settings.Port);
            return HealthCheckResult.Healthy("SMTP endpoint reachable");
        }
        catch (System.Exception ex)
        {
            return HealthCheckResult.Unhealthy("SMTP endpoint unreachable", ex);
        }
    }
}
