using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Service.Utils
{
    public class SendEMail
    {
       // private static Logger logger = LogManager.GetCurrentClassLogger();

        public static bool sendMail(string sourceEmail, List<string> destEmails, string htmlFile,
            string hostName, string subject, string senderUname, string senderPword)
        {
            try
            {
                var mail = new MailMessage();
                foreach (var item in destEmails)
                {
                    mail.To.Add(item);
                }

                mail.From = new MailAddress(sourceEmail);
                mail.Subject = subject;
                string Body = htmlFile;

                mail.Body = Body;
                mail.IsBodyHtml = false;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = hostName;
                if (!String.IsNullOrEmpty(senderUname) && !String.IsNullOrEmpty(senderPword))
                {
                    smtp.Credentials = new System.Net.NetworkCredential(senderUname, senderPword);
                }
                smtp.EnableSsl = false;
                smtp.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
               // logger.Error(ex.Message);
                throw ex;
            }
        }

    }
}
