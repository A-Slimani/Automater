using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Automater
{

  class EmailDetails
  {
    [Required]
    public string sender { get; set; } = string.Empty;

    [Required]
    public string password { get; set; } = string.Empty;

    [Required]
    public string receiver { get; set; } = string.Empty;
  }

  public static class AutomaterEmail
  {
    public static void EmailUpdateSend(Serilog.ILogger logger, int pointsEarnedToday)
    {
      var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

      var configuration = builder.Build();
      var emailDetails = new EmailDetails();
      configuration.GetSection("EmailSends").Bind(emailDetails);

      var valiadationResults = new List<ValidationResult>();
      var validationContext = new ValidationContext(emailDetails);
      Validator.TryValidateObject(emailDetails, validationContext, valiadationResults, true);

      if (!valiadationResults.Any())
      {
        try
        {
          string date = DateTime.Now.ToString("dd-MM-yy");

          var email = new MimeMessage(); 
          email.From.Add(new MailboxAddress("Automater", emailDetails.sender));
          email.To.Add(new MailboxAddress("Recipient",emailDetails.receiver));
          email.Subject = $"Daily Automater Update: {date}";
          email.Body = new TextPart("plain")
          {
            Text = $"Points Earned Today: {pointsEarnedToday}",
          };

          using var client = new SmtpClient();
          client.Connect("smtp.live.email", 587, SecureSocketOptions.StartTls);
          client.Timeout = 15;
          client.Authenticate(emailDetails.sender, emailDetails.password);
          client.Send(email);
          client.Disconnect(true);

          logger.Information($"Email Update sent to {emailDetails.receiver}");
        }
        catch (Exception ex)
        {
          logger.Error(ex.ToString());
        }
      }
      else 
      {
        logger.Error("Missing email send details");
      }
    }
  }
}