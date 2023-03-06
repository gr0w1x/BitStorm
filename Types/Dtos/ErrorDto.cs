using System.ComponentModel.DataAnnotations;

namespace Types.Dtos;

public record ErrorDto([Required] string Message);
