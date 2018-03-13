using J4Net.Core;
using System.Text;
using System;

namespace Kontur.Net2Java.JavaProxy
{
    public class Throwable : IDisposable
    {
        private LocalRef jObject;

        private const string CLASS_NAME = "java/lang/Throwable";
        private const string GET_MESSAGE_METHOD_SIGNATURE = "()Ljava/lang/String;";
        private const string GET_CLASS_METHOD_SIGNATURE = "()Ljava/lang/Class;";
        private const string GET_STACK_TRACE_METHOD_SIGNATURE = "()[Ljava/lang/StackTraceElement;";

        private static GlobalRef classPtr;
        private static GlobalRef getStackTraceMethod;
        private static GlobalRef getMessageMethod;
        private static GlobalRef getClassMethod;
        private static JniEnvWrapper Env { get { return JvmManager.INSTANCE.GetEnv(); } }
        private static readonly object monitor = new object();

        public Throwable(LocalRef jObject)
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

        private static GlobalRef GetMessageMethod
        {
            get
            {
                if (getMessageMethod != null)
                {
                    return getMessageMethod;
                }

                lock (monitor)
                {
                    if (getMessageMethod == null)
                    {
                        getMessageMethod = GetMethod("getMessage", GET_MESSAGE_METHOD_SIGNATURE);
                    }

                    return getMessageMethod;
                }
            }
        }

        private static GlobalRef GetClassMethod
        {
            get
            {
                if (getClassMethod != null)
                {
                    return getClassMethod;
                }

                lock (monitor)
                {
                    if (getClassMethod == null)
                    {
                        getClassMethod = GetMethod("getClass", GET_CLASS_METHOD_SIGNATURE);
                    }

                    return getClassMethod;
                }
            }
        }

        private static GlobalRef GetStackTraceMethod
        {
            get
            {
                if (getStackTraceMethod != null)
                {
                    return getStackTraceMethod;
                }

                lock (monitor)
                {
                    if (getStackTraceMethod == null)
                    {
                        getStackTraceMethod = GetMethod("getStackTrace", GET_STACK_TRACE_METHOD_SIGNATURE);
                    }

                    return getStackTraceMethod;
                }
            }
        }

        public string GetCause()
        {
            using (var exceptionClass = new Class(Env.CallObjectMethod(jObject.Ptr, GetClassMethod.Ptr)))
            {
                return exceptionClass.GetName();
            }
        }

        public string GetMessage()
        {
            using (var messagePtr = Env.CallObjectMethod(jObject.Ptr, GetMessageMethod.Ptr))
            {
                return messagePtr.IsZero() ? "" : Env.GetString(messagePtr.Ptr);
            }
        }

        public string GetStackTrace()
        {
            var stackTrace = new StringBuilder();
            stackTrace.Append(GetCause());
            stackTrace.Append(": ");
            stackTrace.Append(GetMessage());
            stackTrace.AppendLine();

            using (var stackTraceArrayPtr = Env.CallObjectMethod(jObject.Ptr, GetStackTraceMethod.Ptr))
            {
                var st = Env.GetArray(stackTraceArrayPtr.Ptr);

                foreach (var stackTraceElementPtr in st)
                {
                    using (var stackTraceElement = new StackTraceElement(stackTraceElementPtr))
                    {
                        stackTrace.Append("\t at ");
                        stackTrace.Append(stackTraceElement.GetClassName());
                        stackTrace.Append(".");
                        stackTrace.Append(stackTraceElement.GetMethodName());
                        stackTrace.Append("(");
                        stackTrace.Append(stackTraceElement.GetFileName());
                        stackTrace.Append(":");
                        stackTrace.Append(stackTraceElement.GetLineNumber());
                        stackTrace.Append(")");
                        stackTrace.AppendLine();
                    }
                }
            }

            return stackTrace.ToString();
        }

        private static GlobalRef GetMethod(string name, string signature)
        {
            return Env.GetMethodId(ClassPtr.Ptr,
                name, signature);
        }

        public void Dispose()
        {
            if (jObject != null)
            {
                jObject.Dispose();
            }
        }
    }
}