namespace FoodDelivery.BLL.Services
{
    using Dto.EmailModel;
    using PartialModels.Email;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mail;
    using System.Net.Mime;

    public class EmailService : Interfaces.IEmailService
    {
        private readonly EmailData data;
        private readonly string htmlBody;
        private AlternateView htmlView = null;

        public EmailService(EmailData data, string htmlBody = null)
        {
            this.htmlBody = htmlBody; this.data = data;
        }

        public string SendMessage(MessageType msgType, string attachFile = null, string ownerRecepient = null)
        {
            try
            {
                // [0] - returnMsg, [1] - subject, [2] - body
                var param = htmlBody == null ? GetConcreteMsg(msgType).ToList() : GetConcreteMsg(msgType, htmlBody).ToList();
                var recepient = ownerRecepient != null ? ownerRecepient : "myemail@gmail.com";
                MailAddress from = new MailAddress(data.Email, data.UserName);
                MailAddress to = new MailAddress(recepient); // email to admin
                MailMessage m = new MailMessage(from, to);
                m.IsBodyHtml = true;
                m.Subject = param[1];
                if (htmlBody != null)
                {
                    htmlView =
                    AlternateView.CreateAlternateViewFromString(htmlBody, System.Text.Encoding.UTF8, "text/html");
                    m.AlternateViews.Add(htmlView); // And a html attachment to make sure.
                    m.Body = htmlBody;
                }
                else
                {
                    m.Body = param[2];
                }
                if (attachFile != null)
                    m.Attachments.Add(Attach(attachFile));
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.Credentials = new System.Net.NetworkCredential(Credentials.Mail, Credentials.Pass);
                smtp.EnableSsl = true;
                smtp.Send(m);
                return param[0];
            }
            catch (System.Exception ex)
            {
                return ex.Message;// + " | " + ex.StackTrace;
            }
        }

        private Attachment Attach(string file)
        {
            var attach = new Attachment(file, MediaTypeNames.Application.Octet);
            // Add time stamp information for the file.
            ContentDisposition disposition = attach.ContentDisposition;
            disposition.CreationDate = System.IO.File.GetCreationTime(file);
            disposition.ModificationDate = System.IO.File.GetLastWriteTime(file);
            disposition.ReadDate = System.IO.File.GetLastAccessTime(file);
            // Add the file attachment to this email message.
            return attach;
        }

        private IEnumerable<string> GetConcreteMsg(MessageType typeMsg, string htmlBody = "")
        {
            var returnLst = new List<string>();
            string returnMsg = string.Empty, subject = string.Empty, body = string.Empty;
            switch (typeMsg)
            {
                case MessageType.NewOrder:
                    returnMsg = "Замовлення успішно прийняте! Ми сконтактуємось з Вами. Дякуємо що обрали наш сервіс.";
                    subject = $"Нове замовлення  {data.SendingDate}";
                    body = $"<h1 style='color:green;font-weight:bold'>Привіт www.delivery-food.com!</h1><br/>" +
                           $"<h2 style='color:green;font-weight:bold'>I'm '{data.UserName}'. Я оформив наступне замовлення.</h2><br/><br/>" +
                           $"<p><i style='color:black;font-weight:bold'>{data.Comment}.</i></p>" +
                           $"<h2 style='color:violet;font-weight:bold'>Нижче в прикріпленному файлі, копія замовлення.</h2><br/><br/><h3>З пов. {data.UserName}.</h3>";
                    break;
                case MessageType.WriteUs:
                    returnMsg = "Повідомлення успішно надіслане. Скоро наш менеджер звяжеться з Вами!";
                    subject = $"Нове повідомлення з www.delivery-food.com";
                    body = $"<h1 style='font-weight:bold'>Привіт www.delivery-food.com! Я '{data.UserName}'</h1>" +
                           $"<h2 style='color:green;font-weight:bold'>{data.Comment}</h2><br/><br/>" +
                           $"<h3 style='color:green;font-weight:bold'>Phone: {data.Phone}</h3>" +
                           $"<h3 style='color:green;font-weight:bold'>Email: {data.Email}</h3><br/><br/><br/><br/>" +
                           $"<h4 style='color:darkred;font-weight:bold'>Best Regards, {data.UserName}.</h4>";
                    break;
                case MessageType.AcceptToClient:
                    returnMsg = "Замовлення успішно прийняте! Ми сконтактуємось з Вами. Дякуємо що обрали наш сервіс.";
                    subject = $"Замовлення з www.delivery-food.com";
                    body = "";
                    break;
                case MessageType.NewOrderHtmlBody:
                    returnMsg = "Замовлення успішно оброблене! Ми сконтактуємось з Вами. Дякуємо що обрали наш сервіс.";
                    subject = $"Нове замовлення {data.SendingDate}";
                    body = htmlBody;
                    break;
            }
            returnLst.AddRange(new string[] { returnMsg, subject, body });
            return returnLst;
        }
    }
}
