using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using Microsoft.Maui.ApplicationModel;

namespace MusicAlbum_Explorer.Views
{
    public partial class YouTubePlayerPage : ContentPage, IQueryAttributable
    {
        private string _originalUrl;

        public YouTubePlayerPage()
        {
            InitializeComponent();

            // Wire WebView navigation events
            PlayerWebView.Navigating += OnWebViewNavigating;
            PlayerWebView.Navigated += OnWebViewNavigated;

            OpenExternalButton.Clicked += OnOpenExternalClicked;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query != null && query.TryGetValue("url", out var obj) && obj is string url && !string.IsNullOrWhiteSpace(url))
            {
                _originalUrl = url;

                var videoId = ExtractVideoId(url);
                if (!string.IsNullOrEmpty(videoId))
                {
                    // Lazy-load iframe on user interaction to avoid player configuration errors
                    var thumb = $"https://img.youtube.com/vi/{videoId}/hqdefault.jpg";
                    var embedHost = "https://www.youtube-nocookie.com"; // use nocookie domain for fewer restrictions

                    var html = $@"<!doctype html>
<html>
<head>
<meta name='viewport' content='initial-scale=1.0' />
<style>
html,body,#player {{ height:100%; width:100%; margin:0; padding:0; background:black; }}
#player {{ position:relative; display:flex; align-items:center; justify-content:center; height:100%; width:100%; }}
#player img {{ width:100%; height:100%; object-fit:cover; display:block; }}
#playBtn {{ position:absolute; z-index:2; width:96px; height:96px; border-radius:48px; border:none; background:rgba(0,0,0,0.6); display:flex; align-items:center; justify-content:center; cursor:pointer; }}
#playIcon {{ width:0; height:0; border-left:28px solid white; border-top:16px solid transparent; border-bottom:16px solid transparent; margin-left:6px; }}
</style>
</head>
<body>
<div id='player'>
  <img id='thumb' src='{thumb}' alt='thumbnail' />
  <div id='playBtn' onclick='play()'><div id='playIcon'></div></div>
</div>
<script>
// Report unhandled errors and console.error back to the native app via a navigation to a custom scheme
window.onerror = function(message, source, lineno, colno, error) {{
    try {{
        var info = message + ' @ ' + source + ':' + lineno + ':' + colno + (error && error.stack ? '\\n' + error.stack : '');
        window.location.href = 'app://webviewerror?info=' + encodeURIComponent(info);
    }} catch(e) {{}}
}};
(function() {{
    var orig = console.error;
    console.error = function() {{
        try {{
            var args = Array.prototype.slice.call(arguments).map(function(a) {{ try {{ return JSON.stringify(a); }} catch(e) {{ return String(a); }} }}).join(' ');
            window.location.href = 'app://webviewerror?info=' + encodeURIComponent('console.error: ' + args);
        }} catch(e) {{}}
        if (orig) orig.apply(console, arguments);
    }};
}})();

function play() {{
    var iframe = document.createElement('iframe');
    iframe.style.width = '100%';
    iframe.style.height = '100%';
    iframe.setAttribute('frameborder', '0');
    iframe.setAttribute('allow', 'accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture');
    iframe.setAttribute('allowfullscreen', '');
    // When creating the iframe we also add an onload handler to try to detect load failures
    iframe.onload = function() {{
        try {{
            // notify native that iframe loaded
            window.location.href = 'app://webviewevent?info=' + encodeURIComponent('iframe_loaded');
        }} catch(e) {{}}
    }};
    iframe.src = '{embedHost}/embed/{videoId}?rel=0&playsinline=1';
    var p = document.getElementById('player');
    p.innerHTML = '';
    p.appendChild(iframe);
}}
</script>
</body>
</html>";

                    PlayerWebView.Source = new HtmlWebViewSource { Html = html };
                    return;
                }

                // fallback: if we couldn't extract ID, try opening the original URL
                try
                {
                    PlayerWebView.Source = url;
                }
                catch
                {
                    // ignore - WebView may not support the URL, nothing else to do here
                }
            }
        }

        private async void OnWebViewNavigating(object sender, WebNavigatingEventArgs e)
        {
            // Intercept messages from the web content using the custom scheme
            if (!string.IsNullOrEmpty(e.Url) && e.Url.StartsWith("app://webviewerror", StringComparison.OrdinalIgnoreCase))
            {
                e.Cancel = true;
                var idx = e.Url.IndexOf("?info=");
                var info = idx >= 0 ? Uri.UnescapeDataString(e.Url.Substring(idx + 6)) : "";
                await DisplayAlert("WebView error", info, "OK");
                LoadingIndicator.IsRunning = false;
                LoadingIndicator.IsVisible = false;
                FallbackPanel.IsVisible = true;
                return;
            }

            if (!string.IsNullOrEmpty(e.Url) && e.Url.StartsWith("app://webviewevent", StringComparison.OrdinalIgnoreCase))
            {
                e.Cancel = true;
                var idx = e.Url.IndexOf("?info=");
                var info = idx >= 0 ? Uri.UnescapeDataString(e.Url.Substring(idx + 6)) : "";
                // For now we just log or show small notification for events like iframe_loaded
                System.Diagnostics.Debug.WriteLine("WebView event: " + info);
                return;
            }

            LoadingIndicator.IsRunning = true;
            LoadingIndicator.IsVisible = true;
            FallbackPanel.IsVisible = false;
        }

        private void OnWebViewNavigated(object sender, WebNavigatedEventArgs e)
        {
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;

            if (e.Result != WebNavigationResult.Success)
            {
                // show fallback to open externally
                FallbackPanel.IsVisible = true;
            }
            else
            {
                FallbackPanel.IsVisible = false;
            }
        }

        private async void OnOpenExternalClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_originalUrl))
            {
                try
                {
                    await Launcher.OpenAsync(_originalUrl);
                }
                catch
                {
                    await DisplayAlert("Erreur", "Impossible d'ouvrir la vidéo en externe.", "OK");
                }
            }
        }

        private string ExtractVideoId(string url)
        {
            try
            {
                var uri = new Uri(url);

                // short youtu.be links
                if (uri.Host.IndexOf("youtu.be", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return uri.AbsolutePath.Trim('/');
                }

                // query string parsing for v parameter
                var q = uri.Query;
                if (!string.IsNullOrEmpty(q))
                {
                    var query = q.TrimStart('?');
                    var pairs = query.Split('&', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var p in pairs)
                    {
                        var kv = p.Split('=', 2);
                        if (kv.Length == 2 && kv[0] == "v")
                        {
                            return Uri.UnescapeDataString(kv[1]);
                        }
                    }
                }

                // path like /embed/VIDEOID
                var segments = uri.Segments;
                for (int i = 0; i < segments.Length - 1; i++)
                {
                    if (segments[i].Trim('/').Equals("embed", StringComparison.OrdinalIgnoreCase))
                    {
                        return segments[i + 1].Trim('/');
                    }
                }

                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}