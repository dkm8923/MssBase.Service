using Shared.Logic.Common;
using System.Net;

namespace Service.Logger.Dto
{
    public record InsertLoggerRequest
    {
        public InsertLoggerRequest() 
        {
            this.Application = Constants.ApplicationName;
            this.Type = "Application";
            this.Severity = "ERROR";
            this.UTCEventTime = DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'");
            this.ClientIP = CommonUtilities.GetLocalIpAddress();
            this.Host = Dns.GetHostName();
            this.ClientIP = this.ClientIP != null && this.ClientIP != "" ? this.ClientIP : "0.0.0.0";
            this.Host = this.Host != null && this.Host != "" ? this.Host : "CANNOTGET";
        }

        public string Application { get; }
        public string ApplicationMessage { get; set; }
        public string Type { get; }
        public string Category { get; set; }
        public string ClientIP { get; }
        public string Host { get; }
        public string Severity { get; }
        public string Username { get; set; }
        public string UTCEventTime { get; }
    }

}

