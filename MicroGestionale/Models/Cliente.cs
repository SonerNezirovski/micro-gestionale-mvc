using System.ComponentModel.DataAnnotations;

namespace MicroGestionale.Models;

public class Cliente
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Inserisci il nome o la ragione sociale.")]
    [StringLength(150)]
    public string Nome { get; set; } = string.Empty;

    [StringLength(20)]
    public string? PartitaIva { get; set; }

    [StringLength(16)]
    public string? CodiceFiscale { get; set; }

    [StringLength(200)]
    public string? Indirizzo { get; set; }

    [StringLength(10)]
    public string? Cap { get; set; }

    [StringLength(100)]
    public string? Citta { get; set; }

    [StringLength(50)]
    public string? Provincia { get; set; }

    [StringLength(100)]
    public string Nazione { get; set; } = "Italia";

    [EmailAddress(ErrorMessage = "Inserisci un indirizzo email valido.")]
    public string? Email { get; set; }

    [StringLength(30)]
    public string? Telefono { get; set; }

    public bool Bloccato { get; set; } = false;

    public List<Preventivo> Preventivi { get; set; } = new();
}