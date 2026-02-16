namespace IntegrationTests.Shared
{
    public static class ApiEndPoints
    {
        public class Common 
        {
            public class Unit
            {
                public const string Base = "/api/Common/Unit";
            }

            public class UnitDefinition
            {
                public const string Base = "/api/Common/UnitDefinition";
            }
        }

        public class Security 
        {
            public class Application
            {
                public const string Base = "/api/Security/Application";
            }

            public class ApplicationUser
            {
                public const string Base = "/api/Security/ApplicationUser";
            }
        }
    }
}
