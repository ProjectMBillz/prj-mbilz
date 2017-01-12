using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;
using NHibernate;
using CBC.Framework.NHibernateManager;
using System.Timers;

namespace CBC.Framework.Presentation
{
    public abstract class BaseRecurringService : ServiceBase
    {
        private Timer _intervalTimer;
        private System.Threading.Thread _recurringThread;

        public BaseRecurringService()
        {
            //Initialize the timer from the configuration file
            _intervalTimer = new Timer()
            {
                AutoReset = false
            };
            _intervalTimer.Elapsed += new ElapsedEventHandler(_intervalTimer_Elapsed);
        }

        private void _intervalTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //Stop Timer so to prevent timer from elasping during processing
            _intervalTimer.Stop();

            //Encapsulate in a try-catch so that timer can start even if an error occurs
            try
            {
                //Call the recurrent Action
                /*_recurringThread = new System.Threading.Thread(new System.Threading.ThreadStart(RecurringAction));
                _recurringThread.Start();

                _recurringThread.Join();*/
                RecurringAction();
                CBC.Framework.NHibernateManager.NHibernateSessionManager.Instance.CloseSession();
                //Get Timer Interval
                _intervalTimer.Interval = GetTimerInterval();
            }
            catch(Exception ex)
            {
                //Rollback any pending changes
                NHibernateSessionManager.Instance.RollbackSession();

                //Log the Exception
               // Logger.LogException(ex);
            }
            finally
            {
                //Close the NHibernate Session




                NHibernateSessionManager.Instance.CloseSession();

                //Restart the timer for the next iteration
                _intervalTimer.Start();
            }
        }

        /// <summary>
        /// Gets Timer Interval for Recurrent Action
        /// </summary>
        /// <returns></returns>
        protected abstract double GetTimerInterval();

        /// <summary>
        /// Encapsulates the Main Recurrent Action for this Service
        /// </summary>
        protected abstract void RecurringAction();

        protected override void OnStart(string[] args)
        {
            //Start Timer
            _intervalTimer.Interval = GetTimerInterval();
            _intervalTimer.Start();
        }

        protected override void OnStop()
        {
            //Stop Timer so to prevent timer from elasping
            _intervalTimer.Stop();

            /*if (_recurringThread.ThreadState == System.Threading.ThreadState.Running)
            {
                _recurringThread.Abort();
            }*/

            try
            {
                //RollBack the NHibernate session if any pending changes exist
                NHibernateSessionManager.Instance.RollbackSession();
            }
            catch (Exception ex)
            {
               // Logger.LogException(ex);
            }
            finally
            {
                //Close the NHibernate Session
                NHibernateSessionManager.Instance.CloseSession();
            }
            //Close the timer
            _intervalTimer.Close();
        }
    }
}
