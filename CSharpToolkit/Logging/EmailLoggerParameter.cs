using System.Net.Mail;

namespace CSharpToolkit.Logging {
    public class EmailLoggerParameter {
        public string Subject { get; set; }
        public string Body { get; set; }
        public EmailReceipients Receipients { get; set; }
        public MailAddress Sender { get; set; }
        public bool isBodyHtml { get; set; }
    }
}
