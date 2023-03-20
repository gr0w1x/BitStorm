using System.Net;
using System.ComponentModel.DataAnnotations;

namespace Types.Dtos;

public record ErrorDto(
    [Required] string Message,
    [Required] HttpStatusCode StatusCode
);

public class ApiErrorException: Exception
{
    public readonly ErrorDto Error;

    public ApiErrorException(ErrorDto dto): base(dto.Message)
    {
        Error = dto;
    }
}
