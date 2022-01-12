using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Cores.Common
{
    public class MyMail
    {
        private string Host;
        private int Port;
        private string MailAccount;
        private string MailPassword;
        private string MailDomain = "";
        private string SslEnable;
        public MyMail(string host, int port, string mailAccount, string mailPassword, string sslEnable = "0")
        {
            Host = host;
            Port = port;
            MailAccount = mailAccount;
            MailPassword = mailPassword;
            SslEnable = sslEnable;
        }

        public string Send_Email(string toMail,
                               string ccMail,
                               string subject,
                               string htmlString,
                               string attachmentFilename = null)
        {
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress(MailAccount);
                message.To.Add(new MailAddress(toMail));
                if (ccMail.Trim() != "")
                {
                    message.CC.Add(ccMail);
                }
                message.Subject = subject;
                message.IsBodyHtml = true; //to make message body as html  
                message.Body = htmlString;
                //Attachment
                if (attachmentFilename != null && attachmentFilename.Trim() != "")
                    message.Attachments.Add(new Attachment(attachmentFilename));
                //
                smtp.Port = Port;   // 587;
                smtp.Host = Host;   // "smtp.gmail.com"; //for gmail host  
                if (SslEnable == "1")
                {
                    smtp.EnableSsl = true;
                }
                else
                {
                    smtp.EnableSsl = false;
                }
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(MailAccount, MailPassword, MailDomain);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            //
            return "";
        }




    }//End class
}//End namespace
