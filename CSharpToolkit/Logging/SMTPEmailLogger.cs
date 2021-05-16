namespace CSharpToolkit.Logging {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Mail;
    using Utilities;
    using Abstractions;
    /// <summary>
    /// An email logger that uses Simple Mail Transfer Protocol to send emails.
    /// </summary>
    public class SMTPEmailLogger : IEmailLogger {

        SmtpClient _smtpClient;

        /// <summary>
        /// Initializes <see cref="SMTPEmailLogger"/> with the Smtp host, port, and credentials.
        /// </summary>
        /// <param name="host">The host used to send email.</param>
        /// <param name="port">The port to use.</param>
        /// <param name="username">The user for the credentials.</param>
        /// <param name="password">The password for the credentials.</param>
        public SMTPEmailLogger(string host, int port, string username = "", string password = "") {
            _smtpClient = new SmtpClient(host, port) { Credentials = new NetworkCredential(username, password) };
        }

        /// <summary>
        /// Send an email.
        /// </summary>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="body">The body of the email.</param>
        /// <param name="receipients">The receipients of the email.</param>
        /// <param name="sender">The sender of the email.</param>
        /// <param name="isBodyHtml">Whether the body is html or not.</param>
        /// <returns>Whether send performed successfully.</returns>
        public OperationResult SendEmail(string subject, string body, EmailReceipients receipients, MailAddress sender, bool isBodyHtml) {
            OperationResult<MailMessage> mailMessageOperation =
                Get.MailMessage(subject, body, receipients, sender, isBodyHtml);

            if (mailMessageOperation.HadErrors)
                return mailMessageOperation;

            try {
                _smtpClient.Send(mailMessageOperation.Result);
            }
            catch(Exception ex) {
                return new OperationResult(new[] { ex });
            }

            return new OperationResult();
        }

        public OperationResult SendEmail(EmailLoggerParameter model) =>
            SendEmail(model.Subject, model.Body, model.Receipients, model.Sender, model.isBodyHtml);


    }
}