using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace PlanningPoker.Core.Utilities
{
    public interface IEmailUtility
    {
        /// <summary>
        /// Method used to send an email that has a link to the Game Master Start page
        /// </summary>
        /// <param name="toAddress">The email to address</param>
        /// <param name="playerName">The user's name</param>
        /// <param name="gameId">The game id</param>
        void SendGameStartLinkEmail(string toAddress, string playerName, string gameName, Guid gameId);
    }

    public class EmailUtility : IEmailUtility
    {
        /// <inheritdoc />
        public void SendGameStartLinkEmail(string toAddress, string playerName, string gameName, Guid gameId)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Hello " + playerName + ",<br/>");
            sb.Append("You setup a game and wanted to come back to it at a later point so here a link you can use to do so:<br/>");
            sb.Append("<a href='http://planningpoker.harbargerdev.com/Home/GameMasterStart?playerName=" + playerName +
                        "&gameName=" + gameName + "&submitButton=Start+Game%21'>Game Start</a><br/><br/>");
            sb.Append("If you need a link for players to join, here is a link for them:<br/>");
            sb.Append("<a href='http://planningpoker.harbargerdev.com/Home/PlayerStart?gameId=" + gameId.ToString() + "'>Player Start</a><br/>");
            sb.Append("Thank you for your interest in playing an we look forward to you coming back to play.<br/>");
            sb.Append("<br/>Sincerely<br/>");
            sb.Append("harbargerdev");

            string body = sb.ToString();
            SendMessage(toAddress, "Come Back to Start Your Game", body);
        }

        private void SendMessage(string toAddress, string subject, string body)
        {
            // From Address
            string from = "planningpoker@harbargerdev.com";
            string fromName = "Planning Poker";

            // SMTP Username
            string smtpUsername = "User";
            
            // SMTP Password
            string smtpPassword = "Password1!";

            // SMTP Host
            string smtpHost = "email.smtp.us-east-2.amazonaws.com";

            // Port
            int smtpPort = 587;

            // Create and build the MailMessage object
            MailMessage message = new MailMessage();
            message.IsBodyHtml = true;
            message.From = new MailAddress(from, fromName);
            message.To.Add(new MailAddress(toAddress));
            message.Subject = subject;
            message.Body = body;

            using (var client = new SmtpClient(smtpHost, smtpPort))
            {
                // Pass SMTP credentials
                client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);

                // Enable SSL Encryption
                client.EnableSsl = true;

                // try to send message
                try
                {
                    client.Send(message);
                }
                catch (Exception ex)
                {                    
                }
            }
        }
    }
}
