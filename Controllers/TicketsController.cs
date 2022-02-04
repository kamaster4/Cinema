using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cinema.Data;
using Cinema.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Cinema.Controllers
{
    [Authorize()]
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TicketsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Tickets
        public async Task<IActionResult> Index(string userId)
        {
            List<Ticket> list = await (from tick in _context.ticket
                                       where tick.UserId == userId
                                       select new Ticket
                                       {
                                           Id = tick.Id,
                                           MovieName = tick.MovieName,
                                           SeatRow = tick.SeatRow,
                                           SeatNumber = tick.SeatNumber,
                                           Date = tick.Date,
                                           MovieId = tick.MovieId,
                                           UserId = tick.UserId
                                       }).ToListAsync();

            return View(list);
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.ticket
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // GET: Tickets/Create
        public IActionResult Create(int movieId)
        {
            var movie = _context.movie.Find(movieId);

            Ticket ticket = new Ticket
            {
                MovieId = movieId,
                UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                Date = movie.Date,
                MovieName = movie.Name
            };

            return View(ticket);
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MovieName,SeatRow,SeatNumber,Date,MovieId,UserId")] Ticket ticket)
        {
            List<Ticket> list = await (from tick in _context.ticket
                                       where tick.MovieId == ticket.MovieId
                                       where tick.SeatRow == ticket.SeatRow
                                       where tick.SeatNumber == ticket.SeatNumber
                                       select new Ticket
                                       {
                                           Id = tick.Id,
                                           MovieName = tick.MovieName,
                                           SeatRow = tick.SeatRow,
                                           SeatNumber = tick.SeatNumber,
                                           Date = tick.Date,
                                           MovieId = tick.MovieId,
                                           UserId = tick.UserId
                                       }).ToListAsync();

            if (list.Count == 0)
            {
                if (ModelState.IsValid)
                {
                    _context.Add(ticket);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index), new { movieId = ticket.MovieId });
                }
            }
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.ticket.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MovieName,SeatRow,SeatNumber,Date,MovieId,UserId")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.ticket
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.ticket.FindAsync(id);
            _context.ticket.Remove(ticket);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TicketExists(int id)
        {
            return _context.ticket.Any(e => e.Id == id);
        }
    }
}
