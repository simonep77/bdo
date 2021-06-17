using Business.Data.Objects.Core;
using Business.Data.Objects.Common.Utils;
using Business.Data.Objects.TestClass.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq.Expressions;

namespace Business.Data.WinFormTest
{
    public partial class frmTest : Form
    {
        public frmTest()
        {
            InitializeComponent();
        }


        /// <summary>
        ///     ''' Scrive log a video
        ///     ''' </summary>
        ///     ''' <param name="msgFmt"></param>
        ///     ''' <param name="args"></param>
        ///     ''' <remarks></remarks>
        private void WriteLog(string msgFmt, params object[] args)
        {
            string msg = string.Format(msgFmt, args);

            if (this.InvokeRequired)
                this.Invoke(new Action(() => WriteLog(msg)));

            this.txtLog.AppendText(string.Format("{0}  {1}{2}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), msg, Environment.NewLine));
        }

        /// <summary>
        ///     ''' Crea slot 
        ///     ''' </summary>
        ///     ''' <returns></returns>
        private BusinessSlot CreateSlot()
        {
            return this.CreateSlotTest();
        }

        private BusinessSlot CreateSlotTest()
        {
            var cn = System.Configuration.ConfigurationManager.ConnectionStrings["TEST"];
            return new BusinessSlot(cn.ProviderName, cn.ConnectionString);
        }

        private void lINQ2SQLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var ss1 = this.CreateSlot())
            {
                ss1.DB.AutoCloseConnection = true;
                ss1.LiveTrackingEnabled = true;
                ss1.ChangeTrackingEnabled = true;
                ss1.UserName = "Simone";

                //ss1.OnUserInfoRequired += getUserInfoFromSlot;


                // cancellazione logica
                var dtOgg = DateTime.Now;
                // Dim lst = ss1.CreatePagedList(Of OrdineLista)(1, 10).SearchByLinq(Function(o) o.Id > 1000 And (o.StatoId = 1 Or o.StatoId = 3) And DateTime.Now >= o.DataInserimento And dtOgg >= o.DataInserimento And o.CodiceOrdine <> ss1.UserName)
                // Dim lst = ss1.CreatePagedList(Of OrdineLista)(1, 10).SearchByLinq(Function(o) o.Id.OpIN(1, 1000))
                var lst = ss1.CreatePagedList<OrdineLista>(1, 10).SearchByLinq(o => o.Id.In<uint>(Convert.ToUInt32("1"), 1000) & o.Utente == "".PadLeft(3, '0'));

                WriteLog(lst.ToXml());

                lst = ss1.CreatePagedList<OrdineLista>(1, 10).SearchByLinq(o => o.DataInserimento.Between(new DateTime(2000, 1, 1), DateTime.Today));
                WriteLog(lst.ToXml());

                lst = ss1.CreatePagedList<OrdineLista>(1, 10).SearchByLinq(o => o.Utente.Like("aaa%"));
                WriteLog(lst.ToXml());


                lst = ss1.CreatePagedList<OrdineLista>(1, 10).SearchByLinq((o => o.Id > 10));
                WriteLog(lst.ToXml());

                var oo = ss1.LoadObjByLINQ<Ordine>(o => o.Id == 150);

                WriteLog("Utente: {0}", oo.DataInserimento);

                var oo2 = ss1.LoadObjNullByLINQ<Ordine>(o => o.Id == 150000000);

                WriteLog("Utente: {0}", oo2);

                var oo3 = ss1.LoadObjOrNewByLINQ<Ordine>(o => o.Id == 150000000);

                WriteLog("Utente: {0}", oo3);

                // Dim o2 = oAnag1.ToDynamicObject()

                // oAnag1.FillFrom(o2)

                // Me.WriteLog(o2)
                this.WriteLog(ss1.PrintInfo());
            }
        }
    }
}
