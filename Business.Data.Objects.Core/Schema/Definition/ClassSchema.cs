using Business.Data.Objects.Common.Resources;
using Business.Data.Objects.Core.Attributes;
using System;

namespace Business.Data.Objects.Core.Schema.Definition
{
    /// <summary>
    /// Definizione di Schema
    /// </summary>
    class ClassSchema
    {
        public const string PRIMARY_KEY = @"PrimaryKey";

        #region FIELDS
        public Table TableDef;
        public DbConnection DbConnDef;
        public long InternalID;
        public Type OriginalType;

        public byte ObjCount;
        public KeyDictionary Keys = new KeyDictionary();
        public PropertyDictionary Properties;
        public PropertyList AutoProperties = new PropertyList(3);
        public Key PrimaryKey;

        public bool GlobalCache;
        public bool MustReload;
        public bool AutoIncPk;

        public PropertyList LogicalDeletes = new PropertyList(1);
        public Property Username;

        #endregion

        #region PROPERTIES

        public string ClassName
        {
            get
            {
                return this.OriginalType.Name;
            }
        }

        /// <summary>
        /// Indica se la classe e' in sola lettura
        /// </summary>
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Indica se utilizza la connessione db di default
        /// </summary>
        public bool IsDefaultDb
        { 
            get {
                return (this.DbConnDef == null);
            } 
        }

        #endregion

        #region CONSTRUCTORS

        public ClassSchema(Type originalType)
        {
            this.OriginalType = originalType;
            this.Properties = new PropertyDictionary(this, 30);
        }

        #endregion

        #region INTERNAL METHODS

        /// <summary>
        /// Esegue validazione schema
        /// </summary>
        /// <param name="oSchema"></param>
        internal void Validate()
        {
            //Se non presente definizione tabella la crea
            if (this.TableDef==null)
                this.TableDef = new Table(this.ClassName);

            //Se num prop = 0 - errore
            if (this.Properties.Count == 0)
                throw new SchemaReaderException(this, SchemaMessages.Schema_NoProperties);

            //Se la PK non contiene proprietà - errore
            if (this.PrimaryKey == null || this.PrimaryKey.Properties.Count == 0)
                throw new SchemaReaderException(this, SchemaMessages.Schema_NoPrimaryKey);

            //Se la PK contiene oggetti mappati: errore
            for (int i = 0; i < this.PrimaryKey.Properties.Count; i++)
            {
                if (this.PrimaryKey.Properties[i] is PropertyObject)
                    throw new SchemaReaderException(this.PrimaryKey.Properties[i], SchemaMessages.Prop_PrimaryKey_SimpleType);
            }

            //Attributo Username Readonly
            if (this.Username != null && !this.Username.IsReadonly)
                throw new SchemaReaderException(this.Username, SchemaMessages.Prop_MustBeReadOnlyForAttribute, nameof(Attributes.Username));

            //Cancellazione logica solo su campo int o datetime
            //Puo' essere interessante impostarlo come readonly????
            foreach (var ldProp in this.LogicalDeletes)
            {
                if (!ldProp.IsReadonly)
                    throw new SchemaReaderException(ldProp, SchemaMessages.Prop_MustBeReadOnlyForAttribute, nameof(Attributes.LogicalDelete));

                if (!ldProp.Type.IsValueType)
                    throw new SchemaReaderException(ldProp, SchemaMessages.Prop_PrimaryKey_SimpleType);

                switch (Type.GetTypeCode(ldProp.Type))
                {
                    case TypeCode.Boolean:
                        break;
                    case TypeCode.Byte:
                        break;
                    case TypeCode.DateTime:
                        break;
                    case TypeCode.Int16:
                        break;
                    case TypeCode.Int32:
                        break;
                    case TypeCode.Int64:
                        break;
                    case TypeCode.SByte:
                        break;
                    case TypeCode.UInt16:
                        break;
                    case TypeCode.UInt32:
                        break;
                    case TypeCode.UInt64:
                        break;
                    default:
                        throw new SchemaReaderException(ldProp, SchemaMessages.Prop_LogicalDeleteWrongType);
                }
            }
            
        }

        /// <summary>
        /// Carica informazioni da attributo
        /// </summary>
        /// <param name="att"></param>
        internal void FillFromAttribute(Attribute att)
        {
            if (att is Table)
            {
                //Nome Schema - Tabella
                this.TableDef = (Table)att;

                //Se la tabella ha un dbconnection allora emulo l'attributo
                if (!string.IsNullOrEmpty(this.TableDef.DbConnectionKey))
                {
                    if (this.DbConnDef != null)
                        throw new SchemaReaderException(this, SchemaMessages.DbConnectionAlreadyDefined);

                    //Lo imposta 
                    this.DbConnDef = new DbConnection(this.TableDef.DbConnectionKey);
                }
            }
            else if (att is GlobalCache)
            {
                //Cache di sessione
                this.GlobalCache = true;
                //Disabilita accessi in scrittura
                this.IsReadOnly = true;
            }
            else if (att is ReadOnly)
            {
                //Disabilita accessi in scrittura
                this.IsReadOnly = true;
            }
            else if (att is DbConnection)
            {
                if (this.DbConnDef != null)
                    throw new SchemaReaderException(this, SchemaMessages.DbConnectionAlreadyDefined);

                this.DbConnDef = (DbConnection)att;
            }
            

        }
        
        #endregion


    }
}
