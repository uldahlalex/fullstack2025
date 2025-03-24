using System.ComponentModel.DataAnnotations;
using Application.Models.Dtos;
using Core.Domain.Entities;

namespace Infrastructure.Mqtt;

public class ServerSendsMetricToAdminDto : ApplicationBaseDto
{
    [Required]
    public List<Devicelog> Metrics { get; set; } = new();

    public override string EventType { get; set; } = nameof(ServerSendsMetricToAdminDto);
}