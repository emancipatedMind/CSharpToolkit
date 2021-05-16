namespace CSharpToolkit.Logging {
    using System;
    using System.Net.Mail;
    using System.Threading.Tasks;
    using Abstractions;
    using Utilities;

    public class NullEmailLogger : IEmailLoggerAsync {

        static NullEmailLogger _instance;

        public static NullEmailLogger Instance =
            _instance ?? (_instance = new NullEmailLogger());

        public Task<OperationResult> SendEmailAsync(EmailLoggerParameter model) =>
            Task.FromResult(new OperationResult()); 

        public Task<OperationResult> SendEmailAsync(string subject, string body, EmailReceipients receipients, MailAddress sender, bool isBodyHtml) =>
            Task.FromResult(new OperationResult()); 
    }
}
