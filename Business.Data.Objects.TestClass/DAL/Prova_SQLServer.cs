using Business.Data.Objects.Core.Attributes;
using Business.Data.Objects.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
namespace Business.Data.Objects.TestClass.DAL
{

    [Table("ANAGRAFICA_PF")]
    public abstract class ANAGRAFICA_PF : DataObject<ANAGRAFICA_PF>
    {

        [PrimaryKey]
        [AutoIncrement]
        public abstract Int32 ID { get; }

        [AcceptNull]
        [MaxLength(150)]
        [UpperCase]
        public abstract String NOME { get; set; }

        [AcceptNull]
        [MaxLength(150)]
        [UpperCase]
        public abstract String COGNOME { get; set; }

        [AcceptNull]
        [MaxLength(16)]
        [UpperCase]
        public abstract String CODICE_FISCALE { get; set; }

        [AcceptNull]
        [MaxLength(16)]
        [UpperCase]
        public abstract String CODICE_FONDO { get; set; }

        [ExcludeFromInsert]
        [ExcludeFromUpdate]
        [AutomaticField]
        public abstract String CODICE_SAP { get; }

        [AcceptNull]
        public abstract DateTime DATA_NASCITA { get; set; }

        [AcceptNull]
        public abstract DateTime DATA_DECESSO { get; set; }

        [AcceptNull]
        [MaxLength(1)]
        [UpperCase]
        public abstract String SESSO { get; set; }

        [AcceptNull]
        public abstract Int32 NASCITA_GEO_LOCALITA_ID { get; set; }


        [AcceptNull]
        [MaxLength(20)]
        [UpperCase]
        public abstract String RESIDENZA_PROVINCIA { get; set; }

        [AcceptNull]
        [MaxLength(3)]
        [UpperCase]
        public abstract String RESIDENZA_NAZIONE_ISO3 { get; set; }

        [AcceptNull]
        [MaxLength(50)]
        [UpperCase]
        public abstract String RESIDENZA_NAZIONE { get; set; }

        [AcceptNull]
        [MaxLength(20)]
        [UpperCase]
        public abstract String RESIDENZA_CAP { get; set; }

        [AcceptNull]
        [MaxLength(100)]
        [UpperCase]
        public abstract String RESIDENZA_LOCALITA { get; set; }


        [AcceptNull]
        [MaxLength(100)]
        [UpperCase]
        public abstract String RESIDENZA_INDIRIZZO_NOTE { get; set; }

        [AcceptNull]
        [MaxLength(100)]
        [UpperCase]
        public abstract String RESIDENZA_INDIRIZZO { get; set; }

        [AcceptNull]
        [MaxLength(20)]
        [UpperCase]
        public abstract String RESIDENZA_CIVICO { get; set; }


        [AcceptNull]
        [MaxLength(3)]
        [UpperCase]
        public abstract String DOMICILIO_NAZIONE_ISO3 { get; set; }

        [AcceptNull]
        [MaxLength(50)]
        [UpperCase]
        public abstract String DOMICILIO_NAZIONE { get; set; }

        [AcceptNull]
        [MaxLength(20)]
        [UpperCase]
        public abstract String DOMICILIO_PROVINCIA { get; set; }


        [AcceptNull]
        [MaxLength(100)]
        [UpperCase]
        public abstract String DOMICILIO_LOCALITA { get; set; }

        [AcceptNull]
        [MaxLength(100)]
        [UpperCase]
        public abstract String DOMICILIO_INDIRIZZO { get; set; }

        [AcceptNull]
        [MaxLength(100)]
        [UpperCase]
        public abstract String DOMICILIO_INDIRIZZO_NOTE { get; set; }

        [AcceptNull]
        [MaxLength(20)]
        [UpperCase]
        public abstract String DOMICILIO_CAP { get; set; }

        [AcceptNull]
        [MaxLength(20)]
        [UpperCase]
        public abstract String DOMICILIO_CIVICO { get; set; }

        [AcceptNull]
        public abstract Int32 CF_GEO_NAZIONE_ID { get; set; }

        public abstract Boolean FLAG_FORMATO_CF_ESTERO { get; set; }

        [AutomaticField]
        [ExcludeFromInsert]
        [ExcludeFromUpdate]
        public abstract String NOMINATIVO { get; }

        [AutomaticField]
        [ExcludeFromInsert]
        [ExcludeFromUpdate]
        public abstract String NOMINATIVO_INV { get; }

        [LogicalDelete]
        public abstract Boolean FLAG_CANC { get; }

        [AutoInsertTimestamp]
        public abstract DateTime DATA_INSERIMENTO { get; }

        [AutoUpdateTimestamp]
        public abstract DateTime DATA_AGGIORNAMENTO { get; }

        [UserInfo]
        [MaxLength(50)]
        public abstract String CODICE_UTENTE { get; }


    }



    public abstract class ANAGRAFICA_PF_Lista : DataList<ANAGRAFICA_PF_Lista, ANAGRAFICA_PF>
    {




        /// <summary>
        /// Cerca soggetti che compiono l'eta specificata nell'anno indicato
        /// </summary>
        /// <param name="tipoSogg"></param>
        /// <param name="eta"></param>
        /// <param name="anno"></param>
        /// <returns></returns>
        public ANAGRAFICA_PF_Lista CercaTipiSoggettoEtaIntervallo(int tipoSogg, int eta, DateTime dtInizio, DateTime dtFine)
        {
            var daI = dtInizio;
            var daF = dtFine;

            var dnI = new DateTime(dtInizio.Year - eta, dtInizio.Month, dtInizio.Day);
            var dnF = new DateTime(dtFine.Year - eta, dtFine.Month, DateTime.DaysInMonth(dtFine.Year - eta, dtFine.Month));

            var sbSql = new StringBuilder();

            sbSql.Append("select DISTINCT po.ANAGRAFICA_PF_ID AS ID ");
            sbSql.Append($"from POSIZIONI_DATI pd ");
            sbSql.Append($"inner join POSIZIONI_TIPI_SOGGETTO_DETTAGLIO tsd on tsd.ID=pd.VALORE and tsd.TIPO_SOGGETTO_ID = @TIPOSOGG ");
            sbSql.Append($"inner join POSIZIONI po on po.ID=pd.POSIZIONE_ID ");
            sbSql.Append($"inner join ANAGRAFICA_PF an on an.ID=po.ANAGRAFICA_PF_ID ");

            sbSql.Append($"where pd.TIPO_ID = 11 ");
            sbSql.Append("and (@ANNOI between pd.DATA_INIZIO and pd.DATA_FINE ");
            sbSql.Append("OR @ANNOF between pd.DATA_INIZIO and pd.DATA_FINE) ");
            sbSql.Append("and pd.FLAG_CANC = 0 ");
            sbSql.Append("and an.DATA_NASCITA BETWEEN @DNI AND @DNF ");

            this.Slot.DB.AddParameter("@TIPOSOGG", tipoSogg);
            this.Slot.DB.AddParameter("@ANNOI", daI);
            this.Slot.DB.AddParameter("@ANNOF", daF);
            this.Slot.DB.AddParameter("@DNI", dnI);
            this.Slot.DB.AddParameter("@DNF", dnF);

            this.Slot.DB.SQL = sbSql.ToString();

            return this.DoSearch();
        }


    }

}
