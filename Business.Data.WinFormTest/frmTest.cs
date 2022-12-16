using Business.Data.Objects.Core;
using Business.Data.Objects.Core.Objects;
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
using Business.Data.Objects.TestClass.BIZ;
using Business.Data.Objects.TestClass.DTO;
using Business.Data.Objects.Common.Cache;
using System.Diagnostics;

namespace Business.Data.WinFormTest
{
    public partial class frmTest : Form
    {
        public frmTest()
        {
            InitializeComponent();
        }


        private void WriteLog(string msg)
        {


            if (this.InvokeRequired)
                this.Invoke(new Action(() => WriteLog(msg)));

            this.txtLog.AppendText($"{DateTime.Now:dd/MM/yyyy HH:mm:ss}  ");
            this.txtLog.AppendText(msg);
            this.txtLog.AppendText(Environment.NewLine);
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
                var lst = ss1.CreatePagedList<OrdineLista>(1, 10).SearchByLinq(o => o.Id.In<uint>(Convert.ToUInt32("1"), 1000) & o.CodiceOrdine == "".PadLeft(3, '0'));

                WriteLog(lst.ToJSON());

                var aa = new uint[] { 1, 7, 4, 55 };
                lst = ss1.CreatePagedList<OrdineLista>(1, 10).SearchByLinq(o => o.Id.In(aa));
                WriteLog(lst.ToJSON());

                lst = ss1.CreatePagedList<OrdineLista>(1, 10).SearchByLinq(o => o.Id.In(aa.ToList().ToArray()));
                WriteLog(lst.ToJSON());

                lst = ss1.CreatePagedList<OrdineLista>(1, 10).SearchByLinq(o => o.DataInserimento.Between(new DateTime(2000, 1, 1), DateTime.Today));
                WriteLog(lst.ToJSON());

                //lst = ss1.CreatePagedList<OrdineLista>(1, 10).SearchByLinq(o => o.Utente.Like("aaa%"));
                //WriteLog(lst.ToXml());


                lst = ss1.CreatePagedList<OrdineLista>(1, 10).OrderByLinq(o => o.AnagraficaId).OrderByLinqDesc(o => o.Id).SearchByLinq((o => o.Id > 10));
                WriteLog(lst.ToJSON());

                var oo = ss1.LoadObjByLINQ<Ordine>(o => o.Id == 150);

                WriteLog($"Utente: {oo.DataInserimento}");

                var oo2 = ss1.LoadObjNullByLINQ<Ordine>(o => o.Id == 150000000);

                WriteLog($"Utente: {oo2}");

                var oo3 = ss1.LoadObjOrNewByLINQ<Ordine>(o => o.Id == 150000000);

                WriteLog($"Utente: {oo3}");

                // Dim o2 = oAnag1.ToDynamicObject()

                var count = 500;

                WriteLog("Elabrazione FILTER");
                var el3 = ss1.GetCurrentElapsed();
                for (int i = 0; i < count; i++)
                {
                    var l1 = ss1.CreatePagedList<OrdineLista>(1, 10).SearchByColumn(Filter.In(nameof(Ordine.StatoId), 1, 3).And(Filter.Betw(nameof(Ordine.DataInserimento), new DateTime(2000, 1, 1), DateTime.Today)));
                }
                var el4 = ss1.GetCurrentElapsed();
                this.WriteLog(el4.Subtract(el3).TotalMilliseconds.ToString());


                WriteLog("Elabrazione LINQ");
                var el1 = ss1.GetCurrentElapsed();
                for (int i = 0; i < count; i++)
                {
                    var l1 = ss1.CreatePagedList<OrdineLista>(1, 10).SearchByLinq(o => o.StatoId.In(1, 3) && o.DataInserimento.Between(new DateTime(2000, 1, 1), DateTime.Today));
                }
                var el2 = ss1.GetCurrentElapsed();
                this.WriteLog(el2.Subtract(el1).TotalMilliseconds.ToString());

                var filter = new LinqFilter<Ordine>();
                filter.And(o => o.StatoId.In(1, 3));
                filter.And(o => o.DataInserimento.Between(new DateTime(2000, 1, 1), DateTime.Today));

                var l3 = ss1.CreatePagedList<OrdineLista>(1, 10).SearchByLinq(filter.Result);
                this.WriteLog($"Espressione combinata. Count: {l3.Count}");


                // oAnag1.FillFrom(o2)

                // Me.WriteLog(o2)
                this.WriteLog(ss1.PrintInfo());
            }
        }

