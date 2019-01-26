using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using WdtA2Api.Data;
using WdtModels.ApiModels;

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

        // DELETE: api/Slots/5/2019-01-15T13:00
        [HttpDelete("{RoomID}/{StartTime}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Slot>> DeleteSlot(string roomId, DateTime startTime)
        {
            if (!SlotExists(roomId, startTime))
            {
                return BadRequest($"No slot fount for room: \"{roomId.ToUpper()}\" at {startTime:d-MM-yyy h tt}");
            }

            var slot = await _context.Slot.FirstOrDefaultAsync(
                           sl => sl.RoomID.Equals(roomId.ToUpper()) && sl.StartTime.Date.Equals(startTime.Date)
                                                                    && sl.StartTime.Hour.Equals(startTime.Hour));
            if (slot == null)
            {
                return NotFound($"No slot fount for room: \"{roomId.ToUpper()}\" at {startTime:d-MM-yyy h tt}");
            }

            if (slot.Student != null)
            {
                return BadRequest($"slot has assigned student, can't delete");
            }

            _context.Slot.Remove(slot);
            await _context.SaveChangesAsync();

            return slot;
        }

        // GET: api/Slots
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Slot>>> GetSlot()
        {
            return await _context.Slot.ToListAsync();
        }

        // GET: api/Slots/5/2019-01-15T13:00
        [HttpGet("{RoomID}/{StartTime}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Slot>> GetSlot(string roomId, DateTime startTime)
        {
            var slot = await _context.Slot.FirstOrDefaultAsync(
                           sl => sl.RoomID.Equals(roomId.ToUpper()) && sl.StartTime.Date.Equals(startTime.Date)
                                                                    && sl.StartTime.Hour.Equals(startTime.Hour));

            if (slot == null)
            {
                return NotFound($"No slot fount for room: \"{roomId.ToUpper()}\" at {startTime:d-MM-yyy h tt}");
            }

            return slot;
        }

        // POST: api/Slots
        [HttpPost]
        public async Task<ActionResult<Slot>> PostSlot(Slot slot)
        {
            if (SlotExists(slot.RoomID, slot.StartTime))
            {
                return BadRequest("slot already exists");
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
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

        private bool SlotExists(string roomId, DateTime startTime)
        {
            return _context.Slot.Any(
                sl => sl.RoomID.Equals(roomId.ToUpper()) && sl.StartTime.Date.Equals(startTime.Date)
                                                         && sl.StartTime.Hour.Equals(startTime.Hour));
        }
    }
}
