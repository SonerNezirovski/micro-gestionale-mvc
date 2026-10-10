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
            .Include(preventivo => preventivo.Righe)
            .OrderByDescending(preventivo => preventivo.Data)
            .ThenByDescending(preventivo => preventivo.Id)
            .ToListAsync();

        return View(preventivi);
    }

    // CARICA I CLIENTI NEL MENU A TENDINA
    // (i clienti bloccati sono esclusi, tranne quello già
    // collegato al preventivo che si sta modificando)
    private async Task CaricaClientiAsync(
        PreventivoCreateViewModel model,
        int? clienteCorrenteId = null)
    {
        model.ClientiDisponibili = await _context.Clienti
            .AsNoTracking()
            .Where(cliente => !cliente.Bloccato
                              || cliente.Id == clienteCorrenteId)
            .OrderBy(cliente => cliente.Nome)
            .Select(cliente => new SelectListItem
            {
                Value = cliente.Id.ToString(),
                Text = cliente.Nome
            })
            .ToListAsync();
    }

    // CONTROLLI COMUNI A CREAZIONE E MODIFICA DELLA TESTATA
    private async Task ValidaTestataAsync(
        PreventivoCreateViewModel model,
        int? clienteCorrenteId = null)
    {
        // Controlla che il cliente esista e non sia bloccato
        if (model.ClienteId.HasValue)
        {
            var clienteAttivo = await _context.Clienti.AnyAsync(
                cliente => cliente.Id == model.ClienteId.Value
                           && (!cliente.Bloccato
                               || cliente.Id == clienteCorrenteId));

            if (!clienteAttivo)
            {
                ModelState.AddModelError(
                    nameof(model.ClienteId),
                    "Il cliente non esiste oppure è bloccato.");
            }
        }

        // Controlla che il numero non sia già usato
        if (!string.IsNullOrWhiteSpace(model.Numero))
        {
            var numero = model.Numero.Trim();
            var numeroUsato = await _context.Preventivi.AnyAsync(
                p => p.Numero == numero && p.Id != model.Id);

            if (numeroUsato)
            {
                ModelState.AddModelError(
                    nameof(model.Numero),
                    "Esiste già un preventivo con questo numero.");
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
        model.Id = 0;
        await ValidaTestataAsync(model);

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

        return RedirectToAction(nameof(Details), new { id = preventivo.Id });
    }

    // APRE LA PAGINA DI MODIFICA DELLA TESTATA
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var preventivo = await _context.Preventivi
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (preventivo == null)
        {
            return NotFound();
        }

        var model = new PreventivoCreateViewModel
        {
            Id = preventivo.Id,
            Numero = preventivo.Numero,
            Data = preventivo.Data,
            ClienteId = preventivo.ClienteId,
            ValidoFinoAl = preventivo.ValidoFinoAl,
            Note = preventivo.Note
        };

        await CaricaClientiAsync(model, preventivo.ClienteId);

        return View(model);
    }

    // SALVA LA TESTATA MODIFICATA
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        PreventivoCreateViewModel model)
    {
        var preventivo = await _context.Preventivi.FindAsync(id);

        if (preventivo == null)
        {
            return NotFound();
        }

        model.Id = id;
        await ValidaTestataAsync(model, preventivo.ClienteId);

        if (!ModelState.IsValid)
        {
            await CaricaClientiAsync(model, preventivo.ClienteId);
            return View(model);
        }

        preventivo.Numero = model.Numero.Trim();
        preventivo.Data = model.Data!.Value;
        preventivo.ClienteId = model.ClienteId!.Value;
        preventivo.ValidoFinoAl = model.ValidoFinoAl;
        preventivo.Note = model.Note;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id });
    }

    // ELIMINA IL PREVENTIVO (CON TUTTE LE SUE RIGHE)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var preventivo = await _context.Preventivi.FindAsync(id);

        if (preventivo == null)
        {
            return NotFound();
        }

        _context.Preventivi.Remove(preventivo);
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

    // APRE LA PAGINA DI MODIFICA DI UNA RIGA
    [HttpGet]
    public async Task<IActionResult> ModificaRiga(int id)
    {
        var riga = await _context.RighePreventivo
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id);

        if (riga == null)
        {
            return NotFound();
        }

        var model = new RigaPreventivoCreateViewModel
        {
            Id = riga.Id,
            PreventivoId = riga.PreventivoId,
            Descrizione = riga.Descrizione,
            Quantita = riga.Quantita,
            PrezzoUnitario = riga.PrezzoUnitario,
            ScontoPercentuale = riga.ScontoPercentuale,
            AliquotaIva = riga.AliquotaIva
        };

        return View(model);
    }

    // SALVA LA RIGA MODIFICATA
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ModificaRiga(
        int id,
        RigaPreventivoCreateViewModel model)
    {
        var riga = await _context.RighePreventivo.FindAsync(id);

        if (riga == null)
        {
            return NotFound();
        }

        // La riga resta sempre sul suo preventivo
        model.Id = id;
        model.PreventivoId = riga.PreventivoId;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        riga.Descrizione = model.Descrizione.Trim();
        riga.Quantita = model.Quantita;
        riga.PrezzoUnitario = model.PrezzoUnitario;
        riga.ScontoPercentuale = model.ScontoPercentuale;
        riga.AliquotaIva = model.AliquotaIva;

        await _context.SaveChangesAsync();

        return RedirectToAction(
            nameof(Details),
            new { id = riga.PreventivoId });
    }

    // ELIMINA UNA RIGA
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EliminaRiga(int id)
    {
        var riga = await _context.RighePreventivo.FindAsync(id);

        if (riga == null)
        {
            return NotFound();
        }

        _context.RighePreventivo.Remove(riga);
        await _context.SaveChangesAsync();

        return RedirectToAction(
            nameof(Details),
            new { id = riga.PreventivoId });
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
