namespace MssBase.Service.Shared
{
    public static class SwaggerUiDocumentation
    {
        public class QueryStringParms
        {
            public const string QueryStringParmTitle = "<b>Query String Parameters:</b> <br><br>";
            public const string DeleteCache = "<b>deleteCache:</b> Deletes the cached value for the request if applicable and returns fresh response from database. <br>";
            public const string IncludeInactive = "<b>includeInactive:</b> Response only returns 'Active' records by default. (IE: Active Flag = true.) IncludeInactive = true returns all records. <br>";
            public const string IncludeRelated = "<b>includeRelated:</b> Response includes related child data when IncludeRelated = true. <br>";
        }
        public class DefaultResponseMessage
        {
            public const string GetAllRecords = "The records were successfully retrieved.";
            public const string GetRecordById = "The record was successfully retrieved for id in context.";
            public const string FilterRecords = "The record(s) were successfully retrieved for filter parameters in context.";
            public const string InsertRecord = "The record was successfully created.";
            public const string UpdateRecord = "The record was successfully updated.";
            public const string DeleteRecord = "The record was successfully deleted.";
            public const string InternalServerError = "Internal Server Error Occurred.";
        }
    }
}
