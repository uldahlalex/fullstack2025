using System.ComponentModel.DataAnnotations;
using Application.Models.Dtos;
using Core.Domain.Entities;

namespace Infrastructure.Mqtt;

public class ServerSendsMetricToAdmin : ApplicationBaseDto
{
    [Required]
    public List<Devicelog> Metrics { get; set; } = new();
}