using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;

namespace AutoGen.App
{
    public class ObjectFormatter
    {
        public static byte[] FormatObject(object obj)
        {
            return BinaryFormatObject(obj);
        }

        public static object GetObject(byte[] buffer)
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            object res;
            try
            {
                res = BinaryGetObject(buffer);
            } catch(Exception ex)
            {
                throw new Exception("Ошибка десереализации", ex);
            }
            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
            return res;
        }

        public static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            Assembly[] currAss = AppDomain.CurrentDomain.GetAssemblies();
            //try full names
            for (int i = 0; i < currAss.Length;i++ )
            {
                if (currAss[i].FullName.Equals(args.Name))
                    return currAss[i];
            }
            //try only assembly name
            for (int i = 0; i < currAss.Length; i++)
            {
                char[] splitter = {','};
                string[] names = currAss[i].FullName.Split(splitter, StringSplitOptions.None);
                string[] resNames = args.Name.Split(splitter, StringSplitOptions.None);
                if (names.Length > 0 && names[0].Equals(resNames[0]))
                    return currAss[i];
            }
            return Assembly.GetExecutingAssembly();
        }

        private static byte[] SOAPFormatObject(object obj)
        {
            SoapFormatter sf = new SoapFormatter();
            MemoryStream ms = new MemoryStream();
            sf.Serialize(ms, obj);
            ms.Seek(0, SeekOrigin.Begin);
            byte[] buff = ms.ToArray();
            ms.Close();
            return buff;
        }

        private static object SOAPGetObject(byte[] buffer)
        {
            SoapFormatter sf = new SoapFormatter();
            MemoryStream ms = new MemoryStream();
            ms.Write(buffer, 0, buffer.Length);
            ms.Seek(0, SeekOrigin.Begin);
            object res = sf.Deserialize(ms);
            ms.Close();
            return res;
        }

        private static byte[] BinaryFormatObject(object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);
            ms.Seek(0, SeekOrigin.Begin);
            byte[] buff = ms.ToArray();
            ms.Close();
            return buff;
        }

        private static object BinaryGetObject(byte[] buffer)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            ms.Write(buffer, 0, buffer.Length);
            ms.Seek(0, SeekOrigin.Begin);
            object res = bf.Deserialize(ms);
            ms.Close();
            return res;
        }
    }
}
