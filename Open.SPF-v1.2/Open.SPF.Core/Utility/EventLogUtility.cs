using System;
using System.Configuration;
using System.Diagnostics;
using System.Text;

namespace Open.SPF.Utility
{
    public class EventLogUtility
    {
        private const string __eventLogAppSettingKey = "ApplicationEventLogSource";
        public const string DefaultEventLogSource = "Simple Permission Framework";
        private const int __eventLogId = 1001;

        private static string _eventLogSource = null;
        public static string EventLogSource
        {
            get
            {
                if (String.IsNullOrEmpty(_eventLogSource))
                {
                    // Use custom SPF setting for event log first
                    _eventLogSource = Open.SPF.Core.Properties.Settings.Default.ApplicationEventLogSource;

                    // Then use general AppSettings value that is available to all apps
                    if (String.IsNullOrEmpty(_eventLogSource))
                        _eventLogSource = ConfigurationManager.AppSettings[__eventLogAppSettingKey];

                    // Then use a default value
                    if (String.IsNullOrEmpty(_eventLogSource))
                        _eventLogSource = DefaultEventLogSource;
                }

                return _eventLogSource;
            }
        }

        public static void LogException(System.Exception ex)
        {
            if (ex == null) return;
            LogErrorMessage(FormatExceptionMessage(ex));
        }

        public static void LogInformationMessage(string message)
        {
            if (String.IsNullOrEmpty(message)) return;
            System.Diagnostics.EventLog.WriteEntry(EventLogSource, message, EventLogEntryType.Information, __eventLogId);
        }

        public static void LogWarningMessage(string message)
        {
            if (String.IsNullOrEmpty(message)) return;
            System.Diagnostics.EventLog.WriteEntry(EventLogSource, message, EventLogEntryType.Warning, __eventLogId);
        }

        public static void LogErrorMessage(string message)
        {
            if (String.IsNullOrEmpty(message)) return;
            System.Diagnostics.EventLog.WriteEntry(EventLogSource, message, EventLogEntryType.Error, __eventLogId);
        }

        public static void LogDiagnosticMessage(string conciseMessage, string verboseMessage)
        {
            if ((DiagnosticLoggingLevel == DiagnosticLoggingOption.Concise) && (!String.IsNullOrEmpty(conciseMessage)))
            {
                System.Diagnostics.EventLog.WriteEntry(EventLogSource, conciseMessage, EventLogEntryType.Information, __eventLogId);
            }
            else if ((DiagnosticLoggingLevel == DiagnosticLoggingOption.Verbose) && (!String.IsNullOrEmpty(verboseMessage)))
            {
                System.Diagnostics.EventLog.WriteEntry(EventLogSource, verboseMessage, EventLogEntryType.Information, __eventLogId);
            }
        }

        public static void LogDiagnosticMessage(string message, DiagnosticLoggingOption minimumLoggingLevel)
        {
            if (String.IsNullOrEmpty(message)) return;
            if ((minimumLoggingLevel == DiagnosticLoggingLevel) || ((minimumLoggingLevel == DiagnosticLoggingOption.Concise) && (DiagnosticLoggingLevel == DiagnosticLoggingOption.Verbose)))
            {
                System.Diagnostics.EventLog.WriteEntry(EventLogSource, message, EventLogEntryType.Information, __eventLogId);
            }
        }

        public static string FormatExceptionMessage(System.Exception ex)
        {
            StringBuilder sbExceptionMessage = new StringBuilder();

            sbExceptionMessage.Append(String.Format("An Exception (of Type {0}) has been raised (in component {1}):\r\n\r\n", ex.GetType().FullName, DefaultEventLogSource));
            sbExceptionMessage.Append("");
            sbExceptionMessage.Append(ex.Message);
            sbExceptionMessage.Append("\r\n");
            sbExceptionMessage.Append(ex.Source);
            sbExceptionMessage.Append("\r\n");
            sbExceptionMessage.Append(ex.StackTrace);

            FormatInnerExceptionMessage(ex.InnerException, sbExceptionMessage);

            return sbExceptionMessage.ToString();
        }

        private static void FormatInnerExceptionMessage(System.Exception ex, StringBuilder sbExceptionMessage)
        {
            if (ex == null)
                return;

            sbExceptionMessage.Append("\r\n\r\nAdditional Exception details:\r\n\r\n");
            sbExceptionMessage.Append(ex.Message);
            sbExceptionMessage.Append("\r\n");
            sbExceptionMessage.Append(ex.Source);
            sbExceptionMessage.Append("\r\n");
            sbExceptionMessage.Append(ex.StackTrace);

            FormatInnerExceptionMessage(ex.InnerException, sbExceptionMessage);
        }

        private static DiagnosticLoggingOption? _diagnosticLoggingLevel = null;
        public static DiagnosticLoggingOption DiagnosticLoggingLevel
        {
            get
            {
                if (!_diagnosticLoggingLevel.HasValue)
                {
                    _diagnosticLoggingLevel = DiagnosticLoggingOption.Disabled;
                    try
                    {
                        string logDiagnosticString = Open.SPF.Core.Properties.Settings.Default.LogDiagnostics;
                        if (!String.IsNullOrEmpty(logDiagnosticString))
                            _diagnosticLoggingLevel = (DiagnosticLoggingOption)System.Enum.Parse(typeof(DiagnosticLoggingOption), logDiagnosticString, true);
                    }
                    catch (Exception) { } // ignore errors
                }

                return _diagnosticLoggingLevel.Value;
            }
        }
    }
}
