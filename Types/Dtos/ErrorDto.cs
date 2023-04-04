using System.ComponentModel.DataAnnotations;

namespace Types.Dtos;

public record ErrorDto(
    [Required] string Message,
    [Required] int Code
);

public class ApiErrorException: Exception
{
    public readonly ErrorDto Error;

    public ApiErrorException(ErrorDto dto): base(dto.Message)
    {
        Error = dto;
    }
}
