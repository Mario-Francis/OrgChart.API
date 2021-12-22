using ClosedXML.Excel;
using Microsoft.Extensions.Options;
using OrgChart.API.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OrgChart.API.Services
{
    public class ReportService:IReportService
    {
        private readonly IMicrosoftGraphService microsoftGraphService;
        private readonly IOptionsSnapshot<AppSettings> appSettingsDelegate;
        private readonly IMailService mailService;

        public ReportService(IMicrosoftGraphService microsoftGraphService,
            IOptionsSnapshot<AppSettings> appSettingsDelegate,
            IMailService mailService)
        {
            this.microsoftGraphService = microsoftGraphService;
            this.appSettingsDelegate = appSettingsDelegate;
            this.mailService = mailService;
        }
        // Send report on unclaied employees
        public async Task SendUnclaimedEmployeesReport()
        {
            var unclaimedEmployees = await microsoftGraphService.GetUsersWithoutManagers();
            var mail = new Mail
            {
                Email = appSettingsDelegate.Value.ManagersGroupMail,
                Subject = $"Unclaimed Employees for {DateTime.Now.ToString("MM-dd-yyyy")}",
                Body = $@"
Dear team,

Kindly find attached report for unclaimed employees as at today.

Best regards,
Orgchart team.",
                Attachments = new List<DTOs.File>
                {
                    new DTOs.File
                    {
                        FileName=$"{DateTime.Now.ToString("MM-dd-yyyy")} Unclaimed Employees Report.xlsx",
                        FileBuffer = ExportEmployeesToExcel(unclaimedEmployees)
                    }
                }
            };
            await mailService.SendMail(mail);
        }

        private byte[] ExportEmployeesToExcel(IEnumerable<ADUser> employees)
        {
            // create excel
            var workbook = new XLWorkbook(ClosedXML.Excel.XLEventTracking.Disabled);

            // using data table
            var table = new DataTable("Unclaimed Employees");

            table.Columns.Add("#", typeof(string));
            table.Columns.Add("Employee Name", typeof(string));
            table.Columns.Add("Email", typeof(string));
            table.Columns.Add("Job Title", typeof(string));
            table.Columns.Add("Department", typeof(string));
            table.Columns.Add("Mobile", typeof(string));

            var count = 1;
            foreach (var e in employees)
            {
                var row = table.NewRow();

                row[0] = count.ToString();
                row[1] = e.DisplayName;
                row[2] = e.UserPrincipalName;
                row[3] = e.JobTitle;
                row[4] = e.Department;
                row[5] = e.BusinessPhone ?? e.MobilePhone;

                table.Rows.Add(row);
                count++;
            }
            workbook.AddWorksheet(table);

            byte[] byteFile = null;
            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                byteFile = stream.ToArray();
            }

            return byteFile;
        }

    }
}
