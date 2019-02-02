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
    public class UsersController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public UsersController(WdtA2ApiContext context)
        {
            _unitOfWork = new UnitOfWork(context);
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUser() => Ok(await _unitOfWork.User.GetAllAsync());

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(string id)
        {
            var user = await _unitOfWork.User.GetAsync(id);

            return user != null ? Ok(user) : (ActionResult)NotFound();
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            if (await _unitOfWork.User.ExistsAsync(u => (u.UserID == user.UserID)))
                return Conflict();

            await _unitOfWork.User.AddAsync(user);
            await _unitOfWork.CompleteAsync();

            return CreatedAtAction("GetUser", new { id = user.UserID }, user);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(string id, User user)
        {
            if (id != user.UserID ||
                !await _unitOfWork.User.ExistsAsync(u => (u.UserID == user.UserID)))
                return NotFound();

            try
            {
                await _unitOfWork.User.AddAsync(user);
                await _unitOfWork.CompleteAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest();
            }
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser(string id)
        {
            var user = await _unitOfWork.User.GetAsync(id);
            if (user == null)
                return NotFound();

            _unitOfWork.User.Remove(user);
            await _unitOfWork.CompleteAsync();

            return user;
        } 
    }
}
