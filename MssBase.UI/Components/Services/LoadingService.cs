namespace MssBase.UI.Components.Services
{
    public class LoadingService
    {
        private bool _isLoading = false;

        public event Action<bool>? OnLoadingStateChanged;

        public bool IsLoading => _isLoading;

        public Task Show()
        {
            _isLoading = true;
            NotifyStateChanged();
            return Task.CompletedTask;
        }

        public Task Hide()
        {
            _isLoading = false;
            NotifyStateChanged();
            return Task.CompletedTask;
        }

        private void NotifyStateChanged()
        {
            OnLoadingStateChanged?.Invoke(_isLoading);
        }
    }
}