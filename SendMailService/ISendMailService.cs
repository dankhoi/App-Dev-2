using System.Threading.Tasks;
using App_Dev_2.SendMailService;

public interface ISendMailService {
    Task SendMail(MailContent mailContent);
    
    Task SendEmailAsync(string email, string subject, string htmlMessage);
}