using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;

namespace Xpinn.SportsGo.Util.HelperClasses
{
    public enum Correo
    {
        Gmail,
        Hotmail,
        Yahoo
    }

    public class CorreoHelper
    {
        string _correoDestinatario { get; set; }
        string _correoEnviador { get; set; }
        string _claveCorreoEnviador { get; set; }

        //Server  Name SMTP Address    Port  SSL
        //Yahoo!  smtp.mail.yahoo.com  587	 Yes
        //GMail   smtp.gmail.com	   587	 Yes
        //Hotmail smtp.live.com	       587	 Yes
        const int _puertoCorreo = 587;
        const bool useSSL = true;
        const bool useDefaultCredentials = true;

        // Recordar configurar la cuenta Gmail en este caso para que permita el logeo de manera insegura y poder mandar correos
        // https://myaccount.google.com/lesssecureapps?pli=1
        Dictionary<Correo, string> _servidorCorreoDiccionario = new Dictionary<Correo, string>()
        {
            { Correo.Gmail, "smtp.gmail.com" },
            { Correo.Hotmail, "smtp.live.com" },
            { Correo.Yahoo, "smtp.mail.yahoo.com" }
        };


        public CorreoHelper(string correoDestinatario, string correoEnviador, string claveEnviador)
        {
            _correoDestinatario = correoDestinatario.Trim();
            _claveCorreoEnviador = claveEnviador.Trim();
            _correoEnviador = correoEnviador.Trim();
        }

        // Envia Correo como texto puro
        public bool EnviarCorreoSinHTML(string mensaje, Correo servidorCorreo, string tema = " ", string nombreEnviador = " ")
        {
            try
            {
                using (MailMessage mailMessage = CrearMensaje(mensaje, tema, nombreEnviador))
                {
                    using (SmtpClient smtp = CrearSmtpClient(servidorCorreo))
                    {
                        smtp.Send(mailMessage);
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Envia Correo como HTML con la diferencia que puedes enviar a un servidor que no este en la Enum
        public bool EnviarCorreoConHTML(string mensaje, string hosting, int puerto, string tema = " ", string nombreEnviador = " ")
        {
            try
            {
                hosting = hosting.Trim();
                using (MailMessage mailMessage = CrearMensaje(mensaje, tema, nombreEnviador, true))
                {
                    using (SmtpClient smtp = CrearSmtpClient(hosting, puerto))
                    {
                        smtp.Send(mailMessage);
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Envia Correo como HTML
        public bool EnviarCorreoConHTML(string mensaje, Correo servidorCorreo, string tema = " ", string nombreEnviador = " ")
        {
            try
            {
                using (MailMessage mailMessage = CrearMensaje(mensaje, tema, nombreEnviador, true))
                {
                    using (SmtpClient smtp = CrearSmtpClient(servidorCorreo))
                    {
                        smtp.Send(mailMessage);
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        MailMessage CrearMensaje(string mensaje, string tema, string nombreEnviador, bool isBodyHtml = false)
        {
            MailMessage mailMessage = new MailMessage();

            mailMessage.To.Add(_correoDestinatario);
            mailMessage.From = new MailAddress(_correoEnviador, nombreEnviador);
            mailMessage.Subject = tema;
            mailMessage.Body = mensaje;
            mailMessage.IsBodyHtml = isBodyHtml;

            return mailMessage;
        }

        SmtpClient CrearSmtpClient(Correo servidorCorreo)
        {
            SmtpClient smtp = new SmtpClient();
            smtp.Host = _servidorCorreoDiccionario[servidorCorreo];
            smtp.EnableSsl = true;
            NetworkCredential NetworkCred = new NetworkCredential(_correoEnviador, _claveCorreoEnviador);
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = NetworkCred;
            smtp.Port = _puertoCorreo;

            return smtp;
        }

        SmtpClient CrearSmtpClient(string host, int puerto)
        {
            SmtpClient smtp = new SmtpClient();
            smtp.Host = host.Trim();
            smtp.EnableSsl = true;
            NetworkCredential NetworkCred = new NetworkCredential(_correoEnviador, _claveCorreoEnviador);
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = NetworkCred;
            smtp.Port = puerto;

            return smtp;
        }
    }
}
