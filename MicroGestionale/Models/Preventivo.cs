using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
    [NotMapped]
    public decimal TotaleImponibile => Righe.Sum(r => r.Imponibile);

    [NotMapped]
    public decimal TotaleIva => Righe.Sum(r => r.ImportoIva);

    [NotMapped]
    public decimal TotalePreventivo => TotaleImponibile + TotaleIva;
}