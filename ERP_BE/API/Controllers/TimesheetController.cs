using Business.Communication;
using Business.Domain.Services;
using Business.Resources;
using Business.Resources.Timesheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Serilog;

namespace API.Controllers
{
    [ApiController]
    [Route("api/v1/timesheet")]
    public class TimesheetController : ControllerBase
    {
        #region Property

        private readonly ITimesheetService _timesheetService;
        protected readonly ResponseMessage ResponseMessage;

        #endregion

        #region Constructor

        public TimesheetController(ITimesheetService timesheetService,
            IOptionsMonitor<ResponseMessage> responseMessage)
        {
            this._timesheetService = timesheetService;
            this.ResponseMessage = responseMessage.CurrentValue;
        }

        #endregion

        #region Action

        [HttpPost("import")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ImportAsync([FromForm] IFormFile file)
        {
            var validateResult = ValidateTimesheet(file);
            if (!validateResult.isSuccess)
                return BadRequest(validateResult.result);

            var filePath = Path.GetTempFileName();

            try
            {
                await using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);
                stream.Position = 0;

                var result = await _timesheetService.ImportAsync(stream);

                return result.Success ? Ok(result) : BadRequest(result);
            }
            finally
            {
                // Đảm bảo luôn xóa file tạm, kể cả khi xảy ra exception
                if (System.IO.File.Exists(filePath))
                {
                    try
                    {
                        System.IO.File.Delete(filePath);
                    }
                    catch (Exception ex)
                    {
                        // Ghi log nếu cần
                        Console.WriteLine($"Không thể xóa file tạm: {ex.Message}");
                    }
                }
            }
        }


        [HttpGet]
        // [Authorize(Roles = $"{Role.Admin}, {Role.EditorQTNS}, {Role.EditorKT}")]
        [ProducesResponseType(typeof(BaseResponse<TimesheetResource>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<TimesheetResource>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetTimesheetByPersonIdAsync([FromQuery] int personId, [FromQuery] DateTime date)
        {
            Log.Information($"{User.Identity?.Name}: get a timesheet by person-id-{personId} on date {date}.");

            var result = await _timesheetService.GetTimesheetByPersonIdAsync(personId, date);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("export")]
        // [Authorize(Roles = $"{Role.Admin}, {Role.EditorQTNS}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ExportAsync([FromQuery] int year, [FromQuery] int month)
        {
            var content = await _timesheetService.ExportAsync(year, month);
            if (content == null || content.Length == 0)
                return BadRequest("Không có dữ liệu chấm công cho tháng/năm yêu cầu.");

            var fileName = $"Timesheet_{year}_{month}.xlsx";
            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        #endregion

        #region Private work

        private (bool isSuccess, BaseResponse<object> result) ValidateTimesheet(IFormFile file)
        {
            if (file == null || file.Length <= 0)
                return (false, new BaseResponse<object>(ResponseMessage.Values["File_Empty"]));

            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                return (false, new BaseResponse<object>(ResponseMessage.Values["Not_Support_File"]));

            return (true, new BaseResponse<object>(true));
        }

        #endregion
    }
}
