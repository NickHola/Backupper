using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Main.DataOre;
using Main.Validations;

namespace Main.Roots
{
    public static class Root
    {
        internal static NotifyIcon ntiRoot;
        private static bool accessoRoot;

        public static bool AccessoRoot
        {
            get { return accessoRoot; }
            internal set
            {
                Validation.CtrlValue(value);
                accessoRoot = value;
            }
        }

        internal static bool RichiestaAccesso()
        {
            //if (AccessoRoot == false)
            //{
            //    WndAccessoRoot wnd = new WndAccessoRoot();
            //    wnd.ShowDialog();
            //}
            return true;
        }

        public static void RimuoviRoot()
        { AccessoRoot = false; }

        internal static void ThrMostraNotificaAccessoRoot()
        {

            UInt16 intervalloVisualMessMs;
            DateTime attesaTempo = DateTime.MinValue;

            ntiRoot = new NotifyIcon();
            ntiRoot.Icon = Main.Properties.Resources.KeyRootIcon;
            ntiRoot.Text = "Accesso root";
            ntiRoot.BalloonTipTitle = App.SharedCodeApp.GetAppName();

#if DEBUG == true
            intervalloVisualMessMs = 60000;
#else
            intervalloVisualMessMs = 30000;
#endif


#if DEBUG == false
            try
            {
#endif
                while (true)
                {
                    Thread.Sleep(500);
                    if (AccessoRoot == true)
                    {
                        if (ntiRoot.Visible == false)
                        {
                            ntiRoot.Visible = true;
                            ntiRoot.BalloonTipText = "Accesso root ATTIVO";
                        }
                        if (DataOra.AttesaTempo(ref attesaTempo, intervalloVisualMessMs) == false) continue;
                        ntiRoot.ShowBalloonTip(5000);
                    }
                    else
                    {
                        if (ntiRoot.Visible == true)
                        {
                            ntiRoot.BalloonTipText = "Accesso root RIMOSSO";
                            ntiRoot.ShowBalloonTip(5000);
                            ntiRoot.Visible = false;
                            attesaTempo = DateTime.MinValue;
                        }
                    }
                }

#if DEBUG == false
            }
            catch (Exception ex)
            { Thrs.Thr.NotificaErrThrCiclo(ex, true); }
#endif
        }
    }
}
