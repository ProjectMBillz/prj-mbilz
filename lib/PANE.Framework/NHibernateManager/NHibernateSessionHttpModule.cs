namespace CBC.Framework.NHibernateManager
{
    using System;
    using System.Web;

    public class NHibernateSessionHttpModule : IHttpModule
    {
        private void context_BeginRequest(object sender, EventArgs e)
        {
        }

        private void context_EndRequest(object sender, EventArgs e)
        {
            NHibernateSessionManager.Instance.CloseSession();
        }

        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(this.context_BeginRequest);
            context.EndRequest += new EventHandler(this.context_EndRequest);
        }
    }
}

