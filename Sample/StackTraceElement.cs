using J4Net.Core;
using System;

namespace Kontur.Net2Java.JavaProxy
{
    public class StackTraceElement : IDisposable
    {
        private LocalRef jObject;

        private const string CLASS_NAME = "java/lang/StackTraceElement";
        private const string GET_CLASS_NAME_METHOD_SIGNATURE = "()Ljava/lang/String;";
        private const string GET_METHOD_NAME_METHOD_SIGNATURE = "()Ljava/lang/String;";
        private const string GET_FILE_NAME_METHOD_SIGNATURE = "()Ljava/lang/String;";
        private const string GET_LINE_NUMBER_METHOD_SIGNATURE = "()I";


        private static GlobalRef classPtr;
        private static GlobalRef getClassNameMethod;
        private static GlobalRef getMethodNameMethod;
        private static GlobalRef getFileNameMethod;
        private static GlobalRef getLineNumberMethod;
        private static JniEnvWrapper Env { get { return JvmManager.INSTANCE.GetEnv(); } }
        private static readonly object monitor = new object();

        public StackTraceElement(LocalRef jObject)
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

        private static GlobalRef GetClassNameMethod
        {
            get
            {
                if (getClassNameMethod != null)
                {
                    return getClassNameMethod;
                }

                lock (monitor)
                {
                    if (getClassNameMethod == null)
                    {
                        getClassNameMethod = GetMethod("getClassName", GET_CLASS_NAME_METHOD_SIGNATURE);
                    }

                    return getClassNameMethod;
                }
            }
        }

        private static GlobalRef GetMethodNameMethod
        {
            get
            {
                if (getMethodNameMethod != null)
                {
                    return getMethodNameMethod;
                }

                lock (monitor)
                {
                    if (getMethodNameMethod == null)
                    {
                        getMethodNameMethod = GetMethod("getMethodName", GET_METHOD_NAME_METHOD_SIGNATURE);
                    }

                    return getMethodNameMethod;
                }
            }
        }

        private static GlobalRef GetFileNameMethod
        {
            get
            {
                if (getFileNameMethod != null)
                {
                    return getFileNameMethod;
                }

                lock (monitor)
                {
                    if (getFileNameMethod == null)
                    {
                        getFileNameMethod = GetMethod("getFileName", GET_FILE_NAME_METHOD_SIGNATURE);
                    }

                    return getFileNameMethod;
                }
            }
        }

        private static GlobalRef GetLineNumberMethod
        {
            get
            {
                if (getLineNumberMethod != null)
                {
                    return getLineNumberMethod;
                }

                lock (monitor)
                {
                    if (getLineNumberMethod == null)
                    {
                        getLineNumberMethod = GetMethod("getLineNumber", GET_LINE_NUMBER_METHOD_SIGNATURE);
                    }

                    return getLineNumberMethod;
                }
            }
        }

        private static GlobalRef GetMethod(string name, string signature)
        {
            return Env.GetMethodId(ClassPtr.Ptr,
                name, signature);
        }

        public string GetClassName()
        {
            using (var messagePtr = Env.CallObjectMethod(jObject.Ptr, GetClassNameMethod.Ptr))
            {
                return messagePtr.IsZero() ? "" : Env.GetString(messagePtr.Ptr);
            }
        }

        public string GetMethodName()
        {
            using (var messagePtr = Env.CallObjectMethod(jObject.Ptr, GetMethodNameMethod.Ptr))
            {
                return messagePtr.IsZero() ? "" : Env.GetString(messagePtr.Ptr);
            }
        }

        public string GetFileName()
        {
            using (var messagePtr = Env.CallObjectMethod(jObject.Ptr, GetFileNameMethod.Ptr))
            {
                return messagePtr.IsZero() ? "" : Env.GetString(messagePtr.Ptr);
            }
        }

        public int GetLineNumber()
        {
            return Env.CallIntMethod(jObject.Ptr, GetLineNumberMethod.Ptr);
        }

        public virtual void Dispose()
        {
            if (jObject != null)
            {
                jObject.Dispose();
            }
        }
    }
}