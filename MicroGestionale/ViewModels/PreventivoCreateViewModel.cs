using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MicroGestionale.ViewModels;

public class PreventivoCreateViewModel
{
    [Required(ErrorMessage = "Inserisci il numero del preventivo.")]
    [StringLength(30)]
    public string Numero { get; set; } = string.Empty;

    [Required(ErrorMessage = "Inserisci la data.")]
    [DataType(DataType.Date)]
    public DateTime? Data { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Seleziona un cliente.")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleziona un cliente.")]
    public int? ClienteId { get; set; }

    [DataType(DataType.Date)]
    public DateTime? ValidoFinoAl { get; set; }

    [StringLength(2000)]
    public string? Note { get; set; }

    public List<SelectListItem> ClientiDisponibili { get; set; } = new();
}