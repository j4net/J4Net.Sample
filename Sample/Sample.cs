using J4Net.Core;
using J4Net.Core.JNICore.Interface;
using Kontur.Net2Java.JavaProxy;
using System;

namespace Sample
{
    public class Sample
    {
        private const string CLASS_NAME = "ru/sample/Sample";
        private const string INC_METHOD_SIGNATURE = "(I)I";
        private const string CONSTRUCTOR_SIGNATURE = "(IILjava/lang/String;)V";
        private const string PRINT_METHOD_SIGNATURE = "(Ljava/lang/String;)V";

        private static GlobalRef classPtr;
        private static GlobalRef incMethod;
        private static GlobalRef constructor;
        private static GlobalRef printMethod;

        private static JniEnvWrapper Env
        {
            get { return JvmManager.INSTANCE.GetEnv(); }
        }

        private readonly GlobalRef jObject;
        private static readonly object monitor = new object();

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
                        //use signature
                        classPtr = JvmManager.INSTANCE.GetEnv().FindClass(CLASS_NAME);
                    }

                    return classPtr;
                }
            }
        }

        //release objects

        private static GlobalRef IncMethod
        {
            get
            {
                if (incMethod != null)
                {
                    return incMethod;
                }

                lock (monitor)
                {
                    if (incMethod == null)
                    {
                        incMethod = GetMethod("inc", INC_METHOD_SIGNATURE);
                    }

                    return incMethod;
                }
            }
        }

        private static GlobalRef PrintMethod
        {
            get
            {
                if (printMethod != null)
                {
                    return printMethod;
                }

                lock (monitor)
                {
                    if (printMethod == null)
                    {
                        printMethod = GetStaticMethod("print", PRINT_METHOD_SIGNATURE);
                    }

                    return printMethod;
                }
            }
        }

        private static GlobalRef Constructor
        {
            get
            {
                if (constructor != null)
                {
                    return constructor;
                }

                lock (monitor)
                {
                    if (constructor == null)
                    {
                        constructor = GetMethod("<init>", CONSTRUCTOR_SIGNATURE);
                    }

                    return constructor;
                }
            }
        }

        public Sample(int x, int y, string str)
        {
            //todo release string
            using (var strUtf = Env.NewStringUtf(str))
            {
                jObject = Env.NewObject(ClassPtr.Ptr, Constructor.Ptr,
                    new JValue {IntegerValue = x},
                    new JValue {IntegerValue = y},
                    new JValue {PointerValue = strUtf.Ptr}
                );

                HandleException();
            }
        }

        private void HandleException()
        {
            LocalRef exception = null;
            if (Env.TryCatchEcxeption(out exception))
            {
                string stackTrace = null;
                try
                {
                    stackTrace = new Throwable(exception).GetStackTrace();
                }
                finally
                {
                    exception.Dispose();
                }

                Env.ClearContext();
                throw new Exception(stackTrace);
            }
        }

        public Sample(GlobalRef jObject)
        {
            this.jObject = jObject;
        }

        public int Inc(int a)
        {
            try
            {
                //генерим JValue
                return Env.CallIntMethod(jObject.Ptr, IncMethod.Ptr, new JValue {IntegerValue = a});
            }
            finally
            {
                HandleException();
            }
        }

        public static void Print(string text)
        {
            using (var textUtf = Env.NewStringUtf(text))
            {
                Env.CallStaticVoidMethod(ClassPtr.Ptr, PrintMethod.Ptr, new JValue {PointerValue = textUtf.Ptr});
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

        private static GlobalRef GetStaticMethod(string name, string signature)
        {
            return Env.GetStaticMethodId(ClassPtr.Ptr,
                name, signature);
        }
    }
}