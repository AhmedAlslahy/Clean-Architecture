using Application.Common.Abstraction;
using Domain.Enums;

namespace Application.Common.Errors;

public class MessageErrors
{
    public static Error NotFound
      = new Error("Message.NotFound", "Message not found", ErrorType.NotFound);

    public static Error InvalidData
       = new Error("Message.InvalidData", "Invalid Message data", ErrorType.BadRequest);
}