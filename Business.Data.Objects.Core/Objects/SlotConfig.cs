using System;
using System.Collections.Generic;
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
            internal static string LogBaseDirectory = Assembly.GetExecutingAssembly().Location;
            internal static bool LogDatabaseActivity = true;
            internal static bool LogDatabaseOnlyErrors = true;
            internal static int CacheGlobalSize = 8192;
            internal static bool ObjectValidationUseMessageList = false;
            internal static string XmlDefaultDateFormat = @"dd/MM/yyyy";
            internal static string XmlDefaultDecimalFormat = "N2";
            internal static bool LiveTrackingEnabled = false;
            internal static bool ChangeTrackingEnabled = true;
            internal static bool LoadFullObjects = true;

        }

        #region LOGGING

        /// <summary>
        /// Directory base per il logging
        /// </summary>
        public string LogBaseDirectory { get; set; } = Defaults.LogBaseDirectory;

        /// <summary>
        /// Indica se attivo il logging delle operazioni database
        /// </summary>
        public bool LogDatabaseActivity { get; } = Defaults.LogDatabaseActivity;

        /// <summary>
        /// Indica se per il log db vanno loggati solo gli errori
        /// </summary>
        public bool LogDatabaseOnlyErrors { get; } = Defaults.LogDatabaseOnlyErrors;
       
        #endregion

        #region CACHING

        /// <summary>
        /// Dimensione cache GLOBAL
        /// </summary>
        public int CacheGlobalSize { get; } = Defaults.CacheGlobalSize;

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
        /// Indica se attivo il caricamento completo degli oggetti nelle liste
        /// </summary>
        public bool LoadFullObjects { get; set; } = Defaults.LoadFullObjects;

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
    }
}
