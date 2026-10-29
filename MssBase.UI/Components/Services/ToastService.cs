using MudBlazor;

namespace MssBase.UI.Components.Services
{
    public class ToastService(ISnackbar snackbar)
    {
        public void ShowNormal(string message) => Show(message, Severity.Normal);
        public void ShowInfo(string message) => Show(message, Severity.Info);
        public void ShowSuccess(string message) => Show(message, Severity.Success);
        public void ShowWarning(string message) => Show(message, Severity.Warning);
        public void ShowError(string message) => Show(message, Severity.Error);

        private void Show(string message, Severity severity) =>
            snackbar.Add(message, severity);
    }
}