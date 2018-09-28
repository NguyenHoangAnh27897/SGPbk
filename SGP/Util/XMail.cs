using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Net;

/// <summary>
/// Thư viện tiện ích gửi email thông qua tài khoản gmail
/// </summary>
public class XMail
{
    
    /// <summary>
    /// Gửi email thông qua tài khoản gmail
    /// </summary>
    /// <param name="from">Email người gửi</param>
    /// <param name="to">Email người nhận</param>
    /// <param name="cc">Danh sách email những người cùng nhận phân cách bởi dấu phẩy</param>
    /// <param name="bcc">Danh sách email những người cùng nhận phân cách bởi dấu phẩy</param>
    /// <param name="subject">Tiêu đề mail</param>
    /// <param name="body">Nội dung mail</param>
    /// <param name="attachments">Danh sách file định kèm phân cách bởi phẩy hoặc chấm phẩy</param>
    public static void Send(String from, String to, String cc, String subject, String body)
    {
        var fromAddress = new MailAddress(from, "From Name");
        var toAddress = new MailAddress(to, "To Name");

        // Kết nối GMail
        var client = new SmtpClient
        {
            Host = "smtp.gmail.com",
            Port = 587,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential("loivv201@gmail.com",  "Naivecandy@2012")
        };

        // Gởi mail

        using (var message = new MailMessage(from, to)
        {
            Subject = subject,
            Body = body
        })
        {
            client.Send(message);
        }
    }
}