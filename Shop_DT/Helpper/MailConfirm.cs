using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Shop_DT.Helpper
{
    public class MailConfirm
    {
        public static async Task<string> SendMail(string _form, string _to, string _subject, string _body,string _gmail, string _pass)
        {
            MailMessage mess = new MailMessage(_form,_to,_subject,_body);
            mess.BodyEncoding = System.Text.Encoding.UTF8;
            mess.SubjectEncoding = System.Text.Encoding.UTF8;
            mess.IsBodyHtml = true;

            mess.ReplyToList.Add(new MailAddress(_form));
            mess.Sender = new MailAddress(_form);

            using var smtpClient = new SmtpClient("smtp.gmail.com");
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential(_gmail, _pass);
            try
            {
               await smtpClient.SendMailAsync(mess);
                return "Sent sucessfuly";
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "Sent fail";
            }
        }
    }
}
