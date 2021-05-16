namespace CSharpToolkit.Logging {
    using Extensions;
    using System.Collections.Generic;
    using System.Net.Mail;
    using System.Linq;
    /// <summary>
    /// A class that holds the receipients for an email.
    /// </summary>
    public class EmailReceipients {

        /// <summary>
        /// Instantiates the class that holds receipients for an email.
        /// </summary>
        /// <param name="receipients">The fully visible receipients of the email.</param>
        /// <param name="ccReceipients">The copied visible receipients of the email.</param>
        /// <param name="blindCCReceipients">The invisible receipients of the email.</param>
        public EmailReceipients(IEnumerable<MailAddress> receipients = null, IEnumerable<MailAddress> ccReceipients = null, IEnumerable<MailAddress> blindCCReceipients = null) {
            Receipients = receipients?.ToArray() ?? new MailAddress[0];
            CCReceipients = ccReceipients?.ToArray() ?? new MailAddress[0];
            BlindCCReceipients = blindCCReceipients?.ToArray() ?? new MailAddress[0];
        }

        /// <summary>
        /// The fully visible receipients of the email.
        /// </summary>
        public IEnumerable<MailAddress> Receipients { get; }
        /// <summary>
        /// The copied visible receipients of the email.
        /// </summary>
        public IEnumerable<MailAddress> CCReceipients { get; }
        /// <summary>
        /// The invisible receipients of the email.
        /// </summary>
        public IEnumerable<MailAddress> BlindCCReceipients { get; }

        internal bool NoReceipientsSpecified =>
            (Receipients.Any() || CCReceipients.Any() || BlindCCReceipients.Any()) == false; 

        internal MailMessage GetReceipientHydratedMailMessage() {
            var message = new MailMessage();
            Receipients.ForEach(address => message.To.Add(address));
            CCReceipients.ForEach(address => message.CC.Add(address));
            BlindCCReceipients.ForEach(address => message.Bcc.Add(address));
            return message;
        }

    }
}