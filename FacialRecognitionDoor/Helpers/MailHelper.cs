using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Email;

using System.IO;
using Windows.Storage;
using Windows.UI.Popups;
using LightBuzz.SMTP;
using  EAGetMail;

namespace FacialRecognitionDoor.Helpers
{
    public static class MailHelper
    {
        static string hostAddress = "rpifacerecog@outlook.com";
        static string hostPassword = "darks1d1ers";
        static string receipent = "tazimtazim2012@gmail.com";

        public static async Task<bool> SendMail(StorageFile image)
        {


            try
            {
                SmtpClient client = new SmtpClient("smtp-mail.outlook.com", 587, false,hostAddress, hostPassword);
                EmailMessage emailMessage = new EmailMessage();


                emailMessage.To.Add(new EmailRecipient(receipent));
                emailMessage.Subject = "New Visitor";
                emailMessage.Body = "Should I let him in?";
                var stream = Windows.Storage.Streams.RandomAccessStreamReference.CreateFromFile(image);
                var attachment = new EmailAttachment( image.Name,stream);
                emailMessage.Attachments.Add(attachment);




                await client.SendMail(emailMessage);
                
                return true;
            }
            catch (Exception)
            {
                return false;

            }



        }


        public static async Task<bool> ReadMail()
        {

            MessageDialog dialogue;

            // Yahoo POP3 server is "pop.mail.yahoo.com"
            MailServer oServer = new MailServer("pop3.live.com",
                        hostAddress, hostPassword, ServerProtocol.Pop3); // USE YOUR YAHOO EMAIL AND PASSWORD
            MailClient oClient = new MailClient("TryIt");

            // Set SSL connection
            oServer.SSLConnection = true;

            // Set 995 SSL port
            oServer.Port = 995;

            try
            {
                await oClient.ConnectAsync(oServer);
                IList<MailInfo> infos = await oClient.GetMailInfosAsync();

               

                for (int i = 0; i < infos.Count; i++) // For all email
                {
                    MailInfo info = infos[i];             
                    // Download email from Yahoo POP3 server
                    Mail oMail = await oClient.GetMailAsync(info);


                    string frommail = oMail.From.ToString();
                    string subjectmail = oMail.Subject;
                   
                    if (frommail.Contains(receipent) && subjectmail.Contains("yes"))
                    {
                        await oClient.DeleteAsync(info);
                            //email will be deleted so that always newest email will be checked 
                        await oClient.QuitAsync();
                        return true;
                    }

                   
                }

                // Quit and pure emails marked as deleted from Yahoo POP3 server.
                await oClient.QuitAsync();
                return false;
            }
            catch (Exception ep)
            {
                dialogue = new MessageDialog(ep.Message);
                await dialogue.ShowAsync();
                return false;
            }
            

        }

    }
}
