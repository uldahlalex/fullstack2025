using System.ComponentModel.DataAnnotations;
using WebSocketBoilerplate;

namespace Api.Websocket.EventHandlers.ServerEventDtos;

public class ServerSendsErrorMessage : BaseDto
{
    [Required]
    public string Error { get; set; }
    [Required]
    public string RequestId { get; set; }
}