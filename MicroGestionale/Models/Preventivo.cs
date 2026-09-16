using System.ComponentModel.DataAnnotations;

namespace MicroGestionale.Models;

public class Preventivo
{
    public int Id { get; set; }

    [Required]
    [StringLength(30)]
    public string Numero { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    public DateTime Data { get; set; } = DateTime.Today;

    [DataType(DataType.Date)]
    public DateTime? ValidoFinoAl { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Seleziona un cliente.")]
    public int ClienteId { get; set; }

    public Cliente Cliente { get; set; } = null!;

    [StringLength(2000)]
    public string? Note { get; set; }

    public List<RigaPreventivo> Righe { get; set; } = new();
}