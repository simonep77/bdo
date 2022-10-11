using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Business.Data.Objects.Core
{
    /// <summary>
    /// Impostazioni Businessslot - Default da .config
    /// </summary>
    public class SlotConfig
    {
        internal class Defaults
        {
            public static string LogBaseDirectory = string.Empty;
            public static bool LogDatabaseActivity = true;
            public static bool LogDatabaseOnlyErrors = true;
            public static int CacheGlobalSize = 8192;
            public static bool ObjectValidationUseMessageList = false;
            public static string XmlDefaultDateFormat = @"dd/MM/yyyy";
            public static string XmlDefaultDecimalFormat = "N2";
            public static bool LiveTrackingEnabled = false;
            public static bool ChangeTrackingEnabled = true;

        }

        #region LOGGING

        /// <summary>
        /// Directory base per il logging
        /// </summary>
        public string LogBaseDirectory { get; set; } = Defaults.LogBaseDirectory;

        /// <summary>
        /// Indica se attivo il logging delle operazioni database
        /// </summary>
        public bool LogDatabaseActivity { get; set; } = Defaults.LogDatabaseActivity;

        /// <summary>
        /// Indica se per il log db vanno loggati solo gli errori
        /// </summary>
        public bool LogDatabaseOnlyErrors { get; set; } = Defaults.LogDatabaseOnlyErrors;
       
        #endregion

        #region CACHING

        /// <summary>
        /// Dimensione cache GLOBAL
        /// </summary>
        public int CacheGlobalSize { get; set; } = Defaults.CacheGlobalSize;

        #endregion

        #region VALIDATION

        /// <summary>
        /// Indica se i messaggi di validazione devono essere singolarmente immessi nella lista messaggi dello slot
        /// </summary>
        public bool ObjectValidationUseMessageList { get; set; } = Defaults.ObjectValidationUseMessageList;
       
        #endregion

        #region FORMATS

        /// <summary>
        /// Indica il tipo di formattazione XML per le date
        /// </summary>
        public string XmlDefaultDateFormat { get; set; } =  Defaults.XmlDefaultDateFormat;

        /// <summary>
        /// Indica il tipo di formattazione XML dei decimal
        /// </summary>
        public string XmlDefaultDecimalFormat { get; set; } = Defaults.XmlDefaultDecimalFormat;

        #endregion

        #region FEATURES

        /// <summary>
        /// Indica se attivo il live tracking
        /// </summary>
        public bool LiveTrackingEnabled { get; set; } = Defaults.LiveTrackingEnabled;

        /// <summary>
        /// Indica se attivo il change tracking
        /// </summary>
        public bool ChangeTrackingEnabled { get; set; } = Defaults.ChangeTrackingEnabled;

        /// <summary>
        /// Indica se attivo l'event manager
        /// </summary>
        public bool EventManagerEnabled { get; set; } = false;

        /// <summary>
        /// Indica se attivo il caching (globale)
        /// </summary>
        public bool CachingEnabled { get; set; } = true;

        /// <summary>
        /// Indica se attiva la modalita' simulazione (no write)
        /// </summary>
        public bool SimulateEnabled { get; set; } = false;

        #endregion

        /// <summary>
        /// Crea una versione clonata della configurazione
        /// </summary>
        /// <returns></returns>
        public SlotConfig Clone()
        {
            var sc = new SlotConfig();

            sc.LogBaseDirectory = this.LogBaseDirectory;
            sc.LogDatabaseActivity = this.LogDatabaseActivity;
            sc.LogDatabaseOnlyErrors = this.LogDatabaseOnlyErrors;
            sc.CacheGlobalSize = this.CacheGlobalSize;
            sc.ObjectValidationUseMessageList = this.ObjectValidationUseMessageList;
            sc.XmlDefaultDateFormat = this.XmlDefaultDateFormat;
            sc.XmlDefaultDecimalFormat = this.XmlDefaultDecimalFormat;
            sc.LiveTrackingEnabled = this.LiveTrackingEnabled;
            sc.ChangeTrackingEnabled = this.ChangeTrackingEnabled;
            sc.EventManagerEnabled = this.EventManagerEnabled;
            sc.CachingEnabled = this.CachingEnabled;
            sc.SimulateEnabled = this.SimulateEnabled;

            return sc;
        }
    }
}
