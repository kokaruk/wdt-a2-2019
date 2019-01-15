using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        // POST: api/Slots
        [HttpPost]
        public async Task<ActionResult<Slot>> PostSlot(Slot slot)
        {
            if (SlotExists(slot.RoomID, slot.StartTime))
            {
                return BadRequest();
            }

            _context.Slot.Add(slot);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return BadRequest();
            }

            return CreatedAtAction("GetSlot", slot);
        }

        // PUT: api/Slots/
        [HttpPut]
        public async Task<IActionResult> PutSlot(Slot slot)
        {
            if (!SlotExists(slot.RoomID, slot.StartTime))
            {
                return NotFound();
            }

            _context.Entry(slot).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SlotExists(slot.RoomID, slot.StartTime))
                {
                    return NotFound();
                }
            }
            catch (DbUpdateException)
            {
                if (SlotExists(slot.RoomID, slot.StartTime))
                {
                    return BadRequest();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // GET: api/Slots/5/2019-01-15T13:00
        [HttpGet("{RoomID}/{StartTime}")]
        public async Task<ActionResult<Slot>> GetSlot(string roomId, DateTime startTime)
        {
            var slot = await _context.Slot.FirstOrDefaultAsync(
                           sl => sl.RoomID.Equals(roomId.ToUpper()) && sl.StartTime.Date.Equals(startTime.Date)
                                                                    && sl.StartTime.Hour.Equals(startTime.Hour));

            if (slot == null)
            {
                return NotFound();
            }

            return slot;
        }

        // DELETE: api/Slots/5/2019-01-15T13:00
        [HttpDelete("{RoomID}/{StartTime}")]
        public async Task<ActionResult<Slot>> DeleteSlot(string roomId, DateTime startTime)
        {
            if (!SlotExists(roomId, startTime))
            {
                return BadRequest();
            }

            var slot = await _context.Slot.FirstOrDefaultAsync(
                           sl => sl.RoomID.Equals(roomId.ToUpper()) && sl.StartTime.Date.Equals(startTime.Date)
                                                                    && sl.StartTime.Hour.Equals(startTime.Hour));
            if (slot == null)
            {
                return NotFound();
            }

            _context.Slot.Remove(slot);
            await _context.SaveChangesAsync();

            return slot;
        }

        private bool SlotExists(string roomId, DateTime startTime)
        {
            return _context.Slot.Any(
                sl => sl.RoomID.Equals(roomId.ToUpper()) && sl.StartTime.Date.Equals(startTime.Date)
                                                         && sl.StartTime.Hour.Equals(startTime.Hour));
        }
    }
}
