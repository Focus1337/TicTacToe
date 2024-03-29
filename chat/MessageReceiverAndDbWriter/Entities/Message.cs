using System.ComponentModel.DataAnnotations;

namespace MessageReceiverAndDbWriter.Entities;

public class Message
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string Text { get; set; } = null!;

    [Required, DataType(DataType.Date)]
    public DateTime DateTime { get; set; }

    [Required]
    public string UserName { get; set; } = null!;
    public Guid GameId { get; set; }
}