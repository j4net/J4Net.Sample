using J4Net.Core;
using System;

namespace Kontur.Net2Java.JavaProxy
{
    public class Class : IDisposable
    {
        private LocalRef jObject;

        private const string CLASS_NAME = "java/lang/Class";
        private const string GET_NAME_METHOD_SIGNATURE = "()Ljava/lang/String;";
        private static GlobalRef classPtr;
        private static GlobalRef getNameMethod;
        private static JniEnvWrapper Env { get { return JvmManager.INSTANCE.GetEnv(); } }
        private static readonly object monitor = new object();

        public Class(LocalRef jObject)
        {
            this.jObject = jObject;
        }
        private static GlobalRef ClassPtr
        {
            get
            {
                if (classPtr != null)
                {
                    return classPtr;
                }

                lock (monitor)
                {
                    if (classPtr == null)
                    {
                        classPtr = JvmManager.INSTANCE.GetEnv().FindClass(CLASS_NAME);
                    }

                    return classPtr;
                }
            }
        }
        public string GetName()
        {
            using (var obj = Env.CallObjectMethod(jObject.Ptr, GetNameMethod.Ptr))
            {
                return Env.GetString(obj.Ptr);
            }
        }

        private static GlobalRef GetNameMethod
        {
            get
            {
                if (getNameMethod != null)
                {
                    return getNameMethod;
                }

                lock (monitor)
                {
                    if (getNameMethod == null)
                    {
                        getNameMethod = GetMethod("getName", GET_NAME_METHOD_SIGNATURE);
                    }

                    return getNameMethod;
                }
            }
        }

        public virtual void Dispose()
        {
            if (jObject != null)
            {
                jObject.Dispose();
            }
        }

        private static GlobalRef GetMethod(string name, string signature)
        {
            return Env.GetMethodId(ClassPtr.Ptr,
                name, signature);
        }
    }
}