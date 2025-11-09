using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PortHub.Api.Data;
using PortHub.Api.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PortHub.Api.Services
{

    /// Servicio en segundo plano que libera automáticamente slots reservados que expiraron.

    public class SlotCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SlotCleanupService> _logger;
        private readonly SlotReservationOptions _options;

        public SlotCleanupService(
            IServiceProvider serviceProvider,
            ILogger<SlotCleanupService> logger,
            IOptions<SlotReservationOptions> options)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_options.AutoCleanupEnabled)
            {
                _logger.LogInformation("Limpieza automática deshabilitada.");
                return;
            }

            _logger.LogInformation(
                "Servicio iniciado. Intervalo: {Interval} min, Timeout: {Timeout} min",
                _options.CleanupIntervalMinutes,
                _options.TimeoutMinutes
            );

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CleanupExpiredSlotsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error durante la limpieza de slots.");
                }

                // Espera el próximo ciclo
                await Task.Delay(
                    TimeSpan.FromMinutes(_options.CleanupIntervalMinutes),
                    stoppingToken
                );
            }
        }

        private async Task CleanupExpiredSlotsAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var now = DateTime.UtcNow;

            // Busca slots reservados cuyo tiempo de expiración ya pasó
            var expiredSlots = await context.Slots
                .Where(s =>
                    s.Status == "Reservado" &&
                    s.ReservationExpiresAt != null &&
                    s.ReservationExpiresAt < now
                )
                .ToListAsync();

            if (expiredSlots.Any())
            {
                _logger.LogInformation("Liberando {Count} slots expirados.", expiredSlots.Count);

                foreach (var slot in expiredSlots)
                {
                    slot.Status = "Libre";
                    slot.ReservationExpiresAt = null;

                    _logger.LogInformation(
                        "Slot {Id} (Vuelo: {Flight}) liberado por timeout.",
                        slot.Id,
                        slot.FlightCode
                    );
                }

                await context.SaveChangesAsync();
            }
        }
    }
}