        private void jSONTOEFROMToolStripMenuItem_Click(object sender, EventArgs e)
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
                var lst = ss1.CreatePagedList<OrdineLista>(1, 10).SearchByLinq(o => o.Id > 1000);

                foreach (var item in lst)
                {
                    this.WriteLog(item.ToJSON());

                    var aa = ss1.CreateObject<Ordine>();

                    aa.FromJSON(item.ToJSON());

                    this.WriteLog(aa.Id.ToString());

                }

                ss1.ToBizObjectList<AnagraficaBiz, Anagrafica>(new List<Anagrafica>());

                // oAnag1.FillFrom(o2)

                // Me.WriteLog(o2)
                this.WriteLog(ss1.PrintInfo());
            }
        }

        private void lINQ2SQLENHANCEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var ss1 = this.CreateSlot())
            {
                ss1.DB.AutoCloseConnection = true;
                ss1.LiveTrackingEnabled = true;
                ss1.ChangeTrackingEnabled = true;
                ss1.UserName = "Simone";

                //ss1.OnUserInfoRequired += getUserInfoFromSlot;
                this.WriteLog("Query non paginata");

                ss1.DB.SQL = "SELECT * FROM ordini LIMIT 0, 1000";

                var res = ss1.DB.Query<OrdineDTO>();

                this.WriteLog(ss1.GetCurrentElapsed().ToString());
                this.WriteLog(res.Count.ToString());

                this.WriteLog("Query paginata");

                ss1.DB.SQL = "SELECT * FROM ordini order by id";

                var res2 = ss1.DB.Query<OrdineDTO>(2, 1000);

                this.WriteLog(ss1.GetCurrentElapsed().ToString());
                this.WriteLog(res2.Result.Count.ToString());
                this.WriteLog(res2.Pager.TotRecords.ToString());


                // Me.WriteLog(o2)
                this.WriteLog(ss1.PrintInfo());
            }
        }

        private void qUERYMAPSPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var ss1 = new BusinessSlot("MSSQL2005Database", "Server=xxxxx.;Database=xxxxx;User Id=xxx;Password=xxxx"))
            {
                ss1.DB.AutoCloseConnection = true;
                ss1.LiveTrackingEnabled = true;
                ss1.ChangeTrackingEnabled = true;
                ss1.UserName = "Simone";

                //ss1.OnUserInfoRequired += getUserInfoFromSlot;
                this.WriteLog("Query non paginata");

                ss1.DB.AddParameter("@CityId", 5983, DbType.Int32);
                ss1.DB.AddParameter("@Addr", "VIALE CAMILLO SABATINI", DbType.String);
                ss1.DB.CommandType = CommandType.StoredProcedure;
                ss1.DB.SQL = "SP_GT005_CAP_FIND_BY_ADDRESS_NAME";

                var results = ss1.DB.Query<SP_GT005_CAP_FIND_RESULT>();


                //ss1.OnUserInfoRequired += getUserInfoFromSlot;
                this.WriteLog("Query paginata");

                ss1.DB.AddParameter("@CityId", 5983, DbType.Int32);
                ss1.DB.AddParameter("@Addr", "VIALE CAMILLO SABATINI", DbType.String);
                ss1.DB.CommandType = CommandType.StoredProcedure;
                ss1.DB.SQL = "SP_GT005_CAP_FIND_BY_ADDRESS_NAME";

                var results2 = ss1.DB.Query<SP_GT005_CAP_FIND_RESULT>(0, 100);


                // Me.WriteLog(o2)
                this.WriteLog(ss1.PrintInfo());
            }
        }

        private void qUERYMAPPERSBENCHToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var ss1 = this.CreateSlot())
            {
                ss1.DB.AutoCloseConnection = true;
                ss1.LiveTrackingEnabled = true;
                ss1.ChangeTrackingEnabled = true;
                ss1.UserName = "Simone";

                //ss1.OnUserInfoRequired += getUserInfoFromSlot;
                this.WriteLog("Avvio");

                for (int i = 0; i < 500; i++)
                {
                    ss1.DB.SQL = "SELECT * FROM ordini LIMIT 0, 1000";

                    var res = ss1.DB.Query<OrdineDTO>();
                }



                this.WriteLog(ss1.GetCurrentElapsed().ToString());


                // Me.WriteLog(o2)
                this.WriteLog(ss1.PrintInfo());
            }
        }

        private void tESTLOGICALDELETEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var ss1 = this.CreateSlot())
            {
                ss1.DB.AutoCloseConnection = true;
                ss1.LiveTrackingEnabled = true;
                ss1.ChangeTrackingEnabled = true;
                ss1.UserName = "Simone";

                //ss1.OnUserInfoRequired += getUserInfoFromSlot;
                this.WriteLog("Avvio");

                var lst = ss1.CreatePagedList<OrdineLista>(1, 10).SearchByLinq(o => o.Id > 1000);

                var ord = lst.FirstOrDefault();

                //ss1.DeleteObject(ord);

                //this.WriteLog(ord.Cancellato.ToString());

                //this.WriteLog(ss1.GetCurrentElapsed().ToString());


                // Me.WriteLog(o2)
                this.WriteLog(ss1.PrintInfo());
            }
        }

        private void tESTCacheresultConLinqToolStripMenuItem_Click(object sender, EventArgs e)
        {

            using (var ss1 = this.CreateSlot())
            {
                ss1.DB.AutoCloseConnection = true;
                ss1.LiveTrackingEnabled = true;
                ss1.ChangeTrackingEnabled = true;
                ss1.UserName = "Simone";

                //ss1.OnUserInfoRequired += getUserInfoFromSlot;
                this.WriteLog("Avvio");

                var lst = ss1.CreateList<OrdineLista>(1, 10).CacheResult().SearchByLinq(o => o.Id > 1000);

                //lst.Select(x => x.Anagrafica).ToList().ForEach(x => this.WriteLog(x.ToJSON()));

                this.WriteLog(lst.First().ToJSON());
                this.WriteLog(lst.First().Anagrafica.ToJSON());

                lst.First().AnagraficaId = lst.Last().AnagraficaId;

                this.WriteLog(lst.First().ToJSON());
                this.WriteLog(lst.First().Anagrafica.ToJSON());


                this.WriteLog(lst.ToJSON());

                var lst2 = ss1.CreateList<OrdineLista>(1, 10).CacheResult().SearchByLinq(o => o.Id > 1000);

                this.WriteLog(lst.ToJSON());

                // Me.WriteLog(o2)
                this.WriteLog(ss1.PrintInfo());
            }

        }

        private void tESTCacheResultSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {

            using (var ss1 = this.CreateSlot())
            {
                ss1.DB.AutoCloseConnection = true;
                ss1.LiveTrackingEnabled = true;
                ss1.ChangeTrackingEnabled = true;
                ss1.UserName = "Simone";

                //ss1.OnUserInfoRequired += getUserInfoFromSlot;
                this.WriteLog("Avvio");
                var rnd = new Random();

                for (int i = 0; i < 100000; i++)
                {
                    var irnd = rnd.Next(1, 10000);
                    var lst = ss1.CreatePagedList<OrdineLista>(1, 10).CacheResult().SearchByLinq(o => o.Id > irnd);

                    //this.WriteLog(ss1.DB.Stats.ToString());

                    if (i % 1000 == 0)
                        this.WriteLog(i.ToString());
                }


                // Me.WriteLog(o2)
                this.WriteLog(ss1.PrintInfo());

                GC.Collect();
                GC.WaitForPendingFinalizers();

                ss1.ResetCacheGlobal();
                ss1.ResetListGlobal();
                ss1.LiveTrackingClear();
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();



        }

        private void tESTCacheSimpleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var ss1 = this.CreateSlot())
            {
                ss1.DB.AutoCloseConnection = true;
                ss1.LiveTrackingEnabled = true;
                ss1.ChangeTrackingEnabled = true;
                ss1.UserName = "Simone";
                

                var sw = new Stopwatch();
                sw.Start();

                var c2 = new LruCache<string, int>(20000);

                //for (int i = 0; i < 1000000; i++)
                //{
                //    c2.AddOrUpdate(i.ToString(), i);
                //}
                //this.WriteLog(ss1.GetCurrentElapsed().ToString());

                for (int i = 0; i < 1000000; i++)
                {
                    c2.TryGet(i.ToString(), out var k);
                }
                this.WriteLog(sw.Elapsed.ToString());
                //this.WriteLog(ss1.GetCurrentElapsed().ToString());

                //this.WriteLog(c2.Print());
            }





        }

        private void tESTSimpleAESToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var s = new SimpleAes("pippi");

            var enc = s.Encrypt("Forza Roma");

            this.WriteLog(enc);

            this.WriteLog(s.Decrypt(enc));
        }
    }
}
