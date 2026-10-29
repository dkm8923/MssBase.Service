using MudBlazor;

namespace MssBase.UI.Components.Services
{
    public class ToastService(ISnackbar snackbar)
    {
        public void ShowNormal(string message, bool closeAfterNavigation = true) => Show(message, Severity.Normal, closeAfterNavigation);
        public void ShowInfo(string message, bool closeAfterNavigation = true) => Show(message, Severity.Info, closeAfterNavigation);
        public void ShowSuccess(string message, bool closeAfterNavigation = true) => Show(message, Severity.Success, closeAfterNavigation);
        public void ShowWarning(string message, bool closeAfterNavigation = true) => Show(message, Severity.Warning, closeAfterNavigation);
        public void ShowError(string message, bool closeAfterNavigation = true) => Show(message, Severity.Error, closeAfterNavigation);

        private void Show(string message, Severity severity, bool closeAfterNavigation = true)
        {
            snackbar.Add(message, severity, (options) =>
            {
                options.CloseAfterNavigation = closeAfterNavigation;
            });
        }
    }
}