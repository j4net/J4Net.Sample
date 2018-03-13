using System;
using J4Net.Core.JNICore.Interface;
using J4Net.Core;

namespace Kontur.Net2Java.JavaProxy
{
    public class EnvelopeBuilder : IDisposable
    {
        private const string CLASS_NAME = "ru/kontur/alfa/integration/envelope/EnvelopeBuilder";
        private const string ADD_DOCUMENT_METHOD_SIGNATURE = "([B[BI)V";
        private const string CONSTRUCTOR_SIGNATURE = "(SSLjava/lang/String;Ljava/lang/String;Ljava/lang/String;[BI)V";
        private const string BUILD_METHOD_SIGNATURE = "()[B";

        private static GlobalRef classPtr;
        private static GlobalRef addDocumentMethod;
        private static GlobalRef constructor;
        private static GlobalRef buildMethod;

        private static JniEnvWrapper Env { get { return JvmManager.INSTANCE.GetEnv(); } }

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
                        classPtr = JvmManager.INSTANCE.GetEnv().FindClass(CLASS_NAME);
                    }

                    return classPtr;
                }
            }
        }

        private static GlobalRef AddDocumentMethod
        {
            get
            {
                if (addDocumentMethod != null)
                {
                    return addDocumentMethod;
                }

                lock (monitor)
                {
                    if (addDocumentMethod == null)
                    {
                        addDocumentMethod = GetMethod("addDocument", ADD_DOCUMENT_METHOD_SIGNATURE);
                    }

                    return addDocumentMethod;
                }
            }
        }

        private static GlobalRef BuildMethod
        {
            get
            {
                if (buildMethod != null)
                {
                    return buildMethod;
                }

                lock (monitor)
                {
                    if (buildMethod == null)
                    {
                        buildMethod = GetMethod("build", BUILD_METHOD_SIGNATURE);
                    }

                    return buildMethod;
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

        public EnvelopeBuilder(short sourceSystemId,
            short targetSystemId,
            string sellerInn,
            string byerInn,
            string supplyAgreementNumber,
            byte[] secretKey,
            int documentsCount)
        {
            using (var sellerInnUtf = Env.NewStringUtf(sellerInn))
            {
                using (var byerInnUtf = Env.NewStringUtf(byerInn))
                {
                    using (var supplyAgreementNumberUtf = Env.NewStringUtf(supplyAgreementNumber))
                    {
                        using (var secretKeyPtr = Env.NewByteArray(secretKey))
                        {
                            jObject = Env.NewObject(ClassPtr.Ptr, Constructor.Ptr,
                                new JValue { ShortValue = sourceSystemId },
                                new JValue { ShortValue = targetSystemId },
                                new JValue { PointerValue = sellerInnUtf.Ptr },
                                new JValue { PointerValue = byerInnUtf.Ptr },
                                new JValue { PointerValue = supplyAgreementNumberUtf.Ptr },
                                new JValue { PointerValue = secretKeyPtr.Ptr },
                                new JValue { IntegerValue = documentsCount }
                            );

                            HandleException();
                        }
                    }
                }
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
        public EnvelopeBuilder(GlobalRef jObject)
        {
            this.jObject = jObject;
        }

        public void AddDocument(byte[] data, byte[] signature, int formatCode)
        {
            using (var dataA = Env.NewByteArray(data))
            {
                using (var signatureA = Env.NewByteArray(signature))
                {
                    Env.CallVoidMethod(jObject.Ptr, AddDocumentMethod.Ptr,
                        new JValue { PointerValue = dataA.Ptr },
                        new JValue { PointerValue = signatureA.Ptr },
                        new JValue { IntegerValue = formatCode }
                    );

                    HandleException();
                }
            }
        }

        public byte[] Build()
        {
            using (
                var result = Env.CallObjectMethod(jObject.Ptr, BuildMethod.Ptr))
            {
                HandleException();
                return Env.GetByteArray(result.Ptr);
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