using System.ComponentModel.DataAnnotations;

namespace MicroGestionale.ViewModels;

public class RigaPreventivoCreateViewModel
{
    [Range(1, int.MaxValue)]
    public int PreventivoId { get; set; }

    [Required(ErrorMessage = "Inserisci una descrizione.")]
    [StringLength(500)]
    public string Descrizione { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.001", "1000000",
        ErrorMessage = "La quantità deve essere maggiore di zero.")]
    public decimal Quantita { get; set; } = 1m;

    [Range(typeof(decimal), "0", "1000000000",
        ErrorMessage = "Il prezzo non può essere negativo.")]
    public decimal PrezzoUnitario { get; set; }

    [Range(typeof(decimal), "0", "100")]
    public decimal ScontoPercentuale { get; set; }

    [Range(typeof(decimal), "0", "100")]
    public decimal AliquotaIva { get; set; } = 22m;
}