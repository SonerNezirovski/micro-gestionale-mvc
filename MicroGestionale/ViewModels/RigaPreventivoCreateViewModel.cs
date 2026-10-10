using System.ComponentModel.DataAnnotations;

namespace MicroGestionale.ViewModels;

public class RigaPreventivoCreateViewModel
{
    // 0 quando la riga è nuova
    public int Id { get; set; }

    [Range(1, int.MaxValue)]
    public int PreventivoId { get; set; }

    [Required(ErrorMessage = "Inserisci una descrizione.")]
    [StringLength(500)]
    public string Descrizione { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.001", "1000000",
        ErrorMessage = "La quantità deve essere maggiore di zero.",
        ParseLimitsInInvariantCulture = true)]
    public decimal Quantita { get; set; } = 1m;

    [Range(typeof(decimal), "0", "1000000000",
        ErrorMessage = "Il prezzo non può essere negativo.",
        ParseLimitsInInvariantCulture = true)]
    public decimal PrezzoUnitario { get; set; }

    [Range(typeof(decimal), "0", "100", ParseLimitsInInvariantCulture = true)]
    public decimal ScontoPercentuale { get; set; }

    [Range(typeof(decimal), "0", "100", ParseLimitsInInvariantCulture = true)]
    public decimal AliquotaIva { get; set; } = 22m;
}