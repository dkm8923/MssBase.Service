using Dto.Security.Permission;

namespace MssBase.UI.Components.Services
{
    public class PermissionStateService
    {
        public PermissionDto? CurrentPermission { get; set; }

        public void SetPermissionForEdit(PermissionDto permission)
        {
            CurrentPermission = permission;
        }

        public void ClearCurrentPermission()
        {
            CurrentPermission = null;
        }
    }
}