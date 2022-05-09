using System;
using System.Collections.Generic;
using Discord;
using Serilog.Events;
using Serilog.Parsing;

namespace LostArk.Discord.Bot.Infrastructure
{
    public static class LogExtension
    {
        public static LogEvent ToLogEvent(this LogMessage msg)
        {
            return new(DateTimeOffset.Now, msg.Severity.Map(), msg.Exception,
                new MessageTemplate(msg.Message, new MessageTemplateToken[1]
                {
                    new TextToken(msg.Message)
                }), new List<LogEventProperty>
                {
                    new("SourceContext", new ScalarValue(msg.Source))
                });
        }

        private static LogEventLevel Map(this LogSeverity severity)
        {
            return severity switch
            {
                LogSeverity.Critical => LogEventLevel.Fatal,
                LogSeverity.Error => LogEventLevel.Error,
                LogSeverity.Warning => LogEventLevel.Warning,
                LogSeverity.Info => LogEventLevel.Information,
                LogSeverity.Verbose => LogEventLevel.Verbose,
                LogSeverity.Debug => LogEventLevel.Debug,
                _ => throw new ArgumentOutOfRangeException(nameof(severity), severity, null)
            };
        }
    }
}