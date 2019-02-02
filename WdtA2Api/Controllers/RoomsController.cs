using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using WdtA2Api.Data;

using WdtModels.ApiModels;

namespace WdtA2Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public RoomsController(WdtA2ApiContext context)
        {
            _unitOfWork = new UnitOfWork(context);
        }

        // GET: api/Rooms
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Room>>> GetRoom() => Ok(await _unitOfWork.Room.GetAllAsync());

        // GET: api/Rooms/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Room>> GetRoom(string id)
        {
            var room = await _unitOfWork.Room.GetAsync(id);
            return room != null ? Ok(room) : (ActionResult)NotFound();
        }

        // POST: api/Rooms
        [HttpPost]
        public async Task<ActionResult<Room>> PostRoom(Room room)
        {
            if (await _unitOfWork.Room.ExistsAsync(r => (r.RoomID == room.RoomID)))
                return Conflict();

            if (room.RoomID.Length > 2)
                return BadRequest();

            await _unitOfWork.Room.AddAsync(room);
            await _unitOfWork.CompleteAsync();

            return CreatedAtAction("GetRoom", new { id = room.RoomID }, room);
        }

        // PUT: api/Rooms/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRoom(string id, Room room)
        {
            if (id != room.RoomID ||
                !await _unitOfWork.Room.ExistsAsync(r => (r.RoomID == room.RoomID)))
                return NotFound();

            try
            {
                await _unitOfWork.Room.AddAsync(room);
                await _unitOfWork.CompleteAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest();
            }
        }

        // DELETE: api/Rooms/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Room>> DeleteRoom(string id)
        {
            var room = await _unitOfWork.Room.GetAsync(id);
            if (room == null)
                return NotFound();

            _unitOfWork.Room.Remove(room);
            await _unitOfWork.CompleteAsync();

            return room;
        }
    }
}
