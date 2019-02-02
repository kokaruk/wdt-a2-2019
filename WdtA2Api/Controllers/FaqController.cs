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
    public class FaqController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public FaqController(WdtA2ApiContext context)
        {
            _unitOfWork = new UnitOfWork(context);
        }

        // GET: api/Faq
        [HttpGet]
        public async Task<IEnumerable<Faq>> GetAll()
        {
            return await _unitOfWork.Faq.GetAllAsync();
        }

    }
}
