using PlanningPoker.Core.Entities;
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
        void SendGameStartLinkEmail(string toAddress, string playerName, string gameName, Guid playerId, Guid gameId);

        /// <summary>
        /// Method used to send an email that has the game summary
        /// </summary>
        /// <param name="toAddress">The email to address</param>
        /// <param name="game">The finished <see cref="Game"/></param>
        void SendGameSummaryEmail(string toAddress, Game game);
    }

    public class EmailUtility : IEmailUtility
    {
        /// <inheritdoc />
        public void SendGameStartLinkEmail(string toAddress, string playerName, string gameName, Guid playerId, Guid gameId)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Hello " + playerName + ",<br/>");
            sb.Append("You setup a game and wanted to come back to it at a later point so here a link you can use to do so:<br/>");
            sb.Append("<a href='http://planningpoker.harbargerdev.com/Home/GameMasterReturn?playerId=" + playerId.ToString() +
                        "&gameId=" + gameId.ToString() + "'>Game Start</a><br/><br/>");
            sb.Append("If you need a link for players to join, here is a link for them:<br/>");
            sb.Append("<a href='http://planningpoker.harbargerdev.com/Home/PlayerStart?gameId=" + gameId.ToString() + "'>Player Start</a><br/>");
            sb.Append("Thank you for your interest in playing an we look forward to you coming back to play.<br/>");
            sb.Append("<br/>Sincerely<br/>");
            sb.Append("harbargerdev");

            string body = sb.ToString();
            SendMessage(toAddress, "Come Back to Start Your Game", body);
        }

        /// <inheritdoc />
        public void SendGameSummaryEmail(string toAddress, Game game)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Hello " + game.GameMaster.PlayerName + ",<br/><br/>");
            sb.Append("Here is your post game summary for " + game.GameName + ".<br/>");
            sb.Append("Players:<br/>");
            sb.Append("<ul>");
            foreach(var player in game.Players)
            {
                sb.Append("<li>" + player.PlayerName + "</li>");
            }
            sb.Append("</ul><br/>");
            sb.Append("Stories sized:<br/>");
            sb.Append("<table><thead><tr><td>Story Card</td><td>Developer Size</td><td>Testing Size</td><td>Story Size</td></thead>");
            foreach(var story in game.Cards)
            {
                sb.Append("<tr><td>" + story.CardNumber + "</td><td>" + story.DeveloperSize + "</td><td>" + story.TestingSize + "</td><td>" + story.StorySize + "</td></tr>");
            }
            sb.Append("</table><br />");
            sb.Append("Thanks again for playing, and come back for your next session.");

            string body = sb.ToString();
            SendMessage(toAddress, "Your Game Summary", body);
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
