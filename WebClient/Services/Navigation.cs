using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace WebClient.Services;

public class Navigation : IDisposable
{
    private readonly NavigationManager _navigationManager;
    private readonly List<string> _history;

    public Navigation(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
        _history = new List<string>()
        {
            _navigationManager.Uri
        };
        _navigationManager.LocationChanged += OnLocationChanged;
    }

    public void NavigateTo(string url) => NavigateTo(url, false);

    public void NavigateTo(string url, bool replaceThis)
    {
        if (replaceThis && CanNavigateBack)
        {
            _history.RemoveAt(_history.Count - 1);
        }
        _navigationManager.NavigateTo(url);
    }

    public bool CanNavigateBack => _history.Count >= 2;

    public void NavigateBack()
    {
        if (!CanNavigateBack) return;
        var backPageUrl = _history[^2];
        _history.RemoveRange(_history.Count - 2, 2);
        _navigationManager.NavigateTo(backPageUrl);
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        _history.Add(e.Location);
    }

    public void Dispose()
    {
        _navigationManager.LocationChanged -= OnLocationChanged;
        GC.SuppressFinalize(this);
    }
}
