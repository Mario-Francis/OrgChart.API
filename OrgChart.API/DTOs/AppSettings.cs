namespace OrgChart.API.DTOs
{
    public class AppSettings
    {
        public string ManagersGroupId { get; set; }
        public string ManagersGroupMail { get; set; }
        public string SearchFilterSuffix { get; set; }
        public EmailSMTPConfig EmailSMTPConfig { get; set; }

        public bool ReportServiceEnabled { get; set; }
        public bool UpdateAzureAD { get; set; }
        public int ReportServiceExecutionInterval { get; set; } // in minutes
        public string APIKey { get; set; }
    }
}
