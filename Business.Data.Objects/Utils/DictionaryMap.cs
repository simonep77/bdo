using System;
using System.Collections.Generic;

namespace Bdo.Utils
{
    /// <summary>
    /// Calsse dizionario con valori multipli
    /// </summary>
    /// <typeparam name="TKEY"></typeparam>
    /// <typeparam name="TVALUE"></typeparam>
    public class DictionaryMap<TKEY, TVALUE>
    {
        #region FIELDS

        private Dictionary<TKEY, List<TVALUE>> mInternalDic;

        #endregion

        #region PROPERTIES
        
        /// <summary>
        /// Ritorna il primo valore trovato per la chiave
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TVALUE this[TKEY key]
        {
            get 
            {
                return this.mInternalDic[key][0];
            }
        }

        /// <summary>
        /// Ritorna numero chiavi presenti
        /// </summary>
        public int Count
        {
            get
            {
                return this.mInternalDic.Count;
            }
        }

        /// <summary>
        /// Ritorna enumeratore di chiavi
        /// </summary>
        public Dictionary<TKEY, List<TVALUE>>.KeyCollection Keys
        {
            get
            {
                return this.mInternalDic.Keys;
            }
        }

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Costruttore default
        /// </summary>
        public DictionaryMap()
            :this(20)
        {  }

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="capacity"></param>
        public DictionaryMap(int capacity)
        {
            this.mInternalDic = new Dictionary<TKEY, List<TVALUE>>(capacity);
        }

        #endregion

        #region PUBLIC METHODS



        /// <summary>
        /// Verifica se chiave presente
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TKEY key)
        {
            return this.mInternalDic.ContainsKey(key);
        }


        /// <summary>
        /// Ritorna valore alla posizione specificata
        /// </summary>
        /// <param name="key"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public TVALUE GetAt(TKEY key, int index)
        {
            List<TVALUE> oList = null;

            //Se non esiste l'item lo crea
            if (this.mInternalDic.TryGetValue(key, out oList))
            {
                return oList[index];
            }
            else
            {
                throw new KeyNotFoundException("La chave fornita non e' stata trovata");
            }
        }


        /// <summary>
        /// Enumeratore dei valori di una chiave
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<TVALUE>.Enumerator GetValuesEnumerator(TKEY key)
        {
            List<TVALUE> oList = null;

            //Se non esiste l'item lo crea
            if (this.mInternalDic.TryGetValue(key, out oList))
            {
                return oList.GetEnumerator();
            }
            else 
            {
                throw new KeyNotFoundException("La chave fornita non e' stata trovata");
            }
        }

        /// <summary>
        /// Ritorna numero valori
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int CountValues(TKEY key)
        {
            List<TVALUE> oList = null;

            //Se non esiste l'item lo crea
            if (this.mInternalDic.TryGetValue(key, out oList))
            {
                return oList.Count;
            }

            return 0;
        }

        /// <summary>
        /// Aggiunge coppia chiave/valore
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(TKEY key, TVALUE value)
        {
            List<TVALUE> oList = null;

            //Se non esiste l'item lo crea
            if (!this.mInternalDic.TryGetValue(key, out oList))
            {
                //Crea lista
                oList = new List<TVALUE>();
                //Aggiunge chiave dizionario
                this.mInternalDic.Add(key, oList);
            }

            //Aggiunge item a lista
            oList.Add(value);
        }

        /// <summary>
        /// Rimuove chiave (con tutti i valori associati)
        /// </summary>
        /// <param name="key"></param>
        public void RemoveAll(TKEY key)
        {
            List<TVALUE> oList = null;

            //Se non esiste l'item lo crea
            if (this.mInternalDic.TryGetValue(key, out oList))
            {
                //Svuota lista
                oList.Clear();
                //Azzera capacita'
                oList.Capacity = 0;
                //rimuove chiave dizionario
                this.mInternalDic.Remove(key);
            }
        }

        /// <summary>
        /// Rimuove un valore. Se non presenti ulteriori valori elimina totalmente la chiave
        /// </summary>
        /// <param name="key"></param>
        /// <param name="index"></param>
        public void RemoveAt(TKEY key, int index)
        {
            List<TVALUE> oList = null;

            //Se non esiste l'item lo crea
            if (this.mInternalDic.TryGetValue(key, out oList))
            {
                //Rimuove
                oList.RemoveAt(index);
                //Se la lista rimane vuota
                if (oList.Count == 0)
                {
                    //Azzera capacita'
                    oList.Capacity = 0;
                    //rimuove chiave dizionario
                    this.mInternalDic.Remove(key);
                }
            }
        }


        /// <summary>
        /// Elimina tutte le chiavi e gli oggetti
        /// </summary>
        public void Clear()
        {
            this.mInternalDic.Clear();

            //Pulizia effettiva di tutte le strutture dati (libera memoria)
            //while (this.mInternalDic.Keys.Count > 0)
            //{
            //    TKEY oKey = default(TKEY);
            //    foreach (TKEY key in this.mInternalDic.Keys)
            //    {
            //        oKey = key;
            //        break;
            //    }
            //    //Rimuove tutti i valori e la chiave
            //    this.RemoveAll(oKey);
            //}
        }


        /// <summary>
        /// Prova a ottenere il valore per la chiave specificata ed alla posizione fornita (0 -> N).
        /// Se viene trovato ritorna true altrimenti false
        /// </summary>
        /// <param name="key"></param>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(TKEY key, int index, out TVALUE value)
        {
            List<TVALUE> oList = null;

            //Se non esiste l'item lo crea
            if (this.mInternalDic.TryGetValue(key, out oList))
            {
                value = oList[index];
                return true;
            }

            value = default(TVALUE);
            return false;
        }


        /// <summary>
        /// Prova ad ottenere il primo valore della chiave fornita. Se trovato ritorna true altrimenti false
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetFirstValue(TKEY key, out TVALUE value)
        {
            return this.TryGetValue(key, 0, out value);
        }


        /// <summary>
        /// Ritorna ultimo valore trovato
        /// </summary>
        /// <param name="key"></param>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetLastValue(TKEY key, out TVALUE value)
        {
            List<TVALUE> oList = null;

            //Se non esiste l'item lo crea
            if (this.mInternalDic.TryGetValue(key, out oList))
            {
                value = oList[oList.Count - 1];
                return true;
            }

            value = default(TVALUE);
            return false;
        }

        #endregion

        #region PRIVATE METHODS


        #endregion
    }
}
