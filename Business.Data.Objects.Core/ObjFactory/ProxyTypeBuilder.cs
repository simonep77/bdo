using Business.Data.Objects.Common.Exceptions;
using Business.Data.Objects.Common.Resources;
using Business.Data.Objects.Common.Utils;
using Business.Data.Objects.Core.Attributes;
using Business.Data.Objects.Core.Base;
using Business.Data.Objects.Core.Schema;
using Business.Data.Objects.Core.Schema.Definition;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Business.Data.Objects.Core.ObjFactory
{
    /// <summary>
    /// Classe statica che crea le classi dinamiche costruite a partire da quelle dichiarate
    /// </summary>
    static class ProxyTypeBuilder
    {

        #region FIELDS

        private const string STR_BDO_SUFFIX = @"Bdo.Proxy.";
        private const string STR_MODULE_NAME = @"ModuleDyn";
        private static Type TYPE_BIZ_OBJ_BASE = typeof(BusinessObjectBase);
        private static Type TYPE_BIZ_FACTORY_INTERFACE = typeof(IBusinessObjFactory);
        private static Type TYPE_DATA_OBJ_BASE = typeof(DataObjectBase);
        private static Type TYPE_DATA_LIST_BASE = typeof(DataListBase);
        private static Type[] TYPE_ARR_BASE = { typeof(Type) };
        private static Type[] TYPE_ARR_DATAOBJ = { typeof(DataObjectBase) };
        private static object[] ARR_EMPTY = { };
        private static Type[] TYPE_ARR_SERIAL = { typeof(System.Runtime.Serialization.SerializationInfo), typeof(System.Runtime.Serialization.StreamingContext) };


        #endregion


        #region METODI PRIVATI

        #region ASSEMBLY MANAGEMENT

        /// <summary>
        /// Work class
        /// </summary>
        private class DynAssemblyProxy
        {
            public AssemblyBuilder PxAssemblyBuilder;
            public ModuleBuilder PxModuleBuilder;
        }

        /// <summary>
        /// Inizializza l'assembly dinamico che contiene gli oggetti BDO
        /// </summary>
        private static DynAssemblyProxy initProxyAssembly(Assembly ass)
        {
            //Crea un nom assembly
            AssemblyName assName = new AssemblyName(string.Concat(STR_BDO_SUFFIX, ass.GetName().Name))
            {
                Version = new Version(1, 0, 0, 1),
                Flags = AssemblyNameFlags.EnableJITcompileOptimizer
            };

            //Definisce l'assembly builder
            AssemblyBuilder assBuilder = AssemblyBuilder.DefineDynamicAssembly(assName, AssemblyBuilderAccess.Run);



            //Definisce Modulo dinamico
            ModuleBuilder pxModBuilder = assBuilder.DefineDynamicModule(STR_MODULE_NAME);

            return new DynAssemblyProxy() { 
                PxAssemblyBuilder = assBuilder,
                PxModuleBuilder = pxModBuilder
            };
        }


        /// <summary>
        /// Crea i tipi modificati per l'assembly specificato
        /// </summary>
        /// <param name="outProxy"></param>
        internal static void BuildDaoProxyFromAssembly(ProxyAssemblyCache.ProxyAssemblyDao outProxy)
        {
            DynAssemblyProxy dapx = initProxyAssembly(outProxy.SrcAss);

            //Imposta oggetto output base 
            outProxy.DynAss = dapx.PxAssemblyBuilder;
            
            //Controlla ogni tipo definito nell'Assembly
            foreach (Type tOriginal in outProxy.SrcAss.GetTypes())
            {
                bool bCreateSchema = tOriginal.IsSubclassOf(TYPE_DATA_OBJ_BASE);
                bool bIsBdoItem = (bCreateSchema || tOriginal.IsSubclassOf(TYPE_DATA_LIST_BASE));
                PropertyInfo[] arrPropInfo = null;

                //Se non BDO allora skip
                if (!bIsBdoItem)
                    continue;

                //Ok, BDO
                try
                {
                    long iOriginalTypeHandle = tOriginal.TypeHandle.Value.ToInt64();
                    int iPropertyIndex = 0; //Indice interno della proprietà
                    string sOrigFullTypeName = tOriginal.FullName;

                    //la classe deve essere pubblica
                    if (tOriginal.IsNotPublic)
                        throw new TypeFactoryException("La classe '{0}' deve essere definita PUBLIC.", sOrigFullTypeName);

                    //Definisce TypeBuilder
                    TypeBuilder typeBuild = dapx.PxModuleBuilder.DefineType(string.Concat(STR_BDO_SUFFIX, sOrigFullTypeName), TypeAttributes.Public | TypeAttributes.Sealed, tOriginal);
                    ConstructorBuilder cbDefault = typeBuild.DefineDefaultConstructor(MethodAttributes.Public);

                    //Se non necessario schema va ad aggiunta entry
                    if (!bCreateSchema)
                        goto ADD_TYPE_ENTRY;

                    //Carica i Metodi Custom per GET e SET
                    MethodInfo myGetCustom = tOriginal.GetMethod("GetProperty", BindingFlags.Instance | BindingFlags.Public);
                    MethodInfo mySetCustom = tOriginal.GetMethod("SetProperty", BindingFlags.Instance | BindingFlags.Public);

                    //Carica elenco proprieta'
                    arrPropInfo = tOriginal.GetProperties(BindingFlags.Instance | BindingFlags.Public);

                    //Ordina le proprieta' per come sono state definite
                    Array.Sort<PropertyInfo>(arrPropInfo,
                        delegate(PropertyInfo p1, PropertyInfo p2)
                        {
                            return p1.MetadataToken.CompareTo(p2.MetadataToken);
                        });

                    //Loop su proprietà
                    for (int i = 0; i < arrPropInfo.Length; i++)
                    {
                        PropertyInfo prop = arrPropInfo[i];

                        //Cattura Get e Set Correnti
                        MethodInfo getMethod = prop.GetGetMethod();
                        MethodInfo setMethod = prop.GetSetMethod();

                        MethodBuilder newGetMethod = null;
                        MethodBuilder newSetMethod = null;

                        //Crea i nuovi metodi
                        //GET
                        if ((getMethod != null) && (getMethod.IsAbstract))
                        {
                            //Creo un nuovo metodo GET
                            newGetMethod = typeBuild.DefineMethod(getMethod.Name, MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, prop.PropertyType, null);
                            //Creo codice IL
                            ILGenerator ilGen = newGetMethod.GetILGenerator();
                            ilGen.Emit(OpCodes.Ldarg_0);
                            ilGen.Emit(OpCodes.Ldc_I4, iPropertyIndex);
                            ilGen.Emit(OpCodes.Call, myGetCustom);
                            ilGen.Emit(OpCodes.Unbox_Any, prop.PropertyType);
                            ilGen.Emit(OpCodes.Ret);
                        }

                        //SET
                        if ((setMethod != null) && (setMethod.IsAbstract))
                        {
                            //Creo un nuovo metodo SET
                            newSetMethod = typeBuild.DefineMethod(setMethod.Name, MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null, new Type[] { prop.PropertyType });
                            //Creo codice IL
                            ILGenerator ilGen = newSetMethod.GetILGenerator();

                            ilGen.Emit(OpCodes.Ldarg_0);
                            ilGen.Emit(OpCodes.Ldc_I4, iPropertyIndex);
                            ilGen.Emit(OpCodes.Ldarg_1);

                            if (prop.PropertyType.IsValueType)
                                ilGen.Emit(OpCodes.Box, prop.PropertyType);

                            ilGen.Emit(OpCodes.Call, mySetCustom);
                            ilGen.Emit(OpCodes.Ret);
                        }

                        //Se è stato generato almeno un metodo
                        if (newGetMethod != null || newSetMethod != null)
                        {
                            PropertyBuilder propBuild = typeBuild.DefineProperty(prop.Name, PropertyAttributes.None, prop.PropertyType, new Type[] { prop.PropertyType });

                            //Se c'è GET imposta nuovo metodo
                            if (newGetMethod != null)
                            {
                                typeBuild.DefineMethodOverride(newGetMethod, getMethod);
                            }

                            //Se c'è SET imposta nuovo metodo
                            if (newSetMethod != null)
                            {
                                typeBuild.DefineMethodOverride(newSetMethod, setMethod);
                            }

                            //Incrementa contatore di proprietà mappata
                            iPropertyIndex++;
                        }
                        else
                        { 
                            //Si tratta di proprieta' non mappata, quindi la imposta a null
                            arrPropInfo[i] = null;
                        }

                    }

                ADD_TYPE_ENTRY:

                    //Crea oggetto appoggio cont tipo + delegato costruttore
                    ProxyEntryDAO oTypeEntry = new ProxyEntryDAO();
                    oTypeEntry.ProxyType = typeBuild.CreateTypeInfo().AsType();
                    oTypeEntry.Create = createFastConstructor(oTypeEntry.ProxyType, dapx.PxModuleBuilder);
                    //Crea schema dei soli oggetti singoli
                    if (bCreateSchema)
                    {
                        oTypeEntry.ClassSchema = readClassSchemaWithSQL(tOriginal, iOriginalTypeHandle, arrPropInfo);
                    }

                    //Aggiunge a lista su proxy
                    outProxy.TypeDaoEntries.Add(iOriginalTypeHandle, oTypeEntry);
                }
                catch (Exception ex)
                {
                    throw new SchemaReaderException($"Errore dutante la scansione dell'assembly {outProxy.SrcAss.FullName}. {ex.Message}");
                }

            }
        }

        /// <summary>
        /// Crea un delegato (puntatore a funzione) per il costruttore dell'oggetto
        /// </summary>
        /// <param name="aType"></param>
        /// <returns></returns>
        private static ProxyEntryDAO.FastConstructor createFastConstructor(Type aType, ModuleBuilder pxModBuilder)
        {
            DynamicMethod dm = new DynamicMethod("MyCtor", aType, Type.EmptyTypes, pxModBuilder);
            ILGenerator ilgen = dm.GetILGenerator();
            ilgen.Emit(OpCodes.Newobj, aType.GetConstructor(Type.EmptyTypes));
            ilgen.Emit(OpCodes.Ret);

            return (ProxyEntryDAO.FastConstructor)dm.CreateDelegate(typeof(ProxyEntryDAO.FastConstructor));
        }

       
        #endregion

        #region SCHEMA MANAGEMENT


        /// <summary>
        /// Legge il tipo e ritorna lo schema pronto
        /// </summary>
        /// <param name="originalType"></param>
        /// <param name="InternalID"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        private static ClassSchema readClassSchemaWithSQL(Type originalType, long InternalID, PropertyInfo[] properties)
        {
            short iPropertyIndex = 0;
            List<SearchKey> oListKeys = new List<SearchKey>(2);
            StringBuilder sbSqlSelect = new StringBuilder(1000);
            StringBuilder sbSqlReload = new StringBuilder(100);
            StringBuilder sbSqlInsertF = new StringBuilder(400);
            StringBuilder sbSqlInsertP = new StringBuilder(400);
            sbSqlSelect.Append("SELECT ");
            sbSqlReload.Append("SELECT ");

            //Crea Schema
            var oSchema = new ClassSchema(originalType);

            //Imposta ID Interno Schema
            oSchema.InternalID = InternalID;

            //Imposta info schema (tabella) se fornite
            foreach (Attribute att in originalType.GetCustomAttributes(false))
            {
                oSchema.FillFromAttribute(att);
            }

            //Nome tabella insert
          
            //Legge definizione campi
            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo propInfo = properties[i];

                if (propInfo == null)
                    continue;

                //Azzera variabili ciclo
                oListKeys.Clear();

                //Crea nuova Property
                Property oProp;

                //Verifica il tipo di proprietà
                if (propInfo.PropertyType.IsSubclassOf(typeof(DataObjectBase)))
                {
                    oProp = new PropertyObject(propInfo.Name, propInfo.PropertyType, oSchema.ObjCount);
                    oSchema.ObjCount++; //Incrementa contatore generale
                }
                else if (propInfo.PropertyType.IsSubclassOf(typeof(DataListBase)))
                {
                    oProp = new PropertyDataList(propInfo.Name, propInfo.PropertyType);
                }
                else
                    oProp = new PropertySimple(propInfo.Name, propInfo.PropertyType);
                
                oProp.Schema = oSchema;
                oProp.IsReadonly = !propInfo.CanWrite;

                //Inizia valorizzazione
                foreach (Attribute attr in propInfo.GetCustomAttributes(false))
                {
                    if (!(attr is BaseAttribute))
                        continue;

                    //Primary Key, UniqueKey o SearchKey
                    if (attr is SearchKey)
                    {
                        oListKeys.Add((SearchKey)attr);
                    }
                    //ALTRI ATTRIBUTI
                    else
                    {
                        //Carica attributi
                        try
                        {
                            oProp.FillFromAttribute((BaseAttribute)attr);
                        }
                        catch (NotImplementedException)
                        {
                            throw new TypeFactoryException("{0} - Attributo '{1}' non previsto per il tipo di propieta'", oProp.Fullname, attr.GetType().Name);
                        }
                        catch (Exception ex)
                        {
                            throw new TypeFactoryException("{0} - {1}", oProp.Fullname, ex.Message);
                        }
                    }

                    //FINE LOOP ATTRIBUTI
                }

                //Se non sono presenti campi definiti esplicitamente e non e' una mappatura interna ne crea uno di default
                if (oProp is PropertySimple)
                {
                    if (oProp.Column == null)
                        oProp.Column = new Column(oProp.Name, oProp.Type);
                }

                //Gestisce una o più chiavi
                for (int j = 0; j < oListKeys.Count; j++)
                {
                    fillKeyAttribute(oSchema, oProp, oListKeys[j]);
                }

                //Esegue validazione proprietà
                oProp.ValidateDefinition();

                //Imposta indice e incrementa
                oProp.PropertyIndex = iPropertyIndex++; 

                //Aggiunge a schema
                oSchema.Properties.Add(oProp);

     
                //Imposta flag di nuovo caricamento
                if (oProp.IsAutomatic)
                    oSchema.MustReload = true;

                //Select: esclude loadonaccess e mappature property-property
                if (!oProp.IsSqlSelectExcluded)
                {
                    sbSqlSelect.Append(oProp.Column.Name);
                    sbSqlSelect.Append(@", ");
                }

                //Se READONLY NON GENERA nulla
                if (!oSchema.IsReadOnly)
                {
                    //Reload:
                    if (oProp.IsAutomatic)
                    {
                        //Imposta sql reload
                        sbSqlReload.Append(oProp.Column.Name);
                        sbSqlReload.Append(@", ");
                    }

                    //Insert
                    if (!oProp.ExcludeInsert)
                    {
                        //Nomi campi insert
                        sbSqlInsertF.Append(oProp.Column.Name);
                        sbSqlInsertF.Append(@", ");

                        //Parametri o costanti
                        if (oProp.IsAutomatic)
                        {
                            sbSqlInsertP.Append(@"CURRENT_TIMESTAMP, ");
                        }
                        else
                        {
                            sbSqlInsertP.Append(oProp.Column.ParamName);
                            sbSqlInsertP.Append(@", ");
                        }
                    }

                }                

                //FINE LOOP PROPRIETA'
            }


            //Esegue validazione schema
            oSchema.Validate();

            //SQLSELECT - Tabella
            sbSqlSelect.Remove(sbSqlSelect.Length - 2, 2);
            sbSqlSelect.Append(@" FROM ");

            oSchema.TableDef.SQL_Select_Item = sbSqlSelect.ToString();
            //SQLSELECT - Per ogni chiave prepara la where
            foreach (var key in oSchema.Keys.Values)
            {
                sbSqlSelect.Length = 0;//Resetta 
                sbSqlSelect.Append(@" WHERE ");
                for (int i = 0; i < key.Properties.Count; i++)
                {
                    sbSqlSelect.Append(key.Properties[i].Column.Name);
                    sbSqlSelect.Append(@"=");
                    sbSqlSelect.Append(key.Properties[i].Column.GetKeyParamName());
                    sbSqlSelect.Append(@" AND ");

                }

                sbSqlSelect.Remove(sbSqlSelect.Length - 5, 5);
                
                //Imposta SQL completo
                key.SQL_Where_Clause = sbSqlSelect.ToString();
                
                //Se PK genera query base per lista 
                if (key.Name.Equals(ClassSchema.PRIMARY_KEY))
                {
                    //Prepara la query base per la lista
                    sbSqlSelect.Length = 0;//Resetta 
                    sbSqlSelect.Append(@"SELECT ");
                    for (int i = 0; i < key.Properties.Count; i++)
                    {
                        sbSqlSelect.Append(key.Properties[i].Column.Name);
                        sbSqlSelect.Append(@", ");

                    }
                    sbSqlSelect.Remove(sbSqlSelect.Length - 2, 2);
                    sbSqlSelect.Append(@" FROM ");

                    //Aggiunge
                    oSchema.TableDef.SQL_Select_List = sbSqlSelect.ToString();
                }
            }


            if (!oSchema.IsReadOnly)
            {
                //Rimuove caratteri
                sbSqlInsertF.Remove(sbSqlInsertF.Length - 2, 2);
                sbSqlInsertP.Remove(sbSqlInsertP.Length - 2, 2);

                //Sql reload
                if (sbSqlReload.Length > 7)
                {
                    //Rimuove caratteri
                    sbSqlReload.Remove(sbSqlReload.Length - 2, 2);
                    sbSqlReload.Append(@" FROM ");

                    oSchema.TableDef.SQL_Select_Reload = sbSqlReload.ToString();
                }

                //Insert
                oSchema.TableDef.SQL_Insert = string.Concat(@" (", sbSqlInsertF.ToString(), @") VALUES (", sbSqlInsertP.ToString(), @") ");

            }

            //Ritorna schema letto
            return oSchema;
        }


        /// <summary>
        /// Gestione chiavi
        /// </summary>
        /// <param name="oSchema"></param>
        /// <param name="oProp"></param>
        /// <param name="oAttrKey"></param>
        /// <returns></returns>
        private static Key fillKeyAttribute(ClassSchema oSchema, Property oProp, SearchKey oAttrKey) 
        {
            Key oKey;
            if (!oSchema.Keys.TryGetValue(oAttrKey.KeyName, out oKey))
            {
                //Crea le key
                oKey = new Key(oAttrKey.KeyName);
                oKey.HashCode = BdoHash.Instance.Hash(string.Concat(oSchema.OriginalType.FullName, @".BdoKeys.", oKey.Name));
                if (oAttrKey is PrimaryKey)
                    //E' la Primary Key
                    oSchema.PrimaryKey = oKey;

                //Aggiunge ad elenco
                oSchema.Keys.Add(oAttrKey.KeyName, oKey);
            }
            else
            {
                //Controllo Anomalie
                //1) Nome chiave PK
                if (!(oAttrKey is PrimaryKey) && oKey.Name == ClassSchema.PRIMARY_KEY)
                    throw new TypeFactoryException("{0} - Il nome di chiave '{1}' e' riservato", oProp.Fullname, ClassSchema.PRIMARY_KEY);

            }

            ////Chiave definita su proprieta' semplice
            //if (!(oProp is PropertySimple))
            //    throw new TypeFactoryException("{0}.{1} - E' ammesso definire una chiave su una proprieta' semplice (non mappata)", oSchema.ClassName, oProp.Name, ClassSchema.PRIMARY_KEY);

            //La property non puo' essere di tipo LoadOnAccess
            if (oProp.IsSqlSelectExcluded)
                throw new TypeFactoryException(SchemaMessages.Prop_KeyNeedValueQuery, oSchema.ClassName, oProp.Name);

            //Aggiunge property a key
            oKey.AddProperty(oProp);

            //Ritorna Key
            return oKey;
        }




        #endregion


        #region BUSINESS OBJECTS HANDLING

        /// <summary>
        /// Ritorna proxy create per business object
        /// </summary>
        /// <returns></returns>
        public static void BuildBizProxyFromAssembly(ProxyAssemblyCache.ProxyAssemblyBiz outProxy)
        {
            var listBiz = new List<Type>();
            var listFact = new List<Type>();
            var bizCacheLocal = new ProxyEntryBizDic(30);

            //Controlla ogni tipo definito nell'Assembly
            foreach (Type tOriginal in outProxy.SrcAss.GetTypes())
            {
                //Se e' biz lo segna
                if (tOriginal.IsSubclassOf(TYPE_BIZ_OBJ_BASE))
                {
                    listBiz.Add(tOriginal);
                    continue;
                }

                //se factory lo segna
                if (TYPE_BIZ_FACTORY_INTERFACE.IsAssignableFrom(tOriginal))
                {
                    listFact.Add(tOriginal);
                    continue;
                }
            }

            //Crea oggetti business
            foreach (var item in listBiz)
            {
                //Crea entry business
                var entry = buildBizProxyEntry(item, outProxy);
                //Add a cache locale
                bizCacheLocal.Add(entry.TypeKey, entry);
            }

            //Crea i factory personalizzati
            foreach (var item in listFact)
            {
                //Recupera tipo biz base del factory
                var tFactGen = item.BaseType;

                if (!item.BaseType.IsGenericType)
                    throw new ArgumentException(string.Format("Il tipo factory {0} non eredita da BusinessObjFactory<T>", item.Name));

                var tBizBase = item.BaseType.GetGenericArguments()[0];

                //Crea istanza factory (1 sola volta per biz)
                IBusinessObjFactory fact = (IBusinessObjFactory)Activator.CreateInstance(item);

                //Ora cerca tutti i tipi che ereditano dal tipo generico di business trovato
                foreach (var tBiz in listBiz)
                {
                    //Cerca tutte le sottoclassi gia' caricate
                    if (!(tBiz.Equals(tBizBase) || tBiz.IsSubclassOf(tBizBase)))
                        continue;

                    var entry = bizCacheLocal[tBiz.TypeHandle.Value.ToInt64()];
                    entry.Factory = fact;
                }
            }

        }



        /// <summary>
        /// Ritorna proxy create per business object
        /// </summary>
        /// <returns></returns>
        private static ProxyEntryBiz buildBizProxyEntry(Type tOriginal, ProxyAssemblyCache.ProxyAssemblyBiz outProxy)
        {
            //Chiave
            long iOriginalTypeHandle = tOriginal.TypeHandle.Value.ToInt64();

            //Recupera tipo dal
            var tBizGen = tOriginal.BaseType;

            while (!tBizGen.IsGenericType)
                tBizGen = tBizGen.BaseType;

            if (tBizGen == null)
                throw new ArgumentException(string.Format("Il tipo business {0} non eredita da BusinessObject<T>", tOriginal.Name));

            var tDal = tBizGen.GetGenericArguments()[0];

            //Qui dovremmo assicurarci che la scansione dal sia avvenuta
            ProxyAssemblyCache.Instance.GetDaoEntry(tDal);

            //Imposta costruttore
            Type[] argTypesDef = new Type[] { typeof(object) };
            Type[] argTypes = new Type[] { tDal };

            DynamicMethod dm = new DynamicMethod("MyCtor", tOriginal, argTypesDef, tOriginal.Module);
            ILGenerator ilgen = dm.GetILGenerator();
            ilgen.Emit(OpCodes.Ldarg_0);
            ilgen.Emit(OpCodes.Newobj, tOriginal.GetConstructor(argTypes));
            ilgen.Emit(OpCodes.Ret);

            var entry = new ProxyEntryBiz()
                {
                    TypeKey = iOriginalTypeHandle,
                    DalType = tDal,
                    Create = (ProxyEntryBiz.FastCreateBizObj)dm.CreateDelegate(typeof(ProxyEntryBiz.FastCreateBizObj))
                };

            outProxy.TypeBizEntries.Add(iOriginalTypeHandle, entry);

            return entry;
        }





        #endregion



        #endregion

        
    }
}
