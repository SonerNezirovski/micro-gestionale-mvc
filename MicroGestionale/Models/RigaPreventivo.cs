using System.ComponentModel.DataAnnotations;

namespace MicroGestionale.Models;

public class RigaPreventivo
{
    public int Id { get; set; }

    public int PreventivoId { get; set; }

    public Preventivo Preventivo { get; set; } = null!;

    [Range(1, int.MaxValue)]
    public int Posizione { get; set; } = 1;

    [Required(ErrorMessage = "Inserisci una descrizione.")]
    [StringLength(500)]
    public string Descrizione { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.001", "1000000")]
    public decimal Quantita { get; set; } = 1m;

    [Range(typeof(decimal), "0", "1000000000")]
    public decimal PrezzoUnitario { get; set; }

    [Range(typeof(decimal), "0", "100")]
    public decimal ScontoPercentuale { get; set; }

    [Range(typeof(decimal), "0", "100")]
    public decimal AliquotaIva { get; set; } = 22m;
}