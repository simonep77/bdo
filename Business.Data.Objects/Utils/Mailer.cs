using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System.Net;
using System.Net.Security;
using System.Net.Mail;
using Bdo.Common;
using System.Collections.Generic;
using System.Threading;

namespace Bdo.Utils
{
    /// <summary>
    /// Classe per l'invio email generico
    /// </summary>
    public class Mailer
    {
        #region CAMPI
        private SmtpClient mSmtp;
        #endregion

        #region COSTRUTTORI

        public string Host
        {
            get { return this.mSmtp.Host; }
        }

        public int Port
        {
            get { return this.mSmtp.Port; }
        }

        
        #endregion

        #region COSTRUTTORI

        /// <summary>
        /// Crea mailer utilizzando i settaggi standard del file di configurazione
        /// (System.Net)
        /// </summary>
        public Mailer()
        {
            this.mSmtp = new SmtpClient();
        }

        /// <summary>
        /// Crea Mailer fornendo specificamente le informazioni smtp
        /// senza utilizzo di autenticazione
        /// </summary>
        /// <param name="smtpServer"></param>
        /// <param name="smtpPort"></param>
        public Mailer(string smtpServer, int smtpPort)
        {
            this.mSmtp = new SmtpClient(smtpServer, smtpPort);
            this.mSmtp.UseDefaultCredentials = true;
        }

        /// <summary>
        /// Crea Mailer fornendo specificamente le informazioni smtp
        /// e le credenziali di autenticazione senza utilizzo di SSL 
        /// </summary>
        /// <param name="smtpServer"></param>
        /// <param name="smtpPort"></param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        public Mailer(string smtpServer, int smtpPort, string user, string pass)
            :this(smtpServer, smtpPort, user, pass, false)
        {
        }

        /// <summary>
        /// Crea Mailer fornendo specificamente le informazioni smtp
        /// e le credenziali di autenticazione
        /// </summary>
        /// <param name="smtpServer"></param>
        /// <param name="smtpPort"></param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        public Mailer(string smtpServer, int smtpPort, string user, string pass, bool enableSSL)
        {
            this.mSmtp = new SmtpClient(smtpServer, smtpPort);
            this.mSmtp.UseDefaultCredentials = false;
            this.mSmtp.Credentials = new NetworkCredential(user, pass);
            this.mSmtp.EnableSsl = enableSSL;
        }

        /// <summary>
        /// Esegue invio email fornendo tutti i parametri disponibili
        /// </summary>
        /// <param name="from">
        /// Se non fornito utilizza configurazione system.net
        /// </param>
        /// <param name="to">
        /// Destinatari
        /// </param>
        /// <param name="cc">
        /// Destinatari in copia conoscenza
        /// </param>
        /// <param name="bcc">
        /// Destinatari in copia nascosta
        /// </param>
        /// <param name="subject">
        /// Oggetto
        /// </param>
        /// <param name="htmlbody">
        /// Body html
        /// </param>
        /// <param name="attachments">
        /// Lista di nomi file separati da , o ;
        /// </param>
        /// <param name="headers">
        /// Collezione di coppie Nome->Valore da utilizzare nell'header
        /// </param>
        public void Send(string from, string to, string cc, string bcc, string subject, string htmlbody, string attachments, NameValueCollection headers)
        {
            this.InternalSend(false, from, to, cc, bcc, subject, htmlbody, attachments, headers);
        }

        /// <summary>
        /// Esegue invio asincrono (su thread separato)
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="cc"></param>
        /// <param name="bcc"></param>
        /// <param name="subject"></param>
        /// <param name="htmlbody"></param>
        /// <param name="attachments"></param>
        /// <param name="headers"></param>
        public void SendAsync(string from, string to, string cc, string bcc, string subject, string htmlbody, string attachments, NameValueCollection headers)
        {
            var thd = new Thread(this._sendAsyncObj);
            var d = new Dictionary<string, object>
            {
                { @"from", from },
                { @"to", to },
                { @"cc", cc },
                { @"bcc", bcc },
                { @"subject", subject },
                { @"htmlbody", htmlbody },
                { @"attachments", attachments },
                { @"headers", headers }
            };

            thd.Start(d);



        }


        private void InternalSend(bool isAsync, string from, string to, string cc, string bcc, string subject, string htmlbody, string attachments, NameValueCollection headers)
        {
            lock (this.mSmtp)
            {
                if (isAsync)
                {
                    try
                    {
                        this.InternalSend(false, from, to, cc, bcc, subject, htmlbody, attachments, headers);
                    }
                    catch (Exception)
                    {
                        //Eccezione su thread separato
                    }
                }
                else
                {
                    //Invia
                    using (MailMessage oMsg = new MailMessage())
                    {


                        //Aggiunge to SEMPRE
                        oMsg.To.Add(this.normalizeAddresses(to));

                        //Aggiunge solo se fornito altrimenti da config
                        if (!string.IsNullOrEmpty(from))
                        {
                            oMsg.From = new MailAddress(from);
                        }
                        else
                        {

                        }

                        //Aggiunge cc se forniti
                        if (!string.IsNullOrEmpty(cc))
                        {
                            oMsg.CC.Add(this.normalizeAddresses(cc));
                        }

                        //Aggiunge bcc se forniti
                        if (!string.IsNullOrEmpty(bcc))
                        {
                            oMsg.Bcc.Add(this.normalizeAddresses(bcc));
                        }


                        oMsg.IsBodyHtml = true;
                        oMsg.Subject = subject;
                        oMsg.Body = htmlbody;

                        //Aggiunge attachments
                        if (!string.IsNullOrEmpty(attachments))
                        {
                            foreach (string sAttach in attachments.Split(Constants.ARR_DEFAULT_SPLIT_CHARS, StringSplitOptions.RemoveEmptyEntries))
                            {
                                oMsg.Attachments.Add(new Attachment(sAttach));
                            }
                        }

                        //Se forniti headers
                        if (headers != null)
                        {
                            oMsg.Headers.Add(headers);
                        }

                        //Infine invia email
                        this.mSmtp.Send(oMsg);

                    }
                }
                
            }

        }

        private void _sendAsyncObj(object args)
        {

            var d = args as Dictionary<string, object>;
            //dynamic oArgs = args;
            this.InternalSend(true, d["from"] as string, 
                d["to"] as string,
                d["cc"] as string, 
                d["bcc"] as string, 
                d["subject"] as string, 
                d["htmlbody"] as string, 
                d["attachments"] as string, 
                d["headers"] as NameValueCollection);
        }

        #endregion

        #region PRIVATE

        /// <summary>
        /// normalizza elenco indirizzi email su unica stringa
        /// </summary>
        /// <param name="addressesIn"></param>
        /// <returns></returns>
        private string normalizeAddresses(string addressesIn)
        {
            if (string.IsNullOrEmpty(addressesIn))
                return string.Empty;

            return addressesIn.Trim().Replace(';', ',').Replace(" ", "").Trim(Constants.ARR_DEFAULT_SPLIT_CHARS);
        }

        #endregion

    }
}
