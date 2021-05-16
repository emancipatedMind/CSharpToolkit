namespace CSharpToolkit.Logging.Abstractions {
    using System.Net.Mail;
    using System.Threading.Tasks;
    using Utilities;
    /// <summary>
    /// Adorned by a class who can log with emails asynchronously.
    /// </summary>
    public interface IEmailLoggerAsync {
        /// <summary>
        /// Send an email.
        /// </summary>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="body">The body of the email.</param>
        /// <param name="receipients">The receipients of the email.</param>
        /// <param name="sender">The sender of the email.</param>
        /// <param name="isBodyHtml">Whether the body is html or not.</param>
        /// <returns>Whether send performed successfully.</returns>
        Task<OperationResult> SendEmailAsync(string subject, string body, EmailReceipients receipients, MailAddress sender, bool isBodyHtml);

        /// <summary>
        /// Send an email.
        /// </summary>
        /// <param name="model">Model which represents email.</param>
        /// <returns>Whether send performed successfully.</returns>
        Task<OperationResult> SendEmailAsync(EmailLoggerParameter model);
    }
}
