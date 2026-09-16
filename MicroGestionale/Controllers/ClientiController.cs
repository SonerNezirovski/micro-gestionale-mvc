using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MicroGestionale.Data;
using MicroGestionale.Models;
namespace MicroGestionale.Controllers;

public class ClientiController : Controller
{
    private readonly GestionaleDbContext _context;

    public ClientiController(GestionaleDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var clienti = await _context.Clienti
            .AsNoTracking()
            .OrderBy(cliente => cliente.Nome)
            .ToListAsync();

        return View(clienti);
    }
    [HttpGet]
    public IActionResult Create()
    {
        return View(new Cliente());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("Nome,PartitaIva,CodiceFiscale,Indirizzo,Cap,Citta,Provincia,Nazione,Email,Telefono")]
    Cliente cliente)
    {
        if (!ModelState.IsValid)
        {
            return View(cliente);
        }

        _context.Clienti.Add(cliente);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var cliente = await _context.Clienti.FindAsync(id);

        if (cliente == null)
        {
            return NotFound();
        }

        return View(cliente);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, IFormCollection form)
    {
        var cliente = await _context.Clienti.FindAsync(id);

        if (cliente == null)
        {
            return NotFound();
        }

        var aggiornamentoValido = await TryUpdateModelAsync<Cliente>(
            cliente,
            "",
            c => c.Nome,
            c => c.PartitaIva,
            c => c.CodiceFiscale,
            c => c.Indirizzo,
            c => c.Cap,
            c => c.Citta,
            c => c.Provincia,
            c => c.Nazione,
            c => c.Email,
            c => c.Telefono);

        if (!aggiornamentoValido)
        {
            return View(cliente);
        }

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}