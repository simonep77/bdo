﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Bdo.Objects
{
    /// <summary>
    /// Impostazioni Businessslot - Default da .config
    /// </summary>
    public class SlotConfig
    {
        internal class Defaults
        {
            internal static string LogBaseDirectory = Bdo.Properties.Settings.Default.LogBaseDirectory;
            internal static bool LogDatabaseActivity = Bdo.Properties.Settings.Default.LogDatabaseActivity;
            internal static bool LogDatabaseOnlyErrors = Bdo.Properties.Settings.Default.LogDatabaseOnlyErrors;
            internal static int CacheGlobalSize = Bdo.Properties.Settings.Default.CacheGlobalSize;
            internal static bool ObjectValidationUseMessageList = Bdo.Properties.Settings.Default.ObjectValidationUseMessageList;
            internal static string XmlDefaultDateFormat = Bdo.Properties.Settings.Default.XmlDefaultlDateFormat;
            internal static string XmlDefaultDecimalFormat = Bdo.Properties.Settings.Default.XmlDefaultDecimalFormat;
            internal static bool LiveTrackingEnabled = Bdo.Properties.Settings.Default.ObjectLiveTrackingActive;
            internal static bool ChangeTrackingEnabled = Bdo.Properties.Settings.Default.ChangeTrackingEnabled;
            internal static bool LoadFullObjects = Bdo.Properties.Settings.Default.LoadFullObjects;

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
