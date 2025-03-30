using AutoMapper;
using Business.Communication;
using Business.Domain.Models;
using Business.Domain.Repositories;
using Business.Domain.Services;
using Business.Resources;
using Business.Resources.Information;
using Business.Resources.Timesheet;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OfficeOpenXml;
using Serilog;
using System;
using System.Xml.Linq;

namespace Business.Services
{
    public class TimesheetService : ResponseMessageService, ITimesheetService
    {
        #region Property
        protected readonly IMapper Mapper;
        protected readonly IUnitOfWork UnitOfWork;
        private readonly HostResource _hostResource;
        private readonly IWebHostEnvironment _env;
        private readonly IPersonRepository _personRepository;
        private readonly ITimesheetRepository _timesheetRepository;

        private Timesheet _element = new();
        private List<Timesheet> _listElement = new();
        private ExcelPackage package = new ExcelPackage();
        #endregion

        #region Constructor
        public TimesheetService(IPersonRepository personRepository,
            ITimesheetRepository timesheetRepository,
            IMapper mapper,
            ExcelPackage package,
            IUnitOfWork unitOfWork,
            IWebHostEnvironment env,
            IOptionsMonitor<HostResource> hostResource,
            IOptionsMonitor<ResponseMessage> responseMessage) : base(responseMessage)

        {
            this.Mapper = mapper;
            this.UnitOfWork = unitOfWork;
            this._hostResource = hostResource.CurrentValue;
            this.package = package;
            this._env = env;
            this._personRepository = personRepository;
            this._timesheetRepository = timesheetRepository;
        }
        #endregion

        #region Method
        public async Task<BaseResponse<TimesheetResource>> ImportAsync(Stream stream)
        {

            await package.LoadAsync(stream);
            // Get the first worksheet in the workbook
            ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

            int? year = worksheet.Cells[1, 2].GetValue<int?>();
            int? month = worksheet.Cells[1, 4].GetValue<int?>();
            if (year == null || month == null)
                return new BaseResponse<TimesheetResource>(ResponseMessage.Values["Timesheet_Invalid"]);

            // Calculate timesheet
            var people = await _personRepository.GetAllAsync();
            string[] timesheet = new string[31];
            var totalRow = worksheet.Dimension.Rows;
            var totalCol = worksheet.Dimension.Columns;
            for (int row = 3; row <= totalRow; row++)
            {
                string staffId = worksheet.Cells[row, 2].GetValue<string>();

                Log.Information("vcl: " + staffId);
                Log.Information("vcl1: " + people);
                var person = GetPersonByStaffId(people, staffId);

                int day = 0;
                for (int tempCol = 4; tempCol <= totalCol; tempCol++)
                {
                    timesheet[day] = worksheet.Cells[row, tempCol].GetValue<string>()?.Trim().ToUpper();

                    ConvertValueCell(timesheet[day]);

                    day++;
                }
                _element.TimesheetJSON = JsonConvert.SerializeObject(timesheet);
                _element.Date = new DateTime((int)year, (int)month, 1);
                _element.Person = person;
                _element.PersonId = person.Id;

                Log.Information("vcl2: " + _element.TimesheetJSON);
                Log.Information("vcl3: " + _element.Date);
                Log.Information("vcl4: " + _element.Person);
                Log.Information("vcl5: " + _element.PersonId);
                Log.Information("vcl6: " + _element);
                _listElement.Add(_element);
                _element = new();

            }
            foreach (var item in _listElement)
            {
                Log.Information("vcl7: " + item.TimesheetJSON);
                Log.Information("vcl7: " + item.Date);

                Log.Information("vcl7: " + item.Person);
                Log.Information("vcl7: " + item.PersonId);
            }

            _timesheetRepository.AttachRange(_listElement);
            await UnitOfWork.CompleteAsync();

            return new BaseResponse<TimesheetResource>(true);
        }

        public async Task<byte[]> ExportAsync(int year, int month)
        {
            var timesheets = await _timesheetRepository.GetByMonthYearAsync(year, month);

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Timesheet");

            // Header row: năm/tháng
            worksheet.Cells[1, 1].Value = "Năm:";
            worksheet.Cells[1, 2].Value = year;
            worksheet.Cells[1, 3].Value = "Tháng:";
            worksheet.Cells[1, 4].Value = month;

            // Table header
            worksheet.Cells[2, 1].Value = "STT";
            worksheet.Cells[2, 2].Value = "Mã nhân viên";
            worksheet.Cells[2, 3].Value = "Họ và tên"; // Thêm cột họ tên

            for (int day = 1; day <= 31; day++)
            {
                worksheet.Cells[2, day + 3].Value = $"Ngày {day}";
            }

            if (!timesheets.Any())
            {
                return await package.GetAsByteArrayAsync();
            }

            int row = 3;
            int stt = 1;

            foreach (var timesheet in timesheets)
            {
                worksheet.Cells[row, 1].Value = stt++;
                worksheet.Cells[row, 2].Value = timesheet.Person?.StaffId;
                worksheet.Cells[row, 3].Value = $"{timesheet.Person?.FirstName} {timesheet.Person?.LastName}";

                var days = JsonConvert.DeserializeObject<string[]>(timesheet.TimesheetJSON ?? "[]");
                for (int day = 0; day < days.Length && day < 31; day++)
                {
                    worksheet.Cells[row, day + 4].Value = days[day]; // đẩy sang phải 1 cột
                }

                row++;
            }

            return await package.GetAsByteArrayAsync();
        }





        public async Task<BaseResponse<TimesheetResource>> GetTimesheetByPersonIdAsync(int personId, DateTime date)
        {
            var timesheet = await _timesheetRepository.GetTimesheetByPersonIdAsync(personId, date);

            if (timesheet is null)
                return new BaseResponse<TimesheetResource>(ResponseMessage.Values["NoData"]);

            // Mapping
            var resource = Mapper.Map<Timesheet, TimesheetResource>(timesheet);

            return new BaseResponse<TimesheetResource>(resource);
        }

        #region Private work
        private string GetRootPath(string timesheetFileName)
        {
            string timesheetPath = string.Format($"{_hostResource.TimesheetPath}{timesheetFileName}");
            string rootPath = string.Concat(_env.WebRootPath, timesheetPath);

            return rootPath;
        }

        private Person GetPersonByStaffId(IEnumerable<Person> people, string staffId)
        {
            string tempStaffId = staffId.Trim();

            foreach (var person in people)
                if (person.StaffId.Equals(tempStaffId)) return person;

            return null;
        }

        private void ConvertValueCell(string valueCell)
        {
            if (string.IsNullOrEmpty(valueCell)) return;

            var cleanValue = valueCell.Trim().ToUpper();

            if (cleanValue.Equals("-") || cleanValue.Equals("O"))
                return;

            if (cleanValue.Equals("W"))
                _element.WorkDay += 1f;

            if (cleanValue.Equals("R"))
                _element.WorkDay += 0.5f;

            if (cleanValue.Equals("A"))
                _element.Absent += 1f;

            if (cleanValue.Equals("H"))
                _element.Holiday += 1f;

            if (cleanValue.Equals("S"))
                _element.UnpaidLeave += 1f;

            if (cleanValue.Equals("V"))
                _element.PaidLeave += 1f;

            _element.TotalWorkDay += 1;
        }
        #endregion

        #endregion
    }

}
