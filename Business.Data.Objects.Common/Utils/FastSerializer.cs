using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Business.Data.Objects.Common.Utils 
{

    /// <summary>
    /// Enumerazione Che identifica i tipi di dato supportati
    /// </summary>
    internal enum ObjType : byte 
    {
        nullType = 0,
        boolType = 1,
        byteType = 2,
        uint16Type = 3,
        uint32Type = 4,
        uint64Type = 5,
        sbyteType = 6,
        int16Type = 7,
        int32Type = 8,
        int64Type = 9,
        charType = 10,
        stringType = 11,
        singleType = 12,
        doubleType = 13,
        decimalType = 14,
        dateTimeType = 15,
        byteArrayType = 16,
        charArrayType = 17, 
        objectArrayType = 18,
        otherType = 19
    }

    /// <summary>
    /// Serializzatore semplice, rapido, con output non voluminoso
    /// Basato su quello di Tim Haynes, May 2006
      /// </summary>
    public class SerializationWriter 
    {
        private BinaryWriter mBinWriter;


        public SerializationWriter (Stream s)
        {
            this.mBinWriter = new BinaryWriter(s);
        }

        /// <summary>
        /// Scrive tipo oggetto
        /// </summary>
        /// <param name="ot"></param>
        private void writeObjType(ObjType ot)
        {
            this.mBinWriter.Write((byte)ot);
        }


        /// <summary>
        /// scrive byte array
        /// </summary>
        /// <param name="b"></param>
        private void writeByteArray (byte[] b) 
        {
            this.mBinWriter.Write(b.Length);

            if (b.Length > 0)
                this.mBinWriter.Write(b);
        }

        /// <summary>
        /// Scrive array di caratteri
        /// </summary>
        /// <param name="c"></param>
        private void writeCharArray (char[] c) {
            this.mBinWriter.Write(c.Length);

            if (c.Length > 0)
                this.mBinWriter.Write(c);
        }


        /// <summary>
        /// Scrive nel flusso un array di oggetti
        /// </summary>
        /// <param name="o"></param>
        private void writeObjArray(object[] o)
        {
            this.mBinWriter.Write(o.Length);
            for (int i = 0; i < o.Length; i++)
            {
                this.WriteObject(o[i]);
            }
        }


        /// <summary>
        /// Scrive oggetto nel flusso di serializzazione
        /// </summary>
        /// <param name="obj"></param>
        public void WriteObject (object obj) 
        {
            //Se null scrive header ed esce
            if (obj == null)
            {
                this.writeObjType(ObjType.nullType);
                return;
            }

            switch (obj.GetType().Name) 
            {

                case "Boolean": 
                this.writeObjType(ObjType.boolType);
                this.mBinWriter.Write((bool)obj);
                break;

                case "Byte": 
                this.writeObjType(ObjType.byteType);
                this.mBinWriter.Write((byte)obj);
                break;

                case "UInt16": 
                this.writeObjType(ObjType.uint16Type);
                this.mBinWriter.Write((ushort)obj);
                break;

                case "UInt32": 
                this.writeObjType(ObjType.uint32Type);
                this.mBinWriter.Write((uint)obj);
                break;

                case "UInt64": 
                this.writeObjType(ObjType.uint64Type);
                this.mBinWriter.Write((ulong)obj);
                break;

                case "SByte": 
                this.writeObjType(ObjType.sbyteType);
                this.mBinWriter.Write((sbyte)obj);
                break;

                case "Int16": 
                this.writeObjType(ObjType.int16Type);
                this.mBinWriter.Write((short)obj);
                break;

                case "Int32": 
                this.writeObjType(ObjType.int32Type);
                this.mBinWriter.Write((int)obj);
                break;

                case "Int64": 
                this.writeObjType(ObjType.int64Type);
                this.mBinWriter.Write((long)obj);
                break;

                case "Char": 
                this.writeObjType(ObjType.charType);
                this.mBinWriter.Write((char)obj);
                break;

                case "String": 
                this.writeObjType(ObjType.stringType);
                this.mBinWriter.Write((string)obj);
                break;

                case "Single": 
                this.writeObjType(ObjType.singleType);
                this.mBinWriter.Write((float)obj);
                break;

                case "Double": 
                this.writeObjType(ObjType.doubleType);
                this.mBinWriter.Write((double)obj);
                break;

                case "Decimal": 
                this.writeObjType(ObjType.decimalType);
                this.mBinWriter.Write((decimal)obj);
                break;

                case "DateTime": 
                this.writeObjType(ObjType.dateTimeType);
                this.mBinWriter.Write(((DateTime)obj).ToBinary());
                break;

                case "Byte[]": 
                this.writeObjType(ObjType.byteArrayType);
                this.writeByteArray((byte[])obj);
                break;

                case "Char[]": 
                this.writeObjType(ObjType.charArrayType);
                this.writeCharArray ((char[])obj);
                break;

                case "Object[]": 
                this.writeObjType(ObjType.objectArrayType);
                this.writeObjArray((object[])obj);
                break;

                default: 
                this.writeObjType(ObjType.otherType);
                new BinaryFormatter().Serialize(this.mBinWriter.BaseStream, obj);
                break;

            } // switch

        }  // WriteObject

    } // SerializationWriter


    /// <summary>
    /// Classe per Deserializzazione
    /// </summary>
    public class SerializationReader
    {

        private BinaryReader mBinReader;

        public SerializationReader (Stream s) 
        {
            this.mBinReader = new BinaryReader(s);
        }


        /// <summary>
        /// Legge array di byte
        /// </summary>
        /// <returns></returns>
        private byte[] readByteArray () 
        {
            int len = this.mBinReader.ReadInt32();
            if (len > 0)
                return this.mBinReader.ReadBytes(len);
            else
                return new byte[0];
        }

        /// <summary>
        /// Legge array di caratteri
        /// </summary>
        /// <returns></returns>
        private char[] readCharArray () 
        {
            int len = this.mBinReader.ReadInt32();
            if (len > 0)
                return this.mBinReader.ReadChars(len);
            else
                return new char[0];
        }

        /// <summary>
        /// Legge dal flusso un array di oggetti
        /// </summary>
        /// <returns></returns>
        private object[] readObjectArray()
        {
            int len = this.mBinReader.ReadInt32();

            object[] ret = new object[len];

            for (int i = 0; i < len; i++)
            {
                ret[i] = this.ReadObject();
            }

            return ret;
        }



        /// <summary>
        /// Legge oggetto
        /// </summary>
        /// <returns></returns>
        public object ReadObject () 
        {

            ObjType t = (ObjType)this.mBinReader.ReadByte();

            switch (t) 
            {
            case ObjType.boolType: 
                return this.mBinReader.ReadBoolean();

            case ObjType.byteType: 
                return this.mBinReader.ReadByte();

            case ObjType.uint16Type: 
                return this.mBinReader.ReadUInt16();

            case ObjType.uint32Type: 
                return this.mBinReader.ReadUInt32();

            case ObjType.uint64Type: 
                return this.mBinReader.ReadUInt64();

            case ObjType.sbyteType: 
                return this.mBinReader.ReadSByte();

            case ObjType.int16Type: 
                return this.mBinReader.ReadInt16();

            case ObjType.int32Type: 
                return this.mBinReader.ReadInt32();

            case ObjType.int64Type: 
                return this.mBinReader.ReadInt64();

            case ObjType.charType: 
                return this.mBinReader.ReadChar();

            case ObjType.stringType: 
                return this.mBinReader.ReadString();

            case ObjType.singleType: 
                return this.mBinReader.ReadSingle();

            case ObjType.doubleType: 
                return this.mBinReader.ReadDouble();

            case ObjType.decimalType: 
                return this.mBinReader.ReadDecimal();

            case ObjType.dateTimeType: 
                return DateTime.FromBinary(this.mBinReader.ReadInt64());

            case ObjType.byteArrayType : 
                return this.readByteArray();

            case ObjType.charArrayType: 
                return this.readCharArray();

            case ObjType.objectArrayType: 
                return this.readObjectArray();

            case ObjType.otherType: 
                return new BinaryFormatter().Deserialize (this.mBinReader.BaseStream);

            default: 
                return null;
            }
        }


    } // SerializationReader

} // namespace
