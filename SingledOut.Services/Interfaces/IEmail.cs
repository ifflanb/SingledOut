namespace SingledOut.Services.Interfaces
{
    public interface IEmail
    {
        /// <summary>
        /// Sends an Email.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="fromName"></param>
        /// <param name="to"></param>
        /// <param name="cc"></param>
        /// <param name="bcc"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="isHtml"></param>
        void SendEmail(string from, string fromName, string to, string cc, string bcc, string subject, string body, bool isHtml);
    }
}
