using Microsoft.AspNetCore.Components;
using MssBase.UI.Components.Services;

namespace MssBase.UI.Components.Shared.LoadingOverlay
{
    public partial class LoadingOverlay : ComponentBase, IAsyncDisposable
    {
        [Inject]
        public required LoadingService _loadingService { get; set; }

        private bool _isLoading = false;

        protected override void OnInitialized()
        {
            _loadingService.OnLoadingStateChanged += HandleLoadingStateChanged;
        }

        private void HandleLoadingStateChanged(bool isLoading)
        {
            _isLoading = isLoading;
            InvokeAsync(StateHasChanged);
        }

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            _loadingService.OnLoadingStateChanged -= HandleLoadingStateChanged;
            await ValueTask.CompletedTask;
        }
    }
}