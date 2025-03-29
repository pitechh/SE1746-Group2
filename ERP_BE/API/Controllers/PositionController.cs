using Business.Communication;
using Business.Domain.Services;
using Business.Resources.Position;
using Business.Resources.Timesheet;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("api/v1/position")] // Cập nhật route theo yêu cầu
    public class PositionController : ControllerBase
    {
        private readonly IPositionService _positionService;

        public PositionController(IPositionService positionService)
        {
            _positionService = positionService;
        }

        /// <summary>
        /// Lấy danh sách tất cả vị trí (Position)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<PositionResource>>> GetAllPositions()
        {
            var positions = await _positionService.GetAllPositionsAsync();
            
            return Ok(new BaseResponse<List<PositionResource>>(positions));
        }
    }
}