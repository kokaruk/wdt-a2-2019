using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WdtA2Api.Models;

namespace WdtA2Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SlotsController : ControllerBase
    {
        private readonly WdtA2ApiContext _context;

        public SlotsController(WdtA2ApiContext context)
        {
            _context = context;
        }

        // GET: api/Slots
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Slot>>> GetSlot()
        {
            return await _context.Slot.ToListAsync();
        }

        // GET: api/Slots/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Slot>> GetSlot(string id)
        {
            var slot = await _context.Slot.FindAsync(id);

            if (slot == null)
            {
                return NotFound();
            }

            return slot;
        }

        // PUT: api/Slots/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSlot(string id, Slot slot)
        {
            if (id != slot.RoomID)
            {
                return BadRequest();
            }

            _context.Entry(slot).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SlotExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Slots
        [HttpPost]
        public async Task<ActionResult<Slot>> PostSlot(Slot slot)
        {
            _context.Slot.Add(slot);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSlot", new { id = slot.RoomID }, slot);
        }

        // DELETE: api/Slots/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Slot>> DeleteSlot(string id)
        {
            var slot = await _context.Slot.FindAsync(id);
            if (slot == null)
            {
                return NotFound();
            }

            _context.Slot.Remove(slot);
            await _context.SaveChangesAsync();

            return slot;
        }

        private bool SlotExists(string id)
        {
            return _context.Slot.Any(e => e.RoomID == id);
        }
    }
}
