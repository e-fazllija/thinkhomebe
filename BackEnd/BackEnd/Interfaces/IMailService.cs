using BackEnd.Models.MailModels;

namespace BackEnd.Interfaces;

public interface IMailService
{
    Task SendEmailAsync(MailRequest mailRequest);

}