using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MicroGestionale.Data;
using MicroGestionale.Models;
using MicroGestionale.ViewModels;

namespace MicroGestionale.Controllers;

public class PreventiviController : Controller
{
    private readonly GestionaleDbContext _context;

    public PreventiviController(GestionaleDbContext context)
    {
        _context = context;
    }

    // LISTA PREVENTIVI
    public async Task<IActionResult> Index()
    {
        var preventivi = await _context.Preventivi
            .AsNoTracking()
            .Include(preventivo => preventivo.Cliente)
            .OrderByDescending(preventivo => preventivo.Data)
            .ThenByDescending(preventivo => preventivo.Id)
            .ToListAsync();

        return View(preventivi);
    }

    // CARICA I CLIENTI NEL MENU A TENDINA
    private async Task CaricaClientiAsync(
        PreventivoCreateViewModel model)
    {
        model.ClientiDisponibili = await _context.Clienti
            .AsNoTracking()
            .Where(cliente => !cliente.Bloccato)
            .OrderBy(cliente => cliente.Nome)
            .Select(cliente => new SelectListItem
            {
                Value = cliente.Id.ToString(),
                Text = cliente.Nome
            })
            .ToListAsync();
    }

    // APRE LA PAGINA DI CREAZIONE
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new PreventivoCreateViewModel();

        await CaricaClientiAsync(model);

        return View(model);
    }

    // SALVA IL NUOVO PREVENTIVO
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        PreventivoCreateViewModel model)
    {
        // Controlla che il cliente esista e non sia bloccato
        if (model.ClienteId.HasValue)
        {
            var clienteAttivo = await _context.Clienti.AnyAsync(
                cliente => cliente.Id == model.ClienteId.Value
                           && !cliente.Bloccato);

            if (!clienteAttivo)
            {
                ModelState.AddModelError(
                    nameof(model.ClienteId),
                    "Il cliente non esiste oppure è bloccato.");
            }
        }

        // Controlla le date
        if (model.Data.HasValue
            && model.ValidoFinoAl.HasValue
            && model.ValidoFinoAl.Value.Date < model.Data.Value.Date)
        {
            ModelState.AddModelError(
                nameof(model.ValidoFinoAl),
                "La scadenza non può precedere la data del preventivo.");
        }

        // Se ci sono errori, mostra nuovamente il form
        if (!ModelState.IsValid)
        {
            await CaricaClientiAsync(model);
            return View(model);
        }

        // Crea il preventivo
        var preventivo = new Preventivo
        {
            Numero = model.Numero.Trim(),
            Data = model.Data!.Value,
            ClienteId = model.ClienteId!.Value,
            ValidoFinoAl = model.ValidoFinoAl,
            Note = model.Note
        };

        _context.Preventivi.Add(preventivo);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var preventivo = await _context.Preventivi
            .AsNoTracking()
            .Include(p => p.Cliente)
            .Include(p => p.Righe)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (preventivo == null)
        {
            return NotFound();
        }

        return View(preventivo);
    }
    [HttpGet]
    public async Task<IActionResult> AggiungiRiga(int id)
    {
        var esiste = await _context.Preventivi
            .AnyAsync(p => p.Id == id);

        if (!esiste)
        {
            return NotFound();
        }

        var model = new RigaPreventivoCreateViewModel
        {
            PreventivoId = id
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AggiungiRiga(
        RigaPreventivoCreateViewModel model)
    {
        var esiste = await _context.Preventivi
            .AnyAsync(p => p.Id == model.PreventivoId);

        if (!esiste)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var ultimaPosizione = await _context.RighePreventivo
            .Where(r => r.PreventivoId == model.PreventivoId)
            .MaxAsync(r => (int?)r.Posizione) ?? 0;

        var riga = new RigaPreventivo
        {
            PreventivoId = model.PreventivoId,
            Posizione = ultimaPosizione + 1,
            Descrizione = model.Descrizione.Trim(),
            Quantita = model.Quantita,
            PrezzoUnitario = model.PrezzoUnitario,
            ScontoPercentuale = model.ScontoPercentuale,
            AliquotaIva = model.AliquotaIva
        };

        _context.RighePreventivo.Add(riga);
        await _context.SaveChangesAsync();

        return RedirectToAction(
            nameof(Details),
            new { id = model.PreventivoId });
    }
    [HttpGet]
    public async Task<IActionResult> Stampa(int id)
    {
        var preventivo = await _context.Preventivi
            .AsNoTracking()
            .Include(p => p.Cliente)
            .Include(p => p.Righe)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (preventivo == null)
        {
            return NotFound();
        }

        return View(preventivo);
    }
}