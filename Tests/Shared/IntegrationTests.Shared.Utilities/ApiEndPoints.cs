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

            public class ApplicationUserPermission
            {
                public const string Base = "/api/Security/ApplicationUserPermission";
            }
            
            public class ApplicationUserRole
            {
                public const string Base = "/api/Security/ApplicationUserRole";
            }

            public class Role
            {
                public const string Base = "/api/Security/Role";
            }

            public class Permission
            {
                public const string Base = "/api/Security/Permission";
            }

            public class RolePermission
            {
                public const string Base = "/api/Security/RolePermission";
            }
        }
    }
}
