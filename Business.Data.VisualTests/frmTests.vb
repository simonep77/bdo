Imports BDO.Objects
Imports System.Reflection
Imports System.Linq
Imports System.Threading
Imports Business.Data.Objects.TestClass.DAL
Imports Business.Data.Objects.TestClass.BIZ



Public Class frmTests


    ''' <summary>
    ''' Scrive log a video
    ''' </summary>
    ''' <param name="msgFmt"></param>
    ''' <param name="args"></param>
    ''' <remarks></remarks>
    Private Sub WriteLog(ByVal msgFmt As String, ByVal ParamArray args() As Object)


        Dim msg As String = String.Format(msgFmt, args)

        If Me.InvokeRequired Then
            Me.Invoke(Sub() WriteLog(msg))
        End If

        Me.txtLog.AppendText(String.Format("{0}  {1}{2}", Date.Now.ToString("dd/MM/yyyy HH:mm:ss"), msg, Environment.NewLine))
    End Sub

    ''' <summary>
    ''' Crea slot 
    ''' </summary>
    ''' <returns></returns>
    Private Function CreateSlot() As BusinessSlot
        Return Me.CreateSlotTest()
    End Function

    Private Function CreateSlotTest() As BusinessSlot
        Return New BusinessSlot("LAVORO")
    End Function

    Private Sub AvvioSessioneToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AvvioSessioneToolStripMenuItem.Click
        Try

            Using ss1 = Me.CreateSlot()
                'Using ss1 As BDSession = BDNew BusinessSlot("MYSQLDataBase", "data source=fetest01;database=dbfaschim;userid=root;password=!av007aM")
                Dim dtStart As DateTime = DateTime.Now

                'For i As Integer = 1 To 1000
                '    Dim az As Azienda = Azienda.LoadByPK("FAS287")
                '    az.RagioneSociale = "PIPPò " & i.ToString()
                '    Me.WriteLog("{0}", az.RagioneSociale)
                '    Me.WriteLog("{0}", az.Provincia.ToString())
                '    Me.WriteLog("{0}", az.Provincia.Regione.ToString())
                '    Me.WriteLog("Select: {0}", ss1.DB.Stats.GetCounter(Bdo.Database.DBStats.EStatement.Select))

                'Next


                Dim lstAziende As ListaAziende = ss1.CreatePagedList(Of ListaAziende)(1, 100).OrderBy("RagioneSociale", Bdo.Objects.OrderVersus.Asc).CercaTutti()
                'For Each az As Azienda In lstAziende
                '    'Me.WriteLog("{0}", az.RagioneSociale)
                '    'Me.WriteLog("{0}", az.Provincia.ToString())
                '    'Me.WriteLog("{0}", az.Provincia.Regione.ToString())
                '    Me.WriteLog("Select: {0}", ss1.DB.Stats.GetCounter(Bdo.Database.DBStats.EStatement.Select))
                'Next

                'Me.dgv1.DataSource = lstAziende


                Dim a1 As Azienda = lstAziende(0)
                a1.RagioneSociale = a1.RagioneSociale
                'a1.Provincia = ss1.LoadObjByPK(Of Provincia)("RM")
                'a1.Blob = Nothing
                ss1.SaveObject(a1)


                'Me.WriteLog("{0}", a1.RagioneSociale)
                'Try

                '    Dim az1 As Azienda = Azienda.Create(ss1)
                '    az1.IdAzienda = "SIM001"
                '    az1.RagioneSociale = "Pippò càs"
                '    az1.PICF = "AAAAAAAAAAA"
                '    az1.Provincia = ss1.LoadObjectByPK(Of Provincia)("RM")
                '    ss1.SaveObject(az1)
                'Catch ex As Exception
                '    ss1.SharedLog.LogException(ex, False)
                '    Throw
                'End Try




                Dim dtEnd As DateTime = DateTime.Now

                Me.WriteLog("Elaps: {0}", dtEnd.Subtract(dtStart).TotalMilliseconds)
                Me.WriteLog("Select: {0}", ss1.DB.Stats.GetCounter(Bdo.Database.DBStats.EStatement.Select))

                Me.WriteLog(ss1.PrintCacheDebug())

            End Using
        Catch ex As Exception

            Me.WriteLog("ECCEZIONE! Tipo: {0} - {1}", ex.GetType().Name, ex.Message)
            Me.WriteLog("{0}", ex.StackTrace)
        End Try

    End Sub

    Private Sub AvvioSessioniMultipleToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AvvioSessioniMultipleToolStripMenuItem.Click
        Try
            Dim ss1 As BusinessSlot = New BusinessSlot("")

            Me.WriteLog("ss1 ID: {0}", ss1.SlotId)

            Dim ss2 As BusinessSlot = New BusinessSlot("")

            Me.WriteLog("ss1 ID: {0}", ss1.SlotId)
            Me.WriteLog("ss2 ID: {0}", ss2.SlotId)


            ss2.Dispose()
            Me.WriteLog("ss1 ID: {0}", ss1.SlotId)

            Me.WriteLog("ss1 Terminato: {0}", ss1.Terminated)
        Catch ex As Exception
            Me.WriteLog("ECCEZIONE! Tipo: {0} - {1}", ex.GetType().Name, ex.Message)
            Me.WriteLog("{0}", ex.StackTrace)
        End Try
    End Sub

    Private Sub SessioneOggettoToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SessioneOggettoToolStripMenuItem.Click
        Try
            Using ss1 As BusinessSlot = New BusinessSlot("")
                Me.WriteLog("ss1 ID: {0}", ss1.SlotId)



                Using ss2 As BusinessSlot = New BusinessSlot("")
                    Me.WriteLog("ss2 ID: {0}", ss2.SlotId)

                    Dim ass As Assembly = Assembly.GetEntryAssembly()
                    Dim aa As String = ass.Location

                    Me.WriteLog(aa)

                End Using

            End Using



        Catch ex As Exception
            Me.WriteLog("ECCEZIONE! Tipo: {0} - {1}", ex.GetType().Name, ex.Message)
            Me.WriteLog("{0}", ex.StackTrace)
        End Try
    End Sub

    Private Sub ProveSchema1ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ProveSchema1ToolStripMenuItem.Click

    End Sub

    Private Sub SchemaFactoryToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SchemaFactoryToolStripMenuItem.Click


        Try
            Using ss1 As BusinessSlot = New BusinessSlot("MSSQL2005DataBase", "server=dev-mssql05.sds.local;database=gsr;Integrated Security=True")

                'Dim lst As ListaDirigenti = ListaDirigenti.CreatePaged(1, 40)

                'lst.CercaTutti()
                'Me.WriteLog("Select: {0}", ss1.DB.Stats.GetCounter(Bdo.Database.DBStats.EStatement.Select))

                'For Each ob As Dirigente In lst
                '    ob.Save()
                '    Me.WriteLog("{0} {1} {2} {2}", ob.IdDirigente, ob.Cognome, ob.Nome, ob.DataNascita.ToString("dd/MM/yyyy"))
                '    Me.WriteLog("{0} {1}", ob.IdDirigente, ob.LetteraAttivazioneInviata)
                '    Me.WriteLog("Select: {0}", ss1.DB.Stats.GetCounter(Bdo.Database.DBStats.EStatement.Select))

                'Next



                Dim ob As Dirigente = ss1.LoadObjByPK(Of Dirigente)(2)
                Me.WriteLog("{0} {1} {2} {3}", ob.IdDirigente, ob.Cognome, ob.Nome, ob.DataNascita.ToString("dd/MM/yyyy"))
                Me.WriteLog("{0} {1}", ob.IdDirigente, ob.LetteraAttivazioneInviata)
                Me.WriteLog(ob.ToXml())

                Me.WriteLog("Select: {0}", ss1.DB.Stats.GetCounter(Bdo.Database.DBStats.EStatement.Select))

                Dim ob1 As Dirigente = ss1.CreateObject(Of Dirigente)()

                ob1.Cognome = "PIRIPIPPO"
                ob1.Nome = "aaaa"
                ob1.CodiceFiscale = "1234567"
                ob1.DataNascita = New Date(2008, 1, 1)
                'ob1.Save()

                Me.WriteLog("{0} {1}", ob1.IdDirigente, ob1.LetteraAttivazioneInviata)

                Me.WriteLog("Select: {0}", ss1.DB.Stats.GetCounter(Bdo.Database.DBStats.EStatement.Select))


            End Using
        Catch ex As Exception
            Me.WriteLog("ECCEZIONE! Tipo: {0} - {1}", ex.GetType().Name, ex.Message)
            Me.WriteLog("{0}", ex.StackTrace)
        End Try
    End Sub



    Private Sub ProvaOggettiMysqlToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ProvaOggettiMysqlToolStripMenuItem.Click

        Try
            Using ss1 = Me.CreateSlot()

                For index As Integer = 1 To 10
                    Dim ob As Azienda = ss1.LoadObjByPK(Of Azienda)("FAS287")
                    Dim oAzBiz As AziendaBiz = ob.ToBizObject(Of AziendaBiz)()

                    Me.WriteLog("{0} {1} {2}", ob.IdAzienda, ob.RagioneSociale, ob.PICF)
                    Me.WriteLog("{0} {1}", ob.IdAzienda, ob.Disabilitata)
                    Me.WriteLog(ob.ToString())
                    'ob.Disabilitata = False
                    'ob.Save()
                    Me.WriteLog(ob.ToXml(1))
                Next



                Me.WriteLog("Select: {0}", ss1.DB.Stats.GetCounter(Bdo.Database.DBStats.EStatement.Select))



            End Using
        Catch ex As Exception
            Me.WriteLog("ECCEZIONE! Tipo: {0} - {1}", ex.GetType().Name, ex.Message)
            If ex.InnerException IsNot Nothing Then
                Me.WriteLog("INNER! Tipo: {0} - {1}", ex.InnerException.GetType().Name, ex.InnerException.Message)
            End If
            Me.WriteLog("{0}", ex.StackTrace)
        End Try
    End Sub

    Private Sub OggettiCompostiMysqlToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OggettiCompostiMysqlToolStripMenuItem.Click
        Try
            Using ss1 = Me.CreateSlot()
                'Using ss1 As BDSession = BDNew BusinessSlot("MYSQLDataBase", "data source=localhost;database=testdb;userid=root;password=volley2")
                'Dim ob As Azienda = Azienda.LoadByPK("FAS287")

                Dim dtStart As Date = DateTime.Now


                'Dim lst As ListaAziende = ListaAziende.Create().OrderBy("RagioneSociale", ListaAziende.ORDER_ASCENDING).FindByQuery("SELECT IdAzienda FROM AZIENDE")
                'Dim lst As ListaAziende = ListaAziende.Create(ss1).CercaTutti()
                Dim lst As ListaAziende = ss1.CreateList(Of ListaAziende)().SearchByColumn(Filter.Like("IdAzienda", "A%"))
                'Dim lst As ListaAziende = ss1.CreateList(Of ListaAziende)().OrderBy("RagioneSociale", ListaAziende.ORDER_ASCENDING).FindByQuery("SELECT IdAzienda FROM AZIENDE WHERE Disabilitata <> '1' AND BloccoKit='1'")
                'lst.SearchByQuery("SELECT CodAzienda FROM ContabileAzienda WHERE CodAzienda='FAS287'")

                Dim az As Azienda = lst.FindByPK("AVE607")

                'lst.Add(ss1.CloneObject(Of Azienda)(az))

                az = lst.FindByPK("AVE607")

                Dim ii As Integer = lst.IndexOf(az)

                Dim az1 As Azienda = lst.FindByPK("FAS287")
                If az Is Nothing Then
                    Me.WriteLog("Azienda Non Trovata")
                Else
                    Me.WriteLog("Azienda {0} Trovata", az.RagioneSociale)
                End If

                For i As Integer = 0 To 100
                    Dim prov As Provincia = lst(i).Provincia
                    Dim prov2 As Provincia = lst(i).ProvinciaLeg
                    'Dim b As Boolean = az.BloccoKit
                Next


                Me.WriteLog("Elapsed1: {0}", DateTime.Now.Subtract(dtStart))
                Me.WriteLog("Select: {0}", ss1.DB.Stats.GetCounter(Bdo.Database.DBStats.EStatement.Select))

                'Me.dgv1.DataSource = lst

                Me.WriteLog(ss1.PrintInfo())
                Me.WriteLog(ss1.PrintCacheDebug())

            End Using
        Catch ex As Exception
            Me.WriteLog("ECCEZIONE! Tipo: {0} - {1}", ex.GetType().Name, ex.Message)
            Me.WriteLog("{0}", ex.StackTrace)
        End Try
    End Sub




    Private Sub New1ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles New1ToolStripMenuItem.Click


        Try
            Using ss1 = Me.CreateSlot()
                'Using ss1 As BDSession = BDNew BusinessSlot("MYSQLDataBase", "data source=localhost;database=testdb;userid=root;password=volley2")
                'Dim ob As Azienda = Azienda.LoadByPK("FAS287")

                Dim az As Azienda = ss1.LoadObjByPK(Of Azienda)("FAS287")
                Dim az2 As Azienda = ss1.LoadObjByPK(Of Azienda)("FAS287")

                az.RagioneSociale = "  PIPPO PLUTò   "
                Dim b As Boolean = Convert.ToBoolean(Convert.ToInt32(az.Disabilitata))
                az.Disabilitata = Convert.ToInt32(True).ToString()

                Me.WriteLog("AZ: {0}", az.RagioneSociale)
                Me.WriteLog("PROV: {0}", az.Provincia.Descrizione)

                Dim pr As Provincia = ss1.LoadObjByPK(Of Provincia)("MI")

                Me.WriteLog("PROV2: {0}", pr.Descrizione)


                Me.WriteLog("Select: {0}", ss1.DB.Stats.GetCounter(Bdo.Database.DBStats.EStatement.Select))


            End Using
        Catch ex As Exception
            Me.WriteLog("ECCEZIONE! Tipo: {0} - {1}", ex.GetType().Name, ex.Message)
            Me.WriteLog("{0}", ex.StackTrace)
        End Try

    End Sub

    Private Sub TestCacheToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TestCacheToolStripMenuItem.Click

        Using ss1 = Me.CreateSlot()

            Dim dtStart As Date = DateTime.Now

            'For index As Integer = 1 To 1000
            '    Dim az As Azienda = ss1.LoadObjectByPK(Of Azienda)("FAS287")
            'Next

            Dim az As Azienda = ss1.LoadObjByPK(Of Azienda)("FAS287")

            Dim az2 As Azienda = ss1.LoadObjByPK(Of Azienda)("FAS287")
            az2.DifferenzaPremioAzienda = 10.0

            Dim oDiffList As DataDiffList = az.Diff(az2)

            Me.WriteLog("Elapsed1: {0}", DateTime.Now.Subtract(dtStart))
            Me.WriteLog("Select: {0}", ss1.DB.Stats.GetCounter(Bdo.Database.DBStats.EStatement.Select))



        End Using


    End Sub

    Private Sub Schem2ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Schem2ToolStripMenuItem.Click
        Dim ss1 As New BusinessSlot("LAVORO")
        Try


            'Using ss1 As BDSession = BDNew BusinessSlot("MYSQLDataBase", "data source=localhost;database=testdb;userid=root;password=volley2")
            'Dim ob As Azienda = Azienda.LoadByPK("FAS287")

            Dim az As Azienda = ss1.LoadObjByPK(Of Azienda)("FAS287")
            Me.WriteLog("Stato: {0}", az.Stato)

            Dim prov As Provincia = ss1.LoadObjByPK(Of Provincia)("RM")
            az.ProvinciaLeg = prov


            'Dim az2 As Azienda = ss1.LoadObjectByKEY(Of Azienda)("CODICEFISCALE", "PLESMN77H30H501H")

            Dim rg As Regione = ss1.CreateObject(Of Regione)()

            rg.IdRegione = 199
            rg.Descrizione = "SIMONè"
            rg.AreaGeografica = "centro"
            'ss1.SaveObject(rg)

            Dim lst As ListaIscrittiPerPolizza = ss1.CreatePagedList(Of ListaIscrittiPerPolizza)(1, 10000).SearchAllObjects()

            rg.Descrizione = "ddddd"
            'ss1.SaveObject(rg)

            'Me.WriteLog(rg.ToXml(-1))
            Me.WriteLog(ss1.PrintInfo())
            Me.WriteLog(ss1.PrintCacheDebug())


        Catch ex As ObjectValidationException

            Me.WriteLog(ss1.MessageList.ToXml())

        Catch ex As Exception
            Me.WriteLog("ECCEZIONE! Tipo: {0} - {1}", ex.GetType().Name, ex.Message)
            Me.WriteLog("{0}", ex.StackTrace)
        End Try

    End Sub

    Private Sub TESTISCRITTIPERPOLIZZAToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TESTISCRITTIPERPOLIZZAToolStripMenuItem.Click
        Using ss1 = Me.CreateSlot()

            Dim dtStart As Date = DateTime.Now

            Dim rand As New Random(Date.Now.Millisecond)
            Dim iRand As Integer = rand.Next(0, 100)
            Me.WriteLog("Random: {0}", iRand)

            Dim lst As ListaAziende = ss1.CreatePagedList(Of ListaAziende)(rand.Next(0, 100), 100).SearchAllObjects()
            lst.SetPropertyMassive("CAP", "00000")

            dtStart = DateTime.Now

            Dim lstSorted As ListaAziende = lst.SortByProperty("PICF", False)
            Me.WriteLog("Elapsed2: {0}", DateTime.Now.Ticks - dtStart.Ticks)

            'For Each az As Azienda In lstSorted
            '    Me.WriteLog("{0} - {1}", az.IdAzienda, az.PICF)
            'Next

            Me.WriteLog("ElapsedFINALE: {0}", DateTime.Now.Subtract(dtStart))
            Me.WriteLog(ss1.PrintInfo())


        End Using


    End Sub


    Private Sub TestLogToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TestLogToolStripMenuItem.Click
        Using ss1 = Me.CreateSlot()

            Dim dtStart As Date = DateTime.Now

            Dim lst As ListaAziende = ss1.CreateList(Of ListaAziende)().SearchByColumn("Disabilitata", EOperator.Differs, "1")
            Me.WriteLog("Elapsed0: {0}", DateTime.Now.Subtract(dtStart))
            Dim oProv As Provincia = ss1.LoadObjByPK(Of Provincia)("RM")
            dtStart = DateTime.Now
            'Dim lstSorted As List(Of Azienda) = lst.FindAllByProperty("IdAzienda", "FAS287")
            Me.WriteLog("Elapsed1: {0}", DateTime.Now.Subtract(dtStart))


            dtStart = DateTime.Now

            For Each az As Azienda In lst
                'Me.WriteLog("{0} - {1}", az.IdAzienda, az.PICF)
            Next

            Me.WriteLog("Elapsed2: {0}", DateTime.Now.Subtract(dtStart))
            Me.WriteLog(ss1.PrintInfo())


        End Using

    End Sub


    Private Sub TestXmlWriteToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TestXmlWriteToolStripMenuItem.Click
        Dim dtStart As DateTime

        dtStart = DateTime.Now
        Using xw As New Bdo.Utils.XmlWrite()
            xw.WriteStartElement("root")
            For index As Integer = 1 To 100000
                xw.WriteStartElement("iscritto")
                xw.WriteElementString("nome", String.Concat("simone_", index.ToString()))
                xw.WriteElementString("cognome", String.Concat("pelaia_", index.ToString()))
                xw.WriteEndElement()
            Next
            xw.WriteEndElement()
        End Using
        Me.WriteLog("XmlWrite: {0}", DateTime.Now.Subtract(dtStart))


    End Sub

    Private Sub TestArrayBytesToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TestArrayBytesToolStripMenuItem.Click
        Try
            Using ss1 = Me.CreateSlot()

                Dim oProv As Provincia = ss1.LoadObjNullByPK(Of Provincia)("ZZ")


            End Using

        Catch ex As Exception

            Me.WriteLog("ECCEZIONE! Tipo: {0} - {1}", ex.GetType().Name, ex.Message)
            Me.WriteLog("{0}", ex.StackTrace)
        End Try
    End Sub

    Private Sub TestProprietàToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TestProprietàToolStripMenuItem.Click
        Dim NUM As Integer = 1000000
        Dim s1 As String = "Simone "
        Dim s2 As String = "  Pelaiàà "
        Dim sw As Stopwatch
        Using ss1 = Me.CreateSlot()
            ss1.CreateObject(Of Regione)()

            sw = Stopwatch.StartNew()
            Dim o1 As VecchieClassi.Provincia
            For index As Integer = 1 To NUM
                o1 = New VecchieClassi.Provincia()
                o1.codice = s1.Trim().ToUpper()
                o1.descrizione = s2.Trim().ToUpper()

                Dim o3 As Object = o1.descrizione
            Next
            Me.WriteLog("Oggetto Standard: {0}", sw.ElapsedTicks)

            sw = Stopwatch.StartNew()
            Dim o2 As Regione
            For index As Integer = 1 To NUM
                o2 = ss1.CreateObject(Of Regione)()
                o2.Descrizione = s1
                o2.AreaGeografica = s2

                Dim o3 As Object = o2.Descrizione
            Next
            Me.WriteLog("Oggetto Complesso: {0}", sw.ElapsedTicks)


            'Me.WriteLog(ss1.PrintInfo())
        End Using


    End Sub

    Private Sub TestMemoriaToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TestMemoriaToolStripMenuItem.Click
        Try
            Dim s As String
            Using ss1 = Me.CreateSlot()
                'Using ss1 As BDSession = BDNew BusinessSlot("MYSQLDataBase", "data source=fetest01;database=dbfaschim;userid=root;password=!av007aM")

                Dim dtStart As DateTime = DateTime.Now
                Me.WriteLog("Init, Memory: {0:N}", GC.GetTotalMemory(True) / 1024.0)

                For index As Integer = 1 To 100

                    Dim lstAziende As ListaAziende = ss1.CreatePagedList(Of ListaAziende)(1, 1000).OrderBy("RagioneSociale", Bdo.Objects.OrderVersus.Asc).CercaTutti()
                    For Each az As Azienda In lstAziende
                        s = az.RagioneSociale
                        s = az.Provincia.ToString()
                        s = az.Provincia.Regione.ToString()
                    Next

                    Me.WriteLog("Select: {0}", ss1.DB.Stats.GetCounter(Bdo.Database.DBStats.EStatement.Select))

                    Me.WriteLog("Loop {0}, Memory: {1:N}", index, GC.GetTotalMemory(True) / 1024.0)

                Next



                Dim dtEnd As DateTime = DateTime.Now

                Me.WriteLog("Elaps: {0}", dtEnd.Subtract(dtStart).TotalMilliseconds)
                Me.WriteLog("Select: {0}", ss1.DB.Stats.GetCounter(Bdo.Database.DBStats.EStatement.Select))


            End Using
        Catch ex As Exception

            Me.WriteLog("ECCEZIONE! Tipo: {0} - {1}", ex.GetType().Name, ex.Message)
            Me.WriteLog("{0}", ex.StackTrace)
        End Try
    End Sub

    Private Sub TestCacheNoCacheToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TestCacheNoCacheToolStripMenuItem.Click
        Dim N As Integer = 1000
        Try
            Using ss1 = Me.CreateSlot()
                Dim dtStart As DateTime = DateTime.Now


                For index As Integer = 1 To N

                    Dim az As Azienda = ss1.LoadObjByPK(Of Azienda)("fas287")

                Next

                Dim dtEnd As DateTime = DateTime.Now

                Me.WriteLog("Elaps: {0}", dtEnd.Subtract(dtStart).TotalMilliseconds)
                Me.WriteLog("Select: {0}", ss1.DB.Stats.GetCounter(Bdo.Database.DBStats.EStatement.Select))

            End Using

            Using ss1 = Me.CreateSlot()
                Dim dtStart As DateTime = DateTime.Now

                For index As Integer = 1 To N

                    Dim az As Azienda = ss1.LoadObjByPK(Of Azienda)("fas287")

                Next

                Dim dtEnd As DateTime = DateTime.Now

                Me.WriteLog("Elaps: {0}", dtEnd.Subtract(dtStart).TotalMilliseconds)
                Me.WriteLog("Select: {0}", ss1.DB.Stats.GetCounter(Bdo.Database.DBStats.EStatement.Select))

            End Using
        Catch ex As Exception

            Me.WriteLog("ECCEZIONE! Tipo: {0} - {1}", ex.GetType().Name, ex.Message)
            Me.WriteLog("{0}", ex.StackTrace)
        End Try
    End Sub

    Private Sub TestCacheNoCache2ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TestCacheNoCache2ToolStripMenuItem.Click
        Dim N As Integer = 5000
        Try
            Using ss1 = Me.CreateSlot()
                Dim dtStart As DateTime = DateTime.Now
                Randomize()
                Dim oLista As ListaAziende = ss1.CreateList(Of ListaAziende)().CercaTutti()

                For index As Integer = 1 To N
                    Dim i As Integer = New Random().Next(0, oLista.Count - 1)
                    Dim az As Azienda = ss1.LoadObjByPK(Of Azienda)(oLista(i).IdAzienda)

                Next

                Dim dtEnd As DateTime = DateTime.Now

                Me.WriteLog("Elaps: {0}", dtEnd.Subtract(dtStart).TotalMilliseconds)
                Me.WriteLog("Select: {0}", ss1.DB.Stats.GetCounter(Bdo.Database.DBStats.EStatement.Select))

            End Using

            Using ss1 = Me.CreateSlot()
                Dim dtStart As DateTime = DateTime.Now

                Dim oLista As ListaAziende = ss1.CreateList(Of ListaAziende)().CercaTutti()

                Randomize()

                For index As Integer = 1 To N
                    Dim i As Integer = New Random().Next(0, oLista.Count - 1)

                    Dim az As Azienda = ss1.LoadObjByPK(Of Azienda)(oLista(i).IdAzienda)

                Next

                Dim dtEnd As DateTime = DateTime.Now

                Me.WriteLog("Elaps: {0}", dtEnd.Subtract(dtStart).TotalMilliseconds)
                Me.WriteLog("Select: {0}", ss1.DB.Stats.GetCounter(Bdo.Database.DBStats.EStatement.Select))

            End Using
        Catch ex As Exception

            Me.WriteLog("ECCEZIONE! Tipo: {0} - {1}", ex.GetType().Name, ex.Message)
            Me.WriteLog("{0}", ex.StackTrace)
        End Try
    End Sub


    Private Sub TestMultiThreadCacheToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TestMultiThreadCacheToolStripMenuItem.Click
        Dim N As Integer = 10
        Try
            Using ss1 = Me.CreateSlot()

                Dim oProv As Provincia = ss1.LoadObjByPK(Of Provincia)("RM")

                Dim dtStart As DateTime = DateTime.Now

                For index As Integer = 1 To N
                    System.Threading.ThreadPool.QueueUserWorkItem(New Threading.WaitCallback(AddressOf Me.ThreadTestProvincia), ss1)
                Next


                Dim dtEnd As DateTime = DateTime.Now

                Me.WriteLog("Elaps: {0}", dtEnd.Subtract(dtStart).TotalMilliseconds)
                Me.WriteLog("Select: {0}", ss1.DB.Stats.GetCounter(Bdo.Database.DBStats.EStatement.Select))

            End Using

        Catch ex As Exception

            Me.WriteLog("ECCEZIONE! Tipo: {0} - {1}", ex.GetType().Name, ex.Message)
            Me.WriteLog("{0}", ex.StackTrace)
        End Try
    End Sub


    Private Sub ThreadTestProvincia(ByVal oSlotIn As Object)
        Dim ss1 As BusinessSlot = DirectCast(oSlotIn, BusinessSlot)

        For index As Integer = 1 To 2000
            Dim oProv2 As Provincia = ss1.LoadObjByPK(Of Provincia)("RM")
        Next

        Console.WriteLine("{0} - Finito", Threading.Thread.CurrentThread.ManagedThreadId)
    End Sub


    Private Sub TestFunzioniListaSumToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TestFunzioniListaSumToolStripMenuItem.Click
        Try
            Using ss1 = Me.CreateSlot()
                Dim dtStart As DateTime = DateTime.Now
                Dim oLista As ListaAziende = ss1.CreatePagedList(Of ListaAziende)(1, 100).CercaTutti()

                'ss1.SlotCacheForAnyEnabled = False
                'MAX
                Dim oAziendaMax As Azienda = oLista.Max("RagioneSociale")

                If oAziendaMax IsNot Nothing Then
                    Me.WriteLog("Trovata " & oAziendaMax.RagioneSociale)
                Else
                    Me.WriteLog("Non trovata")
                End If

                'MIN
                oAziendaMax = oLista.Min("RagioneSociale")

                If oAziendaMax IsNot Nothing Then
                    Me.WriteLog("Trovata " & oAziendaMax.RagioneSociale)
                Else
                    Me.WriteLog("Non trovata")
                End If

                'SUM
                Dim dSum As Double = oLista.Sum("CodSettore")
                Me.WriteLog("Sum: " & dSum.ToString("N0"))

                'AVG

                Dim dtEnd As DateTime = DateTime.Now
                dSum = oLista.Avg("CodSettore")
                Me.WriteLog("Avg: " & dSum.ToString("N2"))

                Me.WriteLog("Elaps: {0}", dtEnd.Subtract(dtStart).TotalMilliseconds)
                Me.WriteLog("Select: {0}", ss1.DB.Stats.GetCounter(Bdo.Database.DBStats.EStatement.Select))

            End Using


        Catch ex As Exception

            Me.WriteLog("ECCEZIONE! Tipo: {0} - {1}", ex.GetType().Name, ex.Message)
            Me.WriteLog("{0}", ex.StackTrace)
        End Try
    End Sub



    Private Sub BizCTORToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BizCTORToolStripMenuItem.Click
        Try
            Using ss1 = Me.CreateSlot()
                Dim az As Azienda = ss1.LoadObjByPK(Of Azienda)("FAS287")
                Dim aziBiz As AziendaBiz
                Dim dtStart As DateTime = DateTime.Now

                Me.WriteLog("Prova1")

                For i As Integer = 1 To 4000000
                    aziBiz = New AziendaBiz2(az)

                Next

                Dim dtEnd As DateTime = DateTime.Now
                Me.WriteLog("Elaps: {0}", dtEnd.Subtract(dtStart).TotalMilliseconds)


                'Me.WriteLog("Prova2")
                'dtStart = DateTime.Now
                'For i As Integer = 1 To 4000000
                '    aziBiz = AziendaBiz.CreateSlow(Of AziendaBiz)(az)

                'Next

                'dtEnd = DateTime.Now

                'Me.WriteLog("Elaps: {0}", dtEnd.Subtract(dtStart).TotalMilliseconds)

                Me.WriteLog("Select: {0}", ss1.DB.Stats.GetCounter(Bdo.Database.DBStats.EStatement.Select))

            End Using


        Catch ex As Exception

            Me.WriteLog("ECCEZIONE! Tipo: {0} - {1}", ex.GetType().Name, ex.Message)
            Me.WriteLog("{0}", ex.StackTrace)
        End Try
    End Sub

    Private Sub PasswordGeneratorToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PasswordGeneratorToolStripMenuItem.Click
        Dim sPass = Bdo.Utils.PasswordGen.Generate("UUNNSLLS")
        Me.WriteLog("Password: {0}", sPass)
        sPass = Bdo.Utils.PasswordGen.Generate("UUUUU")
        Me.WriteLog("Password: {0}", sPass)
        sPass = Bdo.Utils.PasswordGen.Generate("LLLLL")
        Me.WriteLog("Password: {0}", sPass)
        sPass = Bdo.Utils.PasswordGen.Generate("NNLLNNSS")
        Me.WriteLog("Password: {0}", sPass)
        sPass = Bdo.Utils.PasswordGen.Generate("UUNNSLLS")
        Me.WriteLog("Password: {0}", sPass)
        sPass = Bdo.Utils.PasswordGen.Generate("ULAAAANN")
        Me.WriteLog("Password: {0}", sPass)
    End Sub


    Private Sub InviEmailToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles InviEmailToolStripMenuItem.Click
        Try

            Dim oMailer As New Bdo.Utils.Mailer("mail.sds.it", 5025, "simone.pelaia@sds.it", "volley2")
            oMailer.Send("", "simone.pelaia@sds.it", "", "", "Test mio", "Ciao", "", Nothing)
            Me.WriteLog("Mail inviata")

        Catch ex As Exception

            Me.WriteLog("ECCEZIONE! Tipo: {0} - {1}", ex.GetType().Name, ex.Message)
            Me.WriteLog("{0}", ex.StackTrace)
        End Try
    End Sub

    Private Sub SQL2005PagToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SQL2005PagToolStripMenuItem.Click
        Try
            Dim dtStart As DateTime

            Using ss1 As BusinessSlot = New BusinessSlot("MSSQLDataBase", "Server=pre-mssql.pws.local;database=MF_BACKOFFICE;Trusted_Connection=True")

                dtStart = Date.Now
                ss1.DB.SQL = "       SeLeCT DISTINCT * from Convenzioni order by codice desc"
                Dim oTab As DataTable = ss1.DB.Select(0, 100)

                Me.WriteLog("avvio: {0}", Date.Now.Subtract(dtStart))

            End Using

            Using ss1 As BusinessSlot = New BusinessSlot("MSSQL2012DataBase", "Server=pre-mssql.pws.local;database=MF_BACKOFFICE;Trusted_Connection=True")

                dtStart = Date.Now
                ss1.DB.SQL = "       SeLeCT DISTINCT * from Convenzioni order by codice desc"
                Dim oTab As DataTable = ss1.DB.Select(0, 100)

                Me.WriteLog("2012: {0}", Date.Now.Subtract(dtStart))

            End Using

            Using ss1 As BusinessSlot = New BusinessSlot("MSSQL2005DataBase", "Server=pre-mssql.pws.local;database=MF_BACKOFFICE;Trusted_Connection=True")

                dtStart = Date.Now
                ss1.DB.SQL = "       SeLeCT DISTINCT * from Convenzioni order by codice desc"
                Dim oTab As DataTable = ss1.DB.Select(0, 100)

                Me.WriteLog("2005: {0}", Date.Now.Subtract(dtStart))

            End Using

            Using ss1 As BusinessSlot = New BusinessSlot("MSSQLDataBase", "Server=pre-mssql.pws.local;database=MF_BACKOFFICE;Trusted_Connection=True")

                dtStart = Date.Now
                ss1.DB.SQL = "SeLeCT DISTINCT * from Convenzioni order by codice desc"
                Dim oTab As DataTable = ss1.DB.Select(0, 100)

                Me.WriteLog("2000: {0}", Date.Now.Subtract(dtStart))

            End Using


        Catch ex As Exception

            Me.WriteLog("ECCEZIONE! Tipo: {0} - {1}", ex.GetType().Name, ex.Message)
            Me.WriteLog("{0}", ex.StackTrace)
        End Try
    End Sub

    Private Sub CloneObjectToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CloneObjectToolStripMenuItem.Click
        Try
            Dim dtStart As DateTime
            Using ss1 = Me.CreateSlot()

                dtStart = Date.Now

                Dim a1 As Provincia = ss1.LoadObjByPK(Of Provincia)("RM")
                Dim a2 As Provincia = ss1.CloneObject(Of Provincia)(a1)

                Me.WriteLog("2005: {0}", Date.Now.Subtract(dtStart))

            End Using




        Catch ex As Exception

            Me.WriteLog("ECCEZIONE! Tipo: {0} - {1}", ex.GetType().Name, ex.Message)
            Me.WriteLog("{0}", ex.StackTrace)
        End Try

    End Sub

    Private Sub SumConFiltroToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SumConFiltroToolStripMenuItem.Click
        Try
            Dim dtStart As DateTime
            Using ss1 = Me.CreateSlot()

                dtStart = Date.Now

                'Dim oLista As ListaAziende = ss1.CreatePagedList(Of ListaAziende)(1, 40).SearchAllObjects()
                Dim oLista As ListaAziende = ss1.CreatePagedList(Of ListaAziende)(1, 40).SearchByColumn(New Filter("IdAzienda", EOperator.GreaterThan, "FAS286").And("IdAzienda", EOperator.LessThan, "NNNNNN"))

                Me.WriteLog("Sum: {0}", oLista.Sum("Disabilitata", New Filter("Disabilitata", EOperator.Equal, 0)))
                Me.WriteLog("Sum: {0}", oLista.Sum("Disabilitata", New Filter("Disabilitata", EOperator.Differs, 0)))
                Me.WriteLog("Sum: {0}", oLista.Sum("Disabilitata", New Filter("Disabilitata", EOperator.GreaterThan, 0)))
                Me.WriteLog("Sum: {0}", oLista.Sum("Disabilitata", New Filter("Disabilitata", EOperator.GreaterEquals, 0)))
                Me.WriteLog("Sum: {0}", oLista.Sum("Disabilitata", New Filter("Disabilitata", EOperator.LessThan, 1)))
                Me.WriteLog("Sum: {0}", oLista.Sum("Disabilitata", New Filter("Disabilitata", EOperator.LessEquals, 1)))

                Me.WriteLog("Elaps: {0}", Date.Now.Subtract(dtStart))

            End Using




        Catch ex As Exception

            Me.WriteLog("ECCEZIONE! Tipo: {0} - {1}", ex.GetType().Name, ex.Message)
            Me.WriteLog("{0}", ex.StackTrace)
        End Try
    End Sub

    Private Sub SerializeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SerializeToolStripMenuItem.Click
        Try
            Dim dtStart As DateTime
            Using ss1 = Me.CreateSlot()

                dtStart = Date.Now

                'Dim oAz As Azienda = ss1.CreateObject(Of Azienda)()
                Dim oAz As Azienda = ss1.LoadObjByPK(Of Azienda)("FAS287")


                Dim oBuff() As Byte = ss1.BinSerialize(oAz)

                Dim oAz2 As Azienda = ss1.BinDeserialize(Of Azienda)(oBuff)

                Me.WriteLog("{0}", oAz2.ProvinciaLeg)
                Me.WriteLog("Elaps: {0}", Date.Now.Subtract(dtStart))

            End Using




        Catch ex As Exception

            Me.WriteLog("ECCEZIONE! Tipo: {0} - {1}", ex.GetType().Name, ex.Message)
            Me.WriteLog("{0}", ex.StackTrace)
        End Try
    End Sub

    Private Sub ProvaConnessioneMultiplaToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ProvaConnessioneMultiplaToolStripMenuItem.Click
        Try
            Using ss1 = Me.CreateSlot()
                ss1.DbAdd("CONN2", "MYSQLDataBase", "data source=fetest01;database=faschimcollaudo;userid=root;password=!av007aM")
                Dim dtStart As DateTime = DateTime.Now
                Dim az As Azienda2 = ss1.LoadObjByPK(Of Azienda2)("FAS287")
                Dim pv As Provincia2 = az.Provincia
                Dim rg As Regione = az.Provincia.Regione

                Dim dtEnd As DateTime = DateTime.Now

                Me.WriteLog("Elaps: {0}", dtEnd.Subtract(dtStart).TotalMilliseconds)
                Me.WriteLog("Select: {0}", ss1.DB.Stats.GetCounter(Bdo.Database.DBStats.EStatement.Select))
                Me.WriteLog("Select: {0}", ss1.DbGetStatsAll().GetCounter(Bdo.Database.DBStats.EStatement.Select))

            End Using
        Catch ex As Exception

            Me.WriteLog("ECCEZIONE! Tipo: {0} - {1}", ex.GetType().Name, ex.Message)
            Me.WriteLog("{0}", ex.StackTrace)
        End Try

    End Sub

    Private Sub NuovaListaToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NuovaListaToolStripMenuItem.Click
        Try
            Using ss1 = Me.CreateSlot()
                ss1.DbAdd("CONN2", "MYSQLDataBase", "data source=fetest01;database=faschimcollaudo;userid=root;password=!av007aM")
                Dim dtStart As DateTime = DateTime.Now

                Dim az As Azienda = ss1.CreateObject(Of Azienda)()
                az.IdAzienda = "FAS287"

                Dim az2 As Azienda = ss1.CreateObject(Of Azienda)()
                az2.IdAzienda = "FASAAA"

                Dim oLstAz As ListaAziende = ss1.CreateList(Of ListaAziende)()

                oLstAz.Add(az)
                oLstAz.Add(az)
                oLstAz.Add(az)
                oLstAz.Insert(2, az2)
                oLstAz.Add(az)
                oLstAz.Add(az)

                Dim azF As Azienda = oLstAz.FindByPK("FASAAA")

                Dim dtEnd As DateTime = DateTime.Now

                Me.WriteLog("Elaps: {0}", dtEnd.Subtract(dtStart).TotalMilliseconds)
                Me.WriteLog("Select: {0}", ss1.DB.Stats.GetCounter(Bdo.Database.DBStats.EStatement.Select))
                Me.WriteLog("Select: {0}", ss1.DbGetStatsAll().GetCounter(Bdo.Database.DBStats.EStatement.Select))

            End Using
        Catch ex As Exception

            Me.WriteLog("ECCEZIONE! Tipo: {0} - {1}", ex.GetType().Name, ex.Message)
            Me.WriteLog("{0}", ex.StackTrace)
        End Try
    End Sub

    Private Sub ConfrontoToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ConfrontoToolStripMenuItem.Click

        Dim i As Integer = 0
        Dim iPAGE As Integer = 2
        Dim iTOT As Integer = 400

        Try
            Using ss1 = Me.CreateSlot()

                ss1.LiveTrackingEnabled = False
                ss1.DB.AutoCloseConnection = True
                Dim oSlot As New VecchieClassi.BusinessSlot()
                Dim sw As New Stopwatch()
                'ss1.SetSlotCacheSize(2048)
                For k As Integer = 1 To 2


                    '1) ACCESSI SENZA DB
                    Me.WriteLog("1) ACCESSI SENZA DB")
                    'VECCHIE
                    sw.Reset()
                    sw.Start()
                    For index As Integer = 1 To iTOT
                        Dim oIp As New VecchieClassi.IscrittiPerPolizza(oSlot)
                        Dim s As String = oIp.DataInserimento.ToString()
                    Next
                    sw.Stop()
                    Me.WriteLog("Elaps VECCHIE: {0}", sw.ElapsedTicks)

                    'NUOVE
                    sw.Reset()
                    sw.Start()

                    For index As Integer = 1 To iTOT
                        Dim oIp As IscrittoPerPolizza = ss1.CreateObject(Of IscrittoPerPolizza)()
                        Dim s As String = oIp.DataInserimento.ToString()
                    Next
                    sw.Stop()
                    Me.WriteLog("Elaps NUOVE: {0}", sw.ElapsedTicks)

                    '2) ACCESSI DB
                    Me.WriteLog("2) ACCESSI CON DB")
                    'VECCHIE
                    sw.Reset()
                    sw.Start()
                    Dim oListIp As New VecchieClassi.ListaIscrittiPerPolizza(oSlot)
                    oListIp.Pagina = iPAGE
                    oListIp.Offset = iTOT
                    oListIp.CercaPerTest()

                    For Each oIp As VecchieClassi.IscrittiPerPolizza In oListIp.localElenco
                        Dim oPol As VecchieClassi.Polizza = oIp.Polizza
                        Dim oAnag As VecchieClassi.AnagraficaNew = oIp.Anagrafica
                        Dim oProv As VecchieClassi.Provincia = oIp.Anagrafica.Provincia
                        Dim oStato As VecchieClassi.StatoIscritto = oIp.StatoIscritto
                    Next

                    sw.Stop()
                    Me.WriteLog("Elaps VECCHIE: {0}", sw.ElapsedMilliseconds)

                    'NUOVE
                    sw.Reset()
                    sw.Start()

                    Dim nListIp As ListaIscrittiPerPolizza = ss1.CreatePagedList(Of ListaIscrittiPerPolizza)(iPAGE, iTOT).SearchAllObjects()

                    For Each oIp As IscrittoPerPolizza In nListIp
                        Dim oPol As Polizza = oIp.Polizza
                        Dim oAnag As Anagrafica = oIp.Anagrafica
                        Dim oProv As Provincia = oIp.Anagrafica.Provincia
                        Dim oStato As StatoIscritto = oIp.StatoIscritto

                    Next

                    sw.Stop()

                    Me.WriteLog("Elaps NUOVE: {0}", sw.ElapsedMilliseconds)
                    Me.WriteLog("Select: {0}", ss1.DbGetStatsAll().GetCounter(Bdo.Database.DBStats.EStatement.Select))
                Next

                Me.WriteLog(ss1.PrintInfo())
            End Using
        Catch ex As Exception

            Me.WriteLog("ECCEZIONE! Tipo: {0} - {1}", ex.GetType().Name, ex.Message)
            Me.WriteLog("{0}", ex.StackTrace)
        End Try
    End Sub



    Private Sub DictionaryMapToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DictionaryMapToolStripMenuItem.Click
        Dim i As Integer = 0
        Dim iPAGE As Integer = 2
        Dim iTOT As Integer = 50
        Try
            Using ss1 = Me.CreateSlot()
                Dim oSlot As New VecchieClassi.BusinessSlot()
                Dim dtStart As DateTime = DateTime.Now
                Dim dtEnd As DateTime = DateTime.Now

                dtStart = DateTime.Now

                'Codice QUI
                Dim oMap As New Bdo.Utils.DictionaryMap(Of String, Azienda)

                oMap.Add("AAAA", Nothing)
                oMap.Add("AAAA", Nothing)
                oMap.Add("AAAA", Nothing)

                oMap.RemoveAt("AAAA", 1)
                oMap.RemoveAt("AAAA", 1)

                Me.WriteLog("Keys: {0}", oMap.Count)
                Me.WriteLog("Values Key 1: {0}", oMap.CountValues("AAAA"))

                For Each key As String In oMap.Keys
                    Me.WriteLog("Value: {0}", oMap(key))
                Next

                dtEnd = DateTime.Now

                Me.WriteLog("Elaps: {0}", dtEnd.Subtract(dtStart).TotalMilliseconds)
                Me.WriteLog("Select: {0}", ss1.DB.Stats.GetCounter(Bdo.Database.DBStats.EStatement.Select))

            End Using
        Catch ex As Exception

            Me.WriteLog("ECCEZIONE! Tipo: {0} - {1}", ex.GetType().Name, ex.Message)
            Me.WriteLog("{0}", ex.StackTrace)
        End Try
    End Sub

    Private Sub OperazioniListeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OperazioniListeToolStripMenuItem.Click
        Try
            Dim dtStart As DateTime
            Using ss1 = Me.CreateSlot()

                dtStart = Date.Now

                'Dim oLista As ListaAziende = ss1.CreatePagedList(Of ListaAziende)(1, 40).SearchAllObjects()
                Dim oLista As ListaAziende = ss1.CreatePagedList(Of ListaAziende)(1, 100).SearchByColumn(New Filter("IdAzienda", EOperator.GreaterThan, "FAS286"))
                Dim oLista2 As ListaAziende = ss1.CreatePagedList(Of ListaAziende)(1, 20).SearchByColumn(New Filter("IdAzienda", EOperator.LessThan, "FAS286"))

                Dim oListaRet As ListaAziende = oLista.Union(oLista2)
                Me.WriteLog("Count Union: {0}", oListaRet.Count)

                Dim oListaRet2 As ListaAziende = oLista.UnionAll(oLista2)
                Me.WriteLog("Count UnionAll: {0}", oListaRet2.Count)

                Dim oListaRet3 As ListaAziende = oLista.Diff(oLista2)
                Me.WriteLog("Count Diff: {0}", oListaRet3.Count)

                Dim olistatestdiff As ListaAziende = ss1.CreateList(Of ListaAziende)()
                olistatestdiff.Add(oLista(0))

                Dim oListaRetV As ListaAziende = oLista.Diff(olistatestdiff)
                Me.WriteLog("Count Diff2: {0}", oListaRetV.Count)

                Me.WriteLog("Item 1 diff: {0}", oListaRet3(0))
                'Me.WriteLog("Item 2 Extra: {0}", oLista2.GetExtraValue(1, "NUM"))

                Dim oListaRet4 As ListaAziende = oListaRet3.Clone()
                Me.WriteLog("Count Clone: {0}", oListaRet3.Count)

                oListaRet4(0).CAP = "12345"
                oListaRet4(0).Fax = "12345"

                Me.WriteLog("ID Modificato: {0}", oListaRet4(0).IsChanged("IdAzienda"))
                Me.WriteLog("CAP Modificato: {0}", oListaRet4(0).IsChanged("CAP"))
                Me.WriteLog("Prop Modificate: {0}", oListaRet4(0).GetCurrentChanges().Count)

                Me.WriteLog("Elaps: {0}", Date.Now.Subtract(dtStart))
                Me.WriteLog("Select: {0}", ss1.DB.Stats.GetCounter(Bdo.Database.DBStats.EStatement.Select))

                ss1.SaveAll(oListaRet4)

            End Using




        Catch ex As Exception

            Me.WriteLog("ECCEZIONE! Tipo: {0} - {1}", ex.GetType().Name, ex.Message)
            Me.WriteLog("{0}", ex.StackTrace)
        End Try
    End Sub

    Private Sub ChiaveRicercaOperatoreToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ChiaveRicercaOperatoreToolStripMenuItem.Click
        Try
            Using ss1 = Me.CreateSlot()
                ss1.DbAdd("CONN2", "MYSQLDataBase", "data source=fetest01;database=faschimcollaudo;userid=root;password=!av007aM")
                Dim dtStart As DateTime = DateTime.Now

                Dim az As Azienda = ss1.LoadObjByKEY(Of Azienda)("CAP", "22000")

                Me.WriteLog("Azinda: {0} - {1}", az.RagioneSociale, az.CAP)

                Dim dtEnd As DateTime = DateTime.Now

                Me.WriteLog("Elaps: {0}", dtEnd.Subtract(dtStart).TotalMilliseconds)
                Me.WriteLog("Select: {0}", ss1.DB.Stats.GetCounter(Bdo.Database.DBStats.EStatement.Select))
                Me.WriteLog("Select: {0}", ss1.DbGetStatsAll().GetCounter(Bdo.Database.DBStats.EStatement.Select))

            End Using
        Catch ex As Exception

            Me.WriteLog("ECCEZIONE! Tipo: {0} - {1}", ex.GetType().Name, ex.Message)
            Me.WriteLog("{0}", ex.StackTrace)
        End Try
    End Sub


    Private Sub ChiaviMultipleInCacheToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles ChiaviMultipleInCacheToolStripMenuItem.Click
        Using ss1 = Me.CreateSlot()
            Dim dtStart As DateTime = DateTime.Now


            Dim az As Azienda = ss1.LoadObjByPK(Of Azienda)("FAS287")

            Me.WriteLog("Azienda: {0} - {1}", az.RagioneSociale, az.CAP)

            az = ss1.LoadObjNullByKEY(Of Azienda)("CODICEFISCALE", "97358180152")

            Dim dtEnd As DateTime = DateTime.Now

            Me.WriteLog("Elaps: {0}", dtEnd.Subtract(dtStart).TotalMilliseconds)
            Me.WriteLog("Select: {0}", ss1.DbGetStatsAll().GetCounter(Bdo.Database.DBStats.EStatement.Select))

            Me.WriteLog(ss1.PrintCacheDebug())


        End Using
    End Sub

    Private Sub Query2005ToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles Query2005ToolStripMenuItem.Click
        Dim s As String = "SELECT  UT.Id AS Id FROM Utenti UT LEFT JOIN Dispositivi DI ON DI.CodUtente = UT.Id LEFT JOIN Dispositivi DI2 ON DI2.CodUtente = UT.Id AND DI2.id > DI.id WHERE DI2.id IS NULL AND DI.CodStato = 3"

        Using ss1 As BusinessSlot = New BusinessSlot("MYFASI")
            Dim dtStart As DateTime = DateTime.Now

            ss1.DB.SQL = s
            Dim tab As DataTable = ss1.DB.Select(0, 12)

            Me.WriteLog("Elaps: {0}", DateTime.Now.Subtract(dtStart).TotalMilliseconds)
            Me.WriteLog("Select: {0}", ss1.DbGetStatsAll().GetCounter(Bdo.Database.DBStats.EStatement.Select))



        End Using

    End Sub

    Private Sub TestParecchiOggettiToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles TestParecchiOggettiToolStripMenuItem.Click
        Dim iMemInit As Long = Process.GetCurrentProcess().WorkingSet64

        Using ss1 = Me.CreateSlot()
            ss1.DB.AutoCloseConnection = False
            Dim sw As New Stopwatch()
            Const I_NUM_CICLI As Integer = 5
            Const I_NUM_OGGETTI As Integer = 1000
            'Forza caricamento classi
            Dim o As Azienda = ss1.CreateObject(Of Azienda)()
            Dim rnd As New Random(Date.Now.Millisecond)
            'Inizia
            sw.Start()

            Dim tsProc As TimeSpan = Process.GetCurrentProcess().TotalProcessorTime()

            For i As Integer = 1 To I_NUM_CICLI
                Me.WriteLog("Inizio ciclo {0}", i)
                Dim lst As ListaAziende = ss1.CreatePagedList(Of ListaAziende)(rnd.Next(1, 2), I_NUM_OGGETTI).
                    SearchByColumn(New Filter("IdAzienda", EOperator.Differs, "FAS287").And("CodSettore", EOperator.LessThan, 99).And("Stato", EOperator.Equal, 1))

                For k As Integer = 0 To lst.Count - 1

                    lst(k).RagioneSociale = "aaa"

                Next

                'lst.Clear()
                'GC.Collect()
                'GC.WaitForPendingFinalizers()
                'ss1.ObjectLiveTrackingDeadScan(True)

            Next

            tsProc = Process.GetCurrentProcess().TotalProcessorTime().Subtract(tsProc)

            sw.Stop()

            Me.WriteLog("Elaps: {0}", sw.Elapsed)
            Me.WriteLog("Elaps PROC: {0}", tsProc)
            Me.WriteLog("Mem Init: {0}", iMemInit)
            Me.WriteLog("Mem End: {0}", Process.GetCurrentProcess().WorkingSet64)
            Me.WriteLog("Select: {0}", ss1.DbGetStatsAll().GetCounter(Bdo.Database.DBStats.EStatement.Select))

        End Using
    End Sub


    Private Sub TrackingONToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles TrackingONToolStripMenuItem.Click
        Using ss1 As BusinessSlot = New BusinessSlot("CASA")
            ss1.LiveTrackingEnabled = True

            Dim az As Azienda = ss1.LoadObjByPK(Of Azienda)("FAS287")
            az = Nothing
            GC.Collect()
            GC.WaitForPendingFinalizers()
            Dim az1 As Azienda = ss1.LoadObjByPK(Of Azienda)("FAS287")
            Dim az2 As Azienda = ss1.LoadObjByPK(Of Azienda)("FAS287")

            Me.WriteLog("Reference: {0}", Object.ReferenceEquals(az, az1))
            Me.WriteLog("Reference: {0}", Object.ReferenceEquals(az1, az2))


            Me.WriteLog("Select: {0}", ss1.DbGetStatsAll().GetCounter(Bdo.Database.DBStats.EStatement.Select))

            Me.WriteLog(ss1.PrintCacheDebug())


        End Using
    End Sub

    Private Sub BoxingToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles BoxingToolStripMenuItem.Click
        Dim i As Integer = 5
        Dim o As Object = i
        Dim o2 As Object = o
        o = 6

        Me.WriteLog("i: " & i.ToString())
        Me.WriteLog("o: " & o.ToString())
        Me.WriteLog("o2: " & o.ToString())

        Using ss1 = Me.CreateSlot()

            Dim az As Azienda = ss1.LoadObjByPK(Of Azienda)("FAS287")
            Dim az2 As Azienda = az
            az.RagioneSociale = "Mamma"
            az = ss1.LoadObjByPK(Of Azienda)("AVE607")
            Me.WriteLog("Azienda: {0}", az.RagioneSociale)
            Me.WriteLog("Azienda: {0}", az2.RagioneSociale)


        End Using

    End Sub

    Private Sub DistinctGroupToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles DistinctGroupToolStripMenuItem.Click
        Using ss1 = Me.CreateSlot()

            ss1.ChangeTrackingEnabled = True

            Dim lstAziende As ListaAziende = ss1.CreatePagedList(Of ListaAziende)(1, 100).SearchAllObjects()


            Dim oDizGroup As GroupByResult(Of ListaAziende) = lstAziende.GroupByProperty("CAP")

            lstAziende(0).CAP = "00143"
            Me.WriteLog(lstAziende(0).IsChanged("CAP").ToString())
            Me.WriteLog(lstAziende(0).GetCurrentChanges().Count.ToString())

            Me.WriteLog(ss1.PrintInfo())
        End Using
    End Sub

    Private Sub EventiNEWToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles EventiNEWToolStripMenuItem.Click
        Using ss1 = Me.CreateSlot()

            ss1.ChangeTrackingEnabled = True
            ss1.EventManagerEnabled = True
            ss1.EventManager.RegisterPostEventHandlerForAny(BusinessSlot.EObjectEvent.Load, AddressOf Me.eventoAnyLOAD)

            ss1.EventManager.RegisterPostEventHandler(Of Azienda)(BusinessSlot.EObjectEvent.Load, DirectCast(AddressOf Me.eventoLOAD, BusinessSlot.BDEventPostHandler))
            ss1.EventManager.RegisterPostEventHandler(Of Azienda)(BusinessSlot.EObjectEvent.Load, DirectCast(AddressOf Me.eventoLOAD2, BusinessSlot.BDEventPostHandler))

            ss1.EventManager.RegisterPostEventHandler(Of Azienda)(BusinessSlot.EObjectEvent.Insert, DirectCast(AddressOf Me.eventoSAVE, BusinessSlot.BDEventPostHandler))
            ss1.EventManager.RegisterPreEventHandler(Of Azienda)(BusinessSlot.EObjectEvent.Update, DirectCast(AddressOf Me.eventoPreSAVE, BusinessSlot.BDEventPreHandler))

            Dim lstAziende As ListaAziende = ss1.CreatePagedList(Of ListaAziende)(1, 100).SearchAllObjects()

            Dim t As Type = GetType(Azienda)

            lstAziende(0).ProvinciaLeg = lstAziende(0).Provincia
            Me.WriteLog(lstAziende(0).IsChanged("ProvinciaLeg").ToString())
            Me.WriteLog(lstAziende(0).GetCurrentChanges().Count.ToString())

            ss1.SaveObject(lstAziende(0))

            Me.WriteLog(ss1.DebugObjectDump(lstAziende(0)))

            Me.WriteLog(ss1.PrintInfo())
        End Using
    End Sub

    Private Sub eventoAnyLOAD(o As Bdo.Objects.Base.DataObjectBase)
        Me.WriteLog("***** Any Loaded:" & o.GetType().Name & " ******")
    End Sub

    Private Sub eventoLOAD(az As Azienda)
        Me.WriteLog("***** AZ Loaded 1 ******")
    End Sub
    Private Sub eventoLOAD2(az As Azienda)
        Me.WriteLog("***** AZ Loaded 2 ******")
    End Sub

    Private Sub eventoPreSAVE(az As Azienda, ByRef cancel As Boolean)
        Me.WriteLog("***** AZ Pre Save ******")
        'cancel = True
    End Sub

    Private Sub eventoSAVE(az As Azienda)
        Me.WriteLog("***** AZ Saved " & az.ObjectSource & " ******")
    End Sub

    Private Sub LogDebugToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles LogDebugToolStripMenuItem.Click
        Using ss1 = Me.CreateSlot()
            ss1.DB.AutoCloseConnection = False
            Const I_NUM_CICLI As Integer = 5
            Const I_NUM_OGGETTI As Integer = 1000
            'Forza caricamento classi
            Dim o As Azienda = ss1.CreateObject(Of Azienda)()
            Dim rnd As New Random(Date.Now.Millisecond)
            'Inizia
            AddHandler ss1.OnLogDebugSent, AddressOf Me.slotDebug

            For i As Integer = 1 To I_NUM_CICLI
                Me.WriteLog("Inizio ciclo {0}", i)
                Dim lst As ListaAziende = ss1.CreatePagedList(Of ListaAziende)(rnd.Next(1, 2), I_NUM_OGGETTI).
                    SearchByColumn(New FilterDIFFERS("IdAzienda", "FAS287").And(New FilterIN("Stato", 1, 1)))

                For k As Integer = 0 To lst.Count - 1

                    lst(k).RagioneSociale = "aaa"
                    ss1.LogDebug("Caricata azienda {0}", lst(k).IdAzienda)
                    ss1.LogDebug(DebugLevel.Technical_1, "aaa ")

                Next


            Next

            Me.WriteLog("Elaps: {0}", ss1.GetCurrentElapsed())
            Me.WriteLog(ss1.PrintInfo())
        End Using
    End Sub

    Private Sub slotDebug(slot As BusinessSlot, level As DebugLevel, msg As String)

        Me.WriteLog(level.ToString() & " - " & msg)
    End Sub

    Private Sub ConfrontoBDOVsLINQToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles ConfrontoBDOVsLINQToolStripMenuItem.Click
        Using ss1 = Me.CreateSlot()
            ss1.DB.AutoCloseConnection = False
            ss1.LiveTrackingEnabled = False
            Dim ts As TimeSpan
            Const I_NUM_OGGETTI As Integer = 1000
            'Forza caricamento classi
            Dim o As Azienda = ss1.CreateObject(Of Azienda)()
            Dim rnd As New Random(Date.Now.Millisecond)
            'Inizia
            Dim lst As ListaAziende

            lst = ss1.CreatePagedList(Of ListaAziende)(1, I_NUM_OGGETTI).
                    SearchByColumn(New FilterDIFFERS("IdAzienda", "FAS287").And(New FilterIN("Stato", 1, 1)))


            ts = ss1.GetCurrentElapsed()
            Me.WriteLog("Sum-BDO: {0}", lst.Sum("Stato"))
            Me.WriteLog("Elaps: {0}", ss1.GetCurrentElapsed().Ticks - ts.Ticks)

            lst = ss1.CreatePagedList(Of ListaAziende)(1, I_NUM_OGGETTI).
                    SearchByColumn(New FilterDIFFERS("IdAzienda", "FAS287").And(New FilterIN("Stato", 1, 1)))

            ts = ss1.GetCurrentElapsed()
            Me.WriteLog("Avg-BDO: {0}", lst.Avg("Stato"))
            Me.WriteLog("Elaps: {0}", ss1.GetCurrentElapsed().Ticks - ts.Ticks)

            lst = ss1.CreatePagedList(Of ListaAziende)(1, I_NUM_OGGETTI).
                    SearchByColumn(New FilterDIFFERS("IdAzienda", "FAS287").And(New FilterIN("Stato", 1, 1)))

            ts = ss1.GetCurrentElapsed()
            Me.WriteLog("Sum-LNQ: {0}", lst.Sum(Function(az) az.Stato))
            Me.WriteLog("Elaps: {0}", ss1.GetCurrentElapsed().Ticks - ts.Ticks)

            lst = ss1.CreatePagedList(Of ListaAziende)(1, I_NUM_OGGETTI).
                    SearchByColumn(New FilterDIFFERS("IdAzienda", "FAS287").And(New FilterIN("Stato", 1, 1)))

            ts = ss1.GetCurrentElapsed()
            Me.WriteLog("Avg-LNQ: {0}", lst.Average(Function(az) az.Stato))
            Me.WriteLog("Elaps: {0}", ss1.GetCurrentElapsed().Ticks - ts.Ticks)

            Me.WriteLog("Elaps: {0}", ss1.GetCurrentElapsed())
            Me.WriteLog(ss1.PrintInfo())
        End Using
    End Sub

    Private Sub ObjectExtraDataToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles ObjectExtraDataToolStripMenuItem.Click
        Using ss1 = Me.CreateSlot()
            ss1.DB.AutoCloseConnection = False
            ss1.LiveTrackingEnabled = False
            'Forza caricamento classi
            Dim o As Azienda = ss1.CreateObject(Of Azienda)()
            Dim lst As ListaAziende

            lst = ss1.CreateList(Of ListaAziende)().
                    SearchByColumn(New FilterDIFFERS("IdAzienda", "FAS287").And(New FilterIN("Stato", 1, 1)))

            ss1.ExtraDataSet(o, "Prova", 5)
            ss1.ExtraDataSet(lst, "Prova", 7)


            Me.WriteLog("ExistO   : {0}", ss1.ExtraDataExist(o, "Prova"))
            Me.WriteLog("GetO     : {0}", ss1.ExtraDataGet(o, "Prova", 0))
            ss1.ExtraDataRemove(o, "Prova")
            Me.WriteLog("ExistO   : {0}", ss1.ExtraDataExist(o, "Prova"))

            Me.WriteLog("ExistL   : {0}", ss1.ExtraDataExist(lst, "Prova"))
            Me.WriteLog("GetL     : {0}", ss1.ExtraDataGet(lst, "Prova", 0))
            ss1.ExtraDataRemove(o, "Prova")
            Me.WriteLog("ExistL   : {0}", ss1.ExtraDataExist(lst, "Prova"))

            Me.WriteLog("Elaps: {0}", ss1.GetCurrentElapsed())
            Me.WriteLog(ss1.PrintInfo())
        End Using
    End Sub

    Private Sub LoadOrCreateToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles LoadOrCreateToolStripMenuItem.Click
        Using ss1 = Me.CreateSlot()
            ss1.DB.AutoCloseConnection = False
            ss1.LiveTrackingEnabled = False
            'Forza caricamento classi
            Dim o As Azienda = ss1.LoadObjOrNewByPK(Of Azienda)("AAAAA1")

            Dim o2 As IscrittoPerPolizza = ss1.LoadObjOrNewByPK(Of IscrittoPerPolizza)(-6565)

            Dim oBiz As AziendaBiz = ss1.BizNewWithLoadOrNewByPK(Of AziendaBiz)("AAAAA1")

            Me.WriteLog("Codice: {0}", o.IdAzienda)
            Me.WriteLog("Stato: {0}", o.ObjectState)

            Me.WriteLog("Elaps: {0}", ss1.GetCurrentElapsed())
            Me.WriteLog(ss1.PrintInfo())
        End Using
    End Sub

    Private Sub IndexOfToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles IndexOfToolStripMenuItem.Click
        Using ss1 = Me.CreateSlot()
            ss1.DB.AutoCloseConnection = False
            ss1.LiveTrackingEnabled = False
            Const I_NUM_OGGETTI As Integer = 50
            'Forza caricamento classi
            Dim o As Azienda = ss1.CreateObject(Of Azienda)()
            Dim rnd As New Random(Date.Now.Millisecond)
            'Inizia
            Dim lst As ListaAziende

            lst = ss1.CreatePagedList(Of ListaAziende)(1, I_NUM_OGGETTI).
                    SearchByColumn(New FilterDIFFERS("IdAzienda", "FAS287").And(New FilterIN("Stato", 1, 1)))

            Me.WriteLog("IndexOf: {0}", lst.IndexOfByPK("3MI783"))

            Me.WriteLog("Elaps: {0}", ss1.GetCurrentElapsed())
            Me.WriteLog(ss1.PrintInfo())
        End Using
    End Sub

    Private Sub NuoviValidatoriToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles NuoviValidatoriToolStripMenuItem.Click
        Using ss1 = Me.CreateSlot()
            ss1.DB.AutoCloseConnection = False
            ss1.LiveTrackingEnabled = False
            Const I_NUM_OGGETTI As Integer = 50
            'Forza caricamento classi
            'Inizia
            Dim lst As ListaAziende

            lst = ss1.CreatePagedList(Of ListaAziende)(1, I_NUM_OGGETTI).
                    SearchByColumn(New FilterDIFFERS("IdAzienda", "FAS287").And(New FilterIN("Stato", 1, 1)))

            'lst(0).RagioneSociale = "aaa"
            'ss1.SaveObject(lst(0))

            lst(0).RagioneSociale = "   aaaaaaaaaaaaaaaaaaaa"
            ss1.SaveObject(lst(0))

            Me.WriteLog("IndexOf: {0}", lst.IndexOfByPK("3MI783"))

            Me.WriteLog("Elaps: {0}", ss1.GetCurrentElapsed())
            Me.WriteLog(ss1.PrintInfo())
        End Using
    End Sub

    Private Sub FiltriSuOggettiMappatiConValorePKToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles FiltriSuOggettiMappatiConValorePKToolStripMenuItem.Click

        Using ss1 = Me.CreateSlot()
            ss1.DB.AutoCloseConnection = False
            ss1.LiveTrackingEnabled = False
            Const I_NUM_OGGETTI As Integer = 50
            'Forza caricamento classi
            'Inizia
            Dim lst As ListaAziende

            lst = ss1.CreatePagedList(Of ListaAziende)(1, I_NUM_OGGETTI).
                    SearchByColumn(New FilterDIFFERS("IdAzienda", "FAS287").And(New FilterIN("Stato", 1, 1)))

            'lst(0).RagioneSociale = "aaa"
            'ss1.SaveObject(lst(0))

            Dim lstFiltered As ListaAziende = lst.FindAllByPropertyFilter(New FilterEQUAL("Provincia", "RM"))

            Dim lstFiltered2 As ListaAziende = lst.FindAllByPropertyFilter(New FilterEQUAL("Provincia", New String() {"RM"}))

            Dim lstFiltered3 As ListaAziende = lst.FindAllByPropertyFilter(New FilterEQUAL("Provincia", ss1.LoadObjByPK(Of Provincia)("RM")))

            Dim lstFiltered4 As ListaAziende = lst.FindAllByPropertyFilter(New FilterEQUAL("Provincia", ss1.LoadObjByPK(Of StatoIscritto)(1)))

            Me.WriteLog("Elaps: {0}", ss1.GetCurrentElapsed())
            Me.WriteLog(ss1.PrintInfo())
        End Using

    End Sub

    Private Sub OggettiMappatiSuPropertyToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles OggettiMappatiSuPropertyToolStripMenuItem.Click

        Using ss1 = Me.CreateSlot()
            ss1.DB.AutoCloseConnection = False
            ss1.LiveTrackingEnabled = True
            ss1.DbPrefixKeys.Add("FASCHIMDM", "dev_faschim_demat.")

            Dim az1 As Azienda2 = ss1.LoadObjByPK(Of Azienda2)("FAR175")
            az1.CodProvinciaLeg = "MI"

            az1.Indirizzo += "1"

            ss1.SaveObject(az1)


            Dim oLst As ListaAziende2 = ss1.CreatePagedList(Of ListaAziende2)(1, 20).SearchAllObjects()

            Dim az2 As Azienda2 = ss1.LoadObjByPK(Of Azienda2)("FAS287")
            az2.CodProvinciaLeg = String.Empty

            Me.WriteLog("{0}", az2.Provincia)
            Me.WriteLog("{0}", az2.ProvinciaLeg)

            Me.WriteLog("Elaps: {0}", ss1.GetCurrentElapsed())
            Me.WriteLog(ss1.PrintInfo())
        End Using

    End Sub

    Private Sub ClonaSlotToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles ClonaSlotToolStripMenuItem.Click
        Using ss1 = Me.CreateSlot()
            ss1.DB.AutoCloseConnection = False
            ss1.LiveTrackingEnabled = True
            ss1.DbPrefixKeys.Add("FASCHIMDM", "dev_faschim_demat.")

            Dim ss2 As BusinessSlot = ss1.Clone()

            Me.WriteLog("DBKEYS: {0}", ss2.DbPrefixKeys.Count)
            Me.WriteLog("Elaps: {0}", ss1.GetCurrentElapsed())
            Me.WriteLog(ss1.PrintInfo())
        End Using
    End Sub

    Private Sub ListaBusinessToolStripMenuItem_Click2(sender As Object, e As EventArgs) 'Handles ListaBusinessToolStripMenuItem.Click
        Using ss1 = Me.CreateSlot()
            ss1.DB.AutoCloseConnection = False
            ss1.LiveTrackingEnabled = True
            ss1.DB.EnableTrace(New Bdo.Logging.ConsoleLogger(), False)

            Dim oLst As ListaAziende = ss1.CreatePagedList(Of ListaAziende)(1, 100).SearchAllObjects()

            Dim oBizList As New ListaAziendaBiz(oLst)

            Me.WriteLog("Azienda: {0}", oBizList(10).DataObj.RagioneSociale)

            Me.WriteLog("Elaps: {0}", ss1.GetCurrentElapsed())
            Me.WriteLog(ss1.PrintInfo())
        End Using
    End Sub


    Private Sub ListaBusinessToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ListaBusinessToolStripMenuItem.Click
        Using ss1 = Me.CreateSlot()
            ss1.DB.AutoCloseConnection = False
            ss1.LiveTrackingEnabled = True
            ss1.DB.EnableTrace(New Bdo.Logging.ConsoleLogger(), False)

            ss1.DbPrefixKeys.Add("key1", "simonedb....")
            ss1.DbPrefixKeys.Add("key2", "simonedb3")

            Dim oLst As ListaAziende = ss1.CreateList(Of ListaAziende)()
            Dim oAz As Azienda = ss1.CreateObject(Of Azienda)()

            oAz.RagioneSociale = "BBBBBB"
            oLst.Add(oAz)

            Dim oBizList As New ListaAziendaBiz(oLst)

            Me.WriteLog("Azienda: {0}", oBizList(0).DataObj.RagioneSociale)

            Me.WriteLog("Elaps: {0}", ss1.GetCurrentElapsed())
            Me.WriteLog(ss1.PrintInfo())
        End Using
    End Sub



    Private Sub FilterINObjToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles FilterINObjToolStripMenuItem.Click


        Using ss1 = Me.CreateSlot()
            ss1.DB.AutoCloseConnection = False
            ss1.LiveTrackingEnabled = True
            'ss1.DB.EnableTrace(New Bdo.Logging.ConsoleLogger(), False)

            Me.WriteLog("Begin")


            Dim oLst As ListaAziende = ss1.CreatePagedList(Of ListaAziende)(1, 100).SearchAllObjects()
            Me.WriteLog("Loaded")

            Me.WriteLog(oLst.FindAllByPropertyFilter(New FilterEQUAL("Provincia", "RM").Or(New FilterEQUAL("Provincia", "MI"))).Count.ToString())


            Me.WriteLog("Elaps: {0}", ss1.GetCurrentElapsed())
            Me.WriteLog(ss1.PrintInfo())
        End Using


    End Sub

    Private Sub DefaultValueToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DefaultValueToolStripMenuItem.Click


        Using ss1 = Me.CreateSlot()
            ss1.DB.AutoCloseConnection = False
            ss1.LiveTrackingEnabled = True
            'ss1.DB.EnableTrace(New Bdo.Logging.ConsoleLogger(), False)

            Dim pol As Polizza = ss1.CreateObject(Of Polizza)()

            Me.WriteLog(pol.codDurataPolizza.ToString())
            Me.WriteLog(pol.dataDecorrenza.ToString())
            'Me.WriteLog(pol.provadecimal)




            Me.WriteLog("Elaps: {0}", ss1.GetCurrentElapsed())
            Me.WriteLog(ss1.PrintInfo())
        End Using

    End Sub

    Private Sub BizObjSempliciToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BizObjSempliciToolStripMenuItem.Click
        Const I_CICLI As Integer = 1000000
        Using ss1 = Me.CreateSlot()
            ss1.DB.AutoCloseConnection = False
            ss1.LiveTrackingEnabled = True
            'ss1.DB.EnableTrace(New Bdo.Logging.ConsoleLogger(), False)
            Dim az As Azienda = ss1.CreateObject(Of Azienda)()
            Dim oAzBiz As AziendaBiz


            Dim sw2 As New Stopwatch()
            sw2.Start()
            For i As Integer = 1 To I_CICLI
                oAzBiz = ss1.BizNewWithCreateObj(Of AziendaBiz)()
            Next
            sw2.Stop()
            Me.WriteLog("EMIT: " & sw2.ElapsedTicks)

            Dim sw3 As New Stopwatch()
            sw3.Start()
            For i As Integer = 1 To I_CICLI
                oAzBiz = New AziendaBiz2(ss1.CreateObject(Of Azienda)())
            Next
            sw3.Stop()
            Me.WriteLog("REAL: " & sw3.ElapsedTicks)



            Me.WriteLog("Elaps: {0}", ss1.GetCurrentElapsed())
            Me.WriteLog(ss1.PrintInfo())
        End Using


    End Sub

    Private Sub XmlCustomToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles XmlCustomToolStripMenuItem.Click

        Using ss1 = Me.CreateSlot()
            ss1.DB.AutoCloseConnection = False
            ss1.LiveTrackingEnabled = True
            'ss1.DB.EnableTrace(New Bdo.Logging.ConsoleLogger(), False)
            ss1.XmlDataPagerFunction = AddressOf pagerFunctionXml

            Dim oLst As ListaAziende = ss1.CreatePagedList(Of ListaAziende)(1, 100).SearchAllObjects()


            Me.WriteLog(oLst.Pager.ToXml())




            Me.WriteLog("Elaps: {0}", ss1.GetCurrentElapsed())
            Me.WriteLog(ss1.PrintInfo())
        End Using


    End Sub


    Private Shared Sub pagerFunctionXml(pager As Bdo.Common.DataPager, xw As Bdo.Utils.XmlWrite)
        xw.WriteElementString("Prova", pager.Page.ToString())
    End Sub

    Private Sub PagedSubListToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PagedSubListToolStripMenuItem.Click

        Using ss1 = Me.CreateSlot()
            ss1.DB.AutoCloseConnection = False
            ss1.LiveTrackingEnabled = True
            'ss1.DB.EnableTrace(New Bdo.Logging.ConsoleLogger(), False)

            Dim oLst As ListaAziende = ss1.CreatePagedList(Of ListaAziende)(1, 100).SearchAllObjects()

            Dim oLstSub As ListaAziende = oLst.ToPagedList(2, 60)

            For Each oAz As Azienda In oLstSub
                Me.WriteLog(oAz.RagioneSociale)
            Next




            Me.WriteLog("Elaps: {0}", ss1.GetCurrentElapsed())
            Me.WriteLog(ss1.PrintInfo())
        End Using

    End Sub

    Private Sub BizNewOrCreateToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BizNewOrCreateToolStripMenuItem.Click

        Using ss1 = Me.CreateSlot()
            ss1.DB.AutoCloseConnection = False
            ss1.LiveTrackingEnabled = True
            'ss1.DB.EnableTrace(New Bdo.Logging.ConsoleLogger(), False)

            Dim azBiz As AziendaBiz = ss1.BizNewWithLoadOrNewByPK(Of AziendaBiz)("X1X1X!")

            Me.WriteLog(azBiz.DataObj.IdAzienda)


            Dim az2Biz As AziendaBiz = ss1.LoadObjByPK(Of Azienda)("FAS287").ToBizObject(Of AziendaBiz)()

            Me.WriteLog("Elaps: {0}", ss1.GetCurrentElapsed())
            Me.WriteLog(ss1.PrintInfo())
        End Using

    End Sub



    Private Sub TestGlobalCacheToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TestGlobalCacheToolStripMenuItem.Click

        Using ss1 = Me.CreateSlot()
            ss1.DB.AutoCloseConnection = True
            ss1.LiveTrackingEnabled = True

            Dim pv As Provincia = ss1.LoadObjByPK(Of Provincia)("RM")

            Me.WriteLog("Provincia {0}", pv.Descrizione)
            Me.WriteLog("Regione {0}", pv.Regione.Descrizione)
            Me.WriteLog("ss1 pv {0}", pv.GetSlot().ToString())
            Me.WriteLog("ss1 re {0}", pv.Regione.GetSlot().ToString())

            Using ss2 = Me.CreateSlot()
                ss2.DB.AutoCloseConnection = True
                ss2.LiveTrackingEnabled = True

                Dim pv2 As Provincia = ss2.LoadObjByPK(Of Provincia)("RM")
                Me.WriteLog("Provincia {0}", pv2.Descrizione)
                Me.WriteLog("Regione {0}", pv2.Regione.Descrizione)
                Me.WriteLog("ss2 pv {0}", pv2.GetSlot().ToString())
                Me.WriteLog("ss2 re {0}", pv2.Regione.GetSlot().ToString())

            End Using



            Me.WriteLog("Elaps: {0}", ss1.GetCurrentElapsed())
            Me.WriteLog(ss1.PrintInfo())
        End Using

    End Sub

    Private Sub TestCloneToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TestCloneToolStripMenuItem.Click


        Using ss1 = Me.CreateSlot()
            ss1.DB.AutoCloseConnection = True
            ss1.LiveTrackingEnabled = True

            Dim pv As Provincia = ss1.LoadObjByPK(Of Provincia)("RM")
            Me.WriteLog("Regione {0}", pv.Regione.Descrizione)

            Dim pv2 As Provincia = ss1.CloneObjectForNew(pv)
            Me.WriteLog("Regione {0}", pv2.Regione.Descrizione)



            Me.WriteLog("Elaps: {0}", ss1.GetCurrentElapsed())
            Me.WriteLog(ss1.PrintInfo())
        End Using


    End Sub

    Private Sub DELETEUPDATEToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DELETEUPDATEToolStripMenuItem.Click

        Using ss1 = Me.CreateSlot()
            ss1.DB.AutoCloseConnection = True
            ss1.LiveTrackingEnabled = True

            ss1.DB.BeginTransaction()
            Try
                Me.WriteLog("Begin Load")
                Dim oAnag As Anagrafica = ss1.LoadObjByPK(Of Anagrafica)("BOAL45")
                Me.WriteLog("Begin Update")
                oAnag.cap = "11111"
                ss1.SaveObject(oAnag)

                Me.WriteLog("Modifica OK")


                Me.WriteLog("Begin Delete")
                Dim oProv As ProvinciaWritable = ss1.LoadObjByKEY(Of ProvinciaWritable)(ProvinciaWritable.KEY_SIGLA, "99")

                ss1.DeleteObject(oProv)

                Me.WriteLog("Delete OK")

                Dim oProv2 As ProvinciaWritable = ss1.CreateObject(Of ProvinciaWritable)()
                oProv2.Id = 9999
                oProv2.DescrizioneProvincia = "PROVA"
                oProv2.IdProvincia = "XX"
                oProv2.CodRegione = 2

                ss1.SaveObject(oProv2)
                Me.WriteLog("Insert OK")

            Finally
                ss1.DB.RollbackTransaction()
            End Try




            Me.WriteLog("Elaps: {0}", ss1.GetCurrentElapsed())
            Me.WriteLog(ss1.PrintInfo())
        End Using


    End Sub

    Private Sub SQLSelectBuilderToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SQLSelectBuilderToolStripMenuItem.Click
        Using ss1 = Me.CreateSlot()
            ss1.DB.AutoCloseConnection = True
            ss1.LiveTrackingEnabled = True

            Dim oSql As New Bdo.Utils.SqlSelect()
            oSql.Fields("a.Id", "b.Codice") _
                .From("pippo a") _
                .InnerJoin("pluto b") _
                .On("b.Id=a.Codice") _
                .And("b.Nome= 'aaa'") _
                .Where("a.Num = 3") _
                .And("b.Nome= 'aaa'")

            Me.WriteLog(oSql.ToString())

            Me.WriteLog("Elaps: {0}", ss1.GetCurrentElapsed())
            Me.WriteLog(ss1.PrintInfo())
        End Using

    End Sub

    Private Sub TESTLiveTrackingConTRANToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TESTLiveTrackingConTRANToolStripMenuItem.Click
        Using ss1 = Me.CreateSlot()
            ss1.DB.AutoCloseConnection = True
            ss1.LiveTrackingEnabled = True

            Try
                Me.WriteLog("Begin Load")
                Dim oAnag As Anagrafica = ss1.LoadObjByPK(Of Anagrafica)("BOAL45")
                Me.WriteLog("Begin Update")
                oAnag.cap = "11111"
                ss1.SaveObject(oAnag)
                ss1.DB.BeginTransaction()

                Me.WriteLog("Inizio TRANS")

                Dim oAnag2 As Anagrafica = ss1.LoadObjByKEY(Of Anagrafica)(Anagrafica.KEY_CF, oAnag.codiceFiscale)


                Me.WriteLog("Refs: " & Object.ReferenceEquals(oAnag, oAnag2))
                Dim oAnag3 As Anagrafica = ss1.LoadObjByPK(Of Anagrafica)("BOAL45")
                Me.WriteLog("Refs: " & Object.ReferenceEquals(oAnag2, oAnag3))


            Finally
                If ss1.DB.IsInTransaction Then
                    ss1.DB.RollbackTransaction()
                End If
            End Try




            Me.WriteLog(ss1.PrintLiveTrackingDebug())
            Me.WriteLog("Elaps: {0}", ss1.GetCurrentElapsed())
            Me.WriteLog(ss1.PrintInfo())
        End Using

    End Sub

    Private Sub LISTAMAPPATAToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LISTAMAPPATAToolStripMenuItem.Click

        Using ss1 = Me.CreateSlot()
            ss1.DB.AutoCloseConnection = True
            ss1.LiveTrackingEnabled = True

            Try
                Me.WriteLog("Begin Load")

                Dim oAz As Azienda = ss1.LoadObjByPK(Of Azienda)("FAS287")

                Me.WriteLog("Polizze: {0}", oAz.Polizze.Count)
                Me.WriteLog("Polizza 1: {0}", oAz.Polizze.GetFirst().IdPolizza)
                Dim dto = oAz.ToDTO(5)
                Me.WriteLog("DTO: {0}", dto)

            Finally
            End Try




            Me.WriteLog("Elaps: {0}", ss1.GetCurrentElapsed())
            Me.WriteLog(ss1.PrintInfo())
        End Using

    End Sub

    Private Sub TransazioniPendentiToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TransazioniPendentiToolStripMenuItem.Click

        Using ss1 = Me.CreateSlot()
            ss1.DB.AutoCloseConnection = True
            ss1.LiveTrackingEnabled = True

            ss1.DB.BeginTransaction()
            Try

            Finally
                ss1.DB.RollbackTransaction()
            End Try

            Me.WriteLog("Num Begin: {0}", ss1.DB.Stats.GetCounter(Bdo.Database.DBStats.EStatement.Begin))

            ss1.DB.BeginTransaction()
            Try
                Dim oAz As Azienda = ss1.LoadObjByPK(Of Azienda)("FAS287")

            Finally
                ss1.DB.RollbackTransaction()
            End Try

            Me.WriteLog("Num Begin: {0}", ss1.DB.Stats.GetCounter(Bdo.Database.DBStats.EStatement.Begin))

            Me.WriteLog("Elaps: {0}", ss1.GetCurrentElapsed())
            Me.WriteLog(ss1.PrintInfo())
        End Using

    End Sub





    Private Sub DatalistYieldToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DatalistYieldToolStripMenuItem.Click
        Using ss1 = Me.CreateSlot()
            ss1.DB.AutoCloseConnection = True
            ss1.LiveTrackingEnabled = True
            ss1.ChangeTrackingEnabled = True

            Dim oLst As ListaPolizza = ss1.CreateList(Of ListaPolizza)()
            oLst.Add(ss1.CreateObject(Of Polizza)())
            oLst.Add(ss1.CreateObject(Of Polizza)())
            oLst.Add(ss1.CreateObject(Of Polizza)())
            Dim i As Integer = 0

            For Each item As Polizza In oLst
                Me.WriteLog("Item: {0}", item.ObjectRefId)
            Next


            For Index As Integer = 1 To 1000000000
                Dim item As Polizza = ss1.CreateObject(Of Polizza)()

                If (Index Mod 10000000) = 0 Then
                    Me.WriteLog("Count: {0}", Index)
                    Me.WriteLog("Item: {0}", item.ObjectRefId)
                End If
            Next





            Me.WriteLog("Elaps: {0}", ss1.GetCurrentElapsed())
            Me.WriteLog(ss1.PrintInfo())
        End Using

    End Sub

    Private Sub TestPKDateToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TestPKDateToolStripMenuItem.Click
        Using ss1 = Me.CreateSlot()
            ss1.DB.AutoCloseConnection = True
            ss1.LiveTrackingEnabled = True
            ss1.ChangeTrackingEnabled = True



            Dim oAnag = ss1.LoadObjByFILTER(Of Business.Data.Objects.TestClass.DAL.Anagrafica)(New FilterLIKE("Nome", "Mi%").And(New FilterGREATEREQ("DataInserimento", New Date(1998, 3, 4))))

            Me.WriteLog("RagSoc: {0}", oAnag.Cognome)

            Me.WriteLog("Elaps: {0}", ss1.GetCurrentElapsed())
            Me.WriteLog(ss1.PrintInfo())
        End Using
    End Sub

    Private Sub LogDebugNEWToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LogDebugNEWToolStripMenuItem.Click

        Using ss1 = Me.CreateSlot()
            ss1.DB.AutoCloseConnection = True
            ss1.LiveTrackingEnabled = True
            ss1.ChangeTrackingEnabled = True

            AddHandler ss1.OnLogDebugSent, AddressOf dolog

            ss1.LogDebug("Pippo")

            Dim ex As New Exception("dddd")

            ss1.LogDebugException(ex)

            Me.WriteLog(ss1.PrintInfo())
        End Using

    End Sub

    Private Sub dolog(slot As BusinessSlot, level As DebugLevel, message As String)
        Me.WriteLog(message)
    End Sub

    Private Sub Mailer365ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles Mailer365ToolStripMenuItem.Click
        Dim mailer As New Bdo.Utils.Mailer("smtp.office365.com", 587, "simone.pelaia@postewelfareservizi.it", "Volley99", True)

        mailer.SendAsync("simone.pelaia@postewelfareservizi.it", "simone.pelaia@postewelfareservizi.it", "", "", "subj", "body", Nothing, Nothing)
    End Sub

    Private Sub CastSuLoadObjOrNewToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CastSuLoadObjOrNewToolStripMenuItem.Click
        Using ss1 = Me.CreateSlot()
            ss1.DB.AutoCloseConnection = True
            ss1.LiveTrackingEnabled = True
            ss1.ChangeTrackingEnabled = True

            Dim reg = ss1.LoadObjOrNewByPK(Of Ordine)(200000)

            'reg.Regione = Nothing
            'reg.Regione = 2

            Me.WriteLog(reg.CodiceOrdine)

            Me.WriteLog(ss1.PrintInfo())
        End Using
    End Sub

    Private Sub NewFilterINToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NewFilterINToolStripMenuItem.Click
        Using ss1 = Me.CreateSlot()
            ss1.DB.AutoCloseConnection = True
            ss1.LiveTrackingEnabled = True
            ss1.ChangeTrackingEnabled = True

            Dim oList = ss1.CreateList(Of OrdineStatoLista)().SearchAllObjects()

            Dim olistF = oList.FindAllByPropertyFilter(New FilterIN(NameOf(OrdineStato.Id), 1, 3, 5, 7))

            Dim olistArray = oList.FindAllByPropertyFilter(New FilterIN(NameOf(OrdineStato.Id), New Integer() {1, 3}))



            Dim l = New List(Of String)
            l.Add("FI")
            l.Add("VI")
            l.Add("PA")


            Me.WriteLog(olistF.Count.ToString())
            Me.WriteLog(ss1.PrintInfo())
        End Using
    End Sub

    Private Sub ToJSONToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ToJSONToolStripMenuItem.Click
        Using ss1 = Me.CreateSlot()
            ss1.DB.AutoCloseConnection = True
            ss1.LiveTrackingEnabled = True
            ss1.ChangeTrackingEnabled = True

            Dim oAz = ss1.LoadObjByPK(Of Business.Data.Objects.TestClass.DAL.Anagrafica)(5000)
            'Me.WriteLog(oAz.ToJSON().Replace("{", "{{").Replace("}", "}}"))
            Me.WriteLog(oAz.ToBizObject(Of AnagraficaBiz).ToJSON().Replace("{", "{{").Replace("}", "}}"))

            Dim oList = ss1.CreateList(Of ListaProvince)().SearchAllObjects()

            Dim oProv = oList.Min("IdProvincia")

            Me.WriteLog(oProv.Descrizione)

            oProv = oList.Max("IdProvincia")

            Me.WriteLog(oProv.Descrizione)

            Me.WriteLog(ss1.PrintInfo())
        End Using
    End Sub

    Private Sub HashStringsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles HashStringsToolStripMenuItem.Click

        Using ss1 = Me.CreateSlot()
            ss1.DB.AutoCloseConnection = False
            Dim sw As New Stopwatch()
            Const I_NUM_CICLI As Integer = 5
            Const I_NUM_OGGETTI As Integer = 1000
            'Forza caricamento classi
            Dim o As Azienda = ss1.CreateObject(Of Azienda)()
            Dim rnd As New Random(Date.Now.Millisecond)
            'Inizia
            sw.Start()

            Dim tsProc As TimeSpan = Process.GetCurrentProcess().TotalProcessorTime()

            For i As Integer = 1 To I_NUM_CICLI
                Me.WriteLog("Inizio ciclo {0}", i)
                Dim lst As AnagraficaLista = ss1.CreatePagedList(Of AnagraficaLista)(rnd.Next(1, 2), I_NUM_OGGETTI).
                    SearchByColumn(New FilterDIFFERS(NameOf(Anagrafica.Cognome), "Rossi"))

                For k As Integer = 0 To lst.Count - 1

                    Me.WriteLog("Oggetto {0} - Hash: {1}", lst(k).ToString(), lst(k).GetHashBaseString())

                Next

                'lst.Clear()
                'GC.Collect()
                'GC.WaitForPendingFinalizers()
                'ss1.ObjectLiveTrackingDeadScan(True)

            Next

            tsProc = Process.GetCurrentProcess().TotalProcessorTime().Subtract(tsProc)

            sw.Stop()

            Me.WriteLog("Elaps: {0}", sw.Elapsed)
            Me.WriteLog("Elaps PROC: {0}", tsProc)
            Me.WriteLog("Select: {0}", ss1.DbGetStatsAll().GetCounter(Bdo.Database.DBStats.EStatement.Select))

        End Using
    End Sub

    Private Sub ListeCachedToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ListeCachedToolStripMenuItem.Click


    End Sub

    Private Sub EncryptedPropertyToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EncryptedPropertyToolStripMenuItem.Click

        Using ss1 = Me.CreateSlot()
            ss1.DB.AutoCloseConnection = True
            ss1.LiveTrackingEnabled = True
            ss1.ChangeTrackingEnabled = False


            ss1.PropertySet("KEYRSA", "Simone Pelaia")

            Dim oAz = ss1.LoadObjByPK(Of Azienda3)("FAS287")

            oAz.Indirizzo = oAz.Indirizzo

            ss1.SaveObject(oAz)

            Me.WriteLog(ss1.PrintCacheDebug())
        End Using

    End Sub

    Private Sub ListePrefetchToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ListePrefetchToolStripMenuItem.Click

        Using ss1 = Me.CreateSlotTest()
            ss1.DB.AutoCloseConnection = True
            ss1.LiveTrackingEnabled = True
            ss1.ChangeTrackingEnabled = True


            Dim oList = ss1.CreateList(Of OrdineStatoLista)().LoadFullObjects().SearchAllObjects()


            Me.WriteLog(oList.Count.ToString())
            Me.WriteLog(oList.ToXml())
            Me.WriteLog(ss1.PrintInfo())



            oList = ss1.CreateList(Of OrdineStatoLista)().LoadFullObjects().SearchAllObjects()

            Me.WriteLog(oList.Count.ToString())
            Me.WriteLog(oList.ToXml())
            'Me.WriteLog(ss1.PrintInfo())

            Me.WriteLog(ss1.PrintCacheDebug())
        End Using

    End Sub

    Private Sub ListeCachedToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ListeCachedToolStripMenuItem1.Click
        Using ss1 = Me.CreateSlotTest()
            ss1.DB.AutoCloseConnection = True
            ss1.LiveTrackingEnabled = True
            ss1.ChangeTrackingEnabled = True


            Dim oList = ss1.CreateList(Of OrdineStatoLista)().CacheResult().SearchAllObjects()


            Me.WriteLog(oList.Count.ToString())
            Me.WriteLog(oList.ToXml())
            Me.WriteLog(ss1.PrintInfo())




            oList = ss1.CreateList(Of OrdineStatoLista)().CacheResult().SearchByColumn(Filter.Neq(NameOf(OrdineStato.Id), 1000))

            oList = ss1.CreateList(Of OrdineStatoLista)().CacheResult().SearchByColumn(Filter.Neq(NameOf(OrdineStato.Id), 1000))


            oList = ss1.CreateList(Of OrdineStatoLista)().CacheResult().SearchByColumn(Filter.Neq(NameOf(OrdineStato.Id), 500))


            Me.WriteLog(oList.Count.ToString())
            Me.WriteLog(oList.ToXml())
            Me.WriteLog(ss1.PrintInfo())

            Me.WriteLog(ss1.PrintCacheDebug())
        End Using
    End Sub

    Private Sub NewPaginazioneMYSQLToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NewPaginazioneMYSQLToolStripMenuItem.Click

        Using ss1 = Me.CreateSlotTest()
            ss1.DB.AutoCloseConnection = True
            ss1.LiveTrackingEnabled = True
            ss1.ChangeTrackingEnabled = True


            Dim oList = ss1.CreatePagedList(Of AnagraficaLista)(1, 100).LoadFullObjects().SearchAllObjects()



            Me.WriteLog(oList.ToXml())
            Me.WriteLog(oList.Pager().ToXml())


            ss1.DB.SQL = "SELECT * from anagrafica"

            Dim oTab = ss1.DB.Select(1, 100)
            Me.WriteLog(oTab.Rows.Count.ToString())
            Me.WriteLog(ss1.DB.TotRecordQueryPaginata.ToString())


            Me.WriteLog(ss1.PrintInfo())


        End Using

    End Sub

    Private Sub BenchmarkLoadFullObjectsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BenchmarkLoadFullObjectsToolStripMenuItem.Click

        Using ss1 = Me.CreateSlotTest()
            ss1.DB.AutoCloseConnection = True
            ss1.LiveTrackingEnabled = True
            ss1.ChangeTrackingEnabled = True
            Me.WriteLog("Inizio Warmup")

            Dim oList = ss1.CreatePagedList(Of AnagraficaLista)(1, 1000).SearchAllObjects()
            Dim oList2 = ss1.CreatePagedList(Of AnagraficaLista)(1, 1000).LoadFullObjects().SearchAllObjects()

            Me.WriteLog("Fine Warmup")

        End Using

        Dim i_cicli As Integer = 5
        Dim i_items As Integer = 3000

        ' LoadFull
        Using ss1 = Me.CreateSlotTest()
            ss1.DB.AutoCloseConnection = False
            ss1.LiveTrackingEnabled = True
            ss1.ChangeTrackingEnabled = True

            Me.WriteLog("Avvio test LoadFullObjects")

            For i As Integer = 1 To i_cicli

                Me.WriteLog("Avvio ciclo {0}", i)
                Dim oList = ss1.CreatePagedList(Of AnagraficaLista)(i, i_items).LoadFullObjects().SearchAllObjects()

                For Each item In oList

                Next
                Me.WriteLog("Fine ciclo {0}", i)
                Me.WriteLog("Elapsed: {0}", ss1.GetCurrentElapsed())

            Next


            Me.WriteLog("Fine test LoadFullObjects")
            Me.WriteLog("Elapsed Totale: {0}", ss1.GetCurrentElapsed())
            Me.WriteLog(ss1.PrintInfo())
        End Using

        'Exit Sub

        ' Normale
        Using ss1 = Me.CreateSlotTest()
            ss1.DB.AutoCloseConnection = False
            ss1.LiveTrackingEnabled = True
            ss1.ChangeTrackingEnabled = True
            ss1.Conf.LoadFullObjects = True


            Me.WriteLog("Avvio test Standard")

            For i As Integer = 1 To i_cicli

                Me.WriteLog("Avvio ciclo {0}", i)
                Dim oList = ss1.CreatePagedList(Of AnagraficaLista)(i, i_items).SearchAllObjects()

                For Each item In oList

                Next
                Me.WriteLog("Fine ciclo {0}", i)
                Me.WriteLog("Elapsed: {0}", ss1.GetCurrentElapsed())

            Next


            Me.WriteLog("Fine test Standard")
            Me.WriteLog("Elapsed Totale: {0}", ss1.GetCurrentElapsed())
            Me.WriteLog(ss1.PrintInfo())
        End Using

    End Sub

    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        Me.txtLog.Clear()
    End Sub

    Private Sub CheckModificatoriStringheNULLToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CheckModificatoriStringheNULLToolStripMenuItem.Click

        Using ss1 = Me.CreateSlot()
            ss1.DB.AutoCloseConnection = True
            ss1.LiveTrackingEnabled = True
            ss1.ChangeTrackingEnabled = True

            Dim oAz = ss1.LoadObjByPK(Of Business.Data.Objects.TestClass.DAL.Anagrafica)(5000)
            'Me.WriteLog(oAz.ToJSON().Replace("{", "{{").Replace("}", "}}"))

            oAz.Nome = Nothing
            oAz.Nome = "Simone "

            Me.WriteLog(ss1.DebugObjectDump(oAz))

            Me.WriteLog(ss1.PrintInfo())
        End Using

    End Sub

    Private Sub LoopMT1ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LoopMT1ToolStripMenuItem.Click

        Const I_NUM_ITEMS As Integer = 1000
        Const I_NUM_SLICE As Integer = 1000

        Using ss1 = Me.CreateSlotTest()
            ss1.DB.AutoCloseConnection = True
            ss1.LiveTrackingEnabled = True
            ss1.ChangeTrackingEnabled = True
            Me.WriteLog("Inizio lettura {0} elementi", I_NUM_ITEMS)

            Dim oList = ss1.CreatePagedList(Of ListaAziende)(1, I_NUM_ITEMS).SearchAllObjects()

            ss1.LoopMT(Of ListaAziende)(oList, I_NUM_SLICE, 5, AddressOf elaboraBloccoLista)

        End Using


    End Sub

    Private Sub elaboraBloccoLista(slot As BusinessSlot, slice As ListaAziende)


        For Each anag In slice
            'Me.WriteLog("Sono: {0}", anag.RagioneSociale)
            Console.WriteLine(anag.RagioneSociale)
        Next


    End Sub


End Class
