namespace CSharpToolkit.Logging {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Mail;
    using Utilities;
    using Abstractions;
    using Utilities.Abstractions;
    public class FileEmailLogger : IEmailLogger {

        Logger _logger = new Logger();

        public OperationResult SendEmail(string subject, string body, EmailReceipients receipients, MailAddress sender, bool isBodyHtml) {
            string email = Use.StringBuilder(builder => {
                EmailLoggingToBegin?.Invoke(this, new Utilities.EventArgs.GenericEventArgs<IFileNameSwappable>(_logger));
                builder.AppendLine("***** EMAIL *****\r\n");
                builder.AppendLine($"FROM : {sender.DisplayName}({sender.Address})\r\n");

                if (receipients.NoReceipientsSpecified) {
                    builder.AppendLine("** No Receipients Specified **");
                }
                else {
                    if (receipients.Receipients.Any()) {
                        builder.AppendLine($"TO : {string.Join("; ", receipients.Receipients.Select(rec => $"{rec.DisplayName}({rec.Address})")) };");
                    }
                    if (receipients.CCReceipients.Any()) {
                        builder.AppendLine($"CC : {string.Join("; ", receipients.CCReceipients.Select(rec => $"{rec.DisplayName}({rec.Address})")) };");
                    }
                    if (receipients.BlindCCReceipients.Any()) {
                        builder.AppendLine($"BCC : {string.Join("; ", receipients.BlindCCReceipients.Select(rec => $"{rec.DisplayName}({rec.Address})")) };");
                    }
                }

                builder.AppendLine($"\r\nSUBJECT : {subject}");

                builder.AppendLine($"Is Body HTML? {(isBodyHtml ? "Yes" : "No" )}");
                builder.AppendLine($"\r\nBODY :\r\n{body}");
                builder.AppendLine("\r\n***** EMAIL END *****");
            });
            OperationResult loggingOperation =
                _logger.Log(email);
            if (loggingOperation.WasSuccessful)
                EmailLoggingCompletedSuccessfully?.Invoke(this, new Utilities.EventArgs.GenericEventArgs<string>(_logger.FileName));
            return loggingOperation;
        }

        public event EventHandler<Utilities.EventArgs.GenericEventArgs<string>> EmailLoggingCompletedSuccessfully;
        public event EventHandler<Utilities.EventArgs.GenericEventArgs<IFileNameSwappable>> EmailLoggingToBegin;
    }
}