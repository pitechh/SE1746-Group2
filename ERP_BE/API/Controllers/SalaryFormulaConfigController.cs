using Business.Domain.Services;
using Business.Resources.SalaryFormulaConfig;
using Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/v1/salary-formula")]
    public class SalaryFormulaConfigController : ControllerBase
    {
        private readonly ISalaryFormulaConfigService _formulaService;

        public SalaryFormulaConfigController(ISalaryFormulaConfigService formulaService)
        {
            _formulaService = formulaService;
        }

        /// <summary>
        /// Lấy công thức đang active theo loại
        /// </summary>
        [HttpGet("active")]
        public async Task<IActionResult> GetActive([FromQuery] string type)
        {
            var result = await _formulaService.GetActiveByTypeAsync(type);
            if (result == null)
                return NotFound("Không có công thức phù hợp");
            return Ok(result);
        }

        /// <summary>
        /// Lấy toàn bộ công thức theo loại
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string type)
        {
            var result = await _formulaService.GetAllByTypeAsync(type);
            return Ok(result);
        }

        /// <summary>
        /// Thêm công thức mới
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SalaryFormulaConfigCreateResource resource)
        {
            var success = await _formulaService.CreateAsync(resource);
            if (!success)
                return BadRequest("Không thể thêm công thức");
            return Ok("Tạo công thức thành công");
        }

        /// <summary>
        /// Đặt công thức là active
        /// </summary>
        [HttpPut("set-active/{id}")]
        public async Task<IActionResult> SetActive(int id, [FromQuery] string type)
        {
            var result = await _formulaService.SetActiveAsync(id, type);
            return Ok(new { success = result });
        }

    }
}
