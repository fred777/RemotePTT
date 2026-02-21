using Microsoft.Extensions.Logging;

namespace RemotePTT.GUI
{
    // A simple ILogger implementation that writes formatted log messages into a WinForms RichTextBox.
    internal class RichTextBoxLogger : ILogger
    {
        private readonly RichTextBox _rtb;
        private readonly string _category;

        public RichTextBoxLogger(RichTextBox rtb, string category)
        {
            _rtb = rtb ?? throw new ArgumentNullException(nameof(rtb));
            _category = category;
        }

        public IDisposable BeginScope<TState>(TState state) => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            var message = formatter != null ? formatter(state, exception) : state?.ToString();
            var text = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}";
            if (exception != null)
                text += "\n" + exception;

            var color = logLevel switch
            { 
                LogLevel.Critical => Color.Red,
                LogLevel.Error => Color.Red,
                LogLevel.Warning => Color.Blue,
                _ => Color.Black
            };

            // Ensure we append on UI thread
            if (_rtb.IsDisposed) return;
            try
            {
                if (_rtb.InvokeRequired)
                {
                    _rtb.BeginInvoke((Action)(() => AppendText(text, color)));
                }
                else
                {
                    AppendText(text, color);
                }
            }
            catch
            {
                // Swallow any exceptions during logging to avoid crashes from logger itself.
            }
        }

        private void AppendText(string text, Color color)
        {
            if (_rtb.IsDisposed) return;
            _rtb.AppendText(text + Environment.NewLine, color);
            _rtb.SelectionStart = _rtb.TextLength;
            _rtb.ScrollToCaret();
        }

        private class NullScope : IDisposable
        {
            public static NullScope Instance { get; } = new NullScope();
            public void Dispose() { }
        }
    }

    internal class RichTextBoxLoggerProvider : ILoggerProvider
    {
        private readonly RichTextBox _rtb;

        public RichTextBoxLoggerProvider(RichTextBox rtb)
        {
            _rtb = rtb ?? throw new ArgumentNullException(nameof(rtb));
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new RichTextBoxLogger(_rtb, categoryName);
        }

        public void Dispose()
        {
            // Nothing to dispose explicitly; the provider doesn't own the RichTextBox.
        }
    }
}