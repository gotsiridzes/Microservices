using System.ComponentModel.DataAnnotations;

namespace CommandService.DataTransferObjects;

public class CommandCreateDto
{
    [Required]
    public string HowTo { get; set; }

    [Required]
    public string CommandLine { get; set; }
}
