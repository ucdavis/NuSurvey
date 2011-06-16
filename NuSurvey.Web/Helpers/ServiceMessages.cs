using System;
using System.ServiceModel;
using System.Web;
using System.Web.Caching;
using System.Web.Configuration;
using System.Web.Mvc;

namespace NuSurvey.Web.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class LocServiceMessageAttribute : ActionFilterAttribute
    {
        private readonly string _appName;

        public LocServiceMessageAttribute(string appName)
        {
            _appName = appName;
        }

        private string _cacheKey;

        public string CacheKey
        {
            get { return _cacheKey ?? "ServiceMessages"; }
            set { _cacheKey = value; }
        }

        /// <summary>
        /// Set the cache absolute cache expiration in seconds.  Default is one day from now.
        /// </summary>
        public int CacheExpirationInSeconds { get; set; }

        private TimeSpan CacheExpiration
        {
            get
            {
                return CacheExpirationInSeconds == default(int)
                           ? TimeSpan.FromDays(1)
                           : TimeSpan.FromSeconds(CacheExpirationInSeconds);
            }
        }

        /// <summary>
        /// Full qualified Url to the message service.  Either MessageServiceAppSettingsKey or MessageServiceAppSettingsKey must be set.
        /// </summary>
        public string MessageServiceUrl { get; set; }

        /// <summary>
        /// AppSettings key which holds the servicel Url. Either MessageServiceAppSettingsKey or MessageServiceAppSettingsKey must be set.
        /// </summary>
        public string MessageServiceAppSettingsKey { get; set; }

        /// <summary>
        /// If provided, ViewData["ViewDataKey"] will contain a null-safe array of services messages.
        /// </summary>
        public string ViewDataKey { get; set; }

        private string ServiceUrl
        {
            get
            {
                if (MessageServiceUrl == null && MessageServiceAppSettingsKey == null)
                    throw new InvalidOperationException(
                        "Either MessageServiceAppSettingsKey or MessageServiceAppSettingsKey must be set.");

                if (MessageServiceUrl != null && MessageServiceAppSettingsKey != null)
                    throw new InvalidOperationException(
                        "Either MessageServiceAppSettingsKey or MessageServiceAppSettingsKey must be set, but not both.");

                return MessageServiceUrl ?? WebConfigurationManager.AppSettings[MessageServiceAppSettingsKey];
            }
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var cache = HttpRuntime.Cache;

            if (!string.IsNullOrWhiteSpace(ViewDataKey))
            {
                filterContext.Controller.ViewData[ViewDataKey] = cache[CacheKey] ?? new ServiceMessage[0];
            }

            if (cache[CacheKey] != null) return;

            if (string.IsNullOrWhiteSpace(ServiceUrl))
                throw new InvalidOperationException(
                    "Service Url is not valid:  Please set either MessageServiceAppSettingsKey or MessageServiceAppSettingsKey");

            cache[CacheKey] = new ServiceMessage[0];

            var binding = new BasicHttpBinding();

            if (ServiceUrl.StartsWith("https://")) binding.Security.Mode = BasicHttpSecurityMode.Transport;

            var client = new MessageServiceClient(binding, new EndpointAddress(ServiceUrl));

            client.BeginGetMessages(_appName, OnMessagesRecieved, client);
        }

        private void OnMessagesRecieved(IAsyncResult ar)
        {
            var client = (MessageServiceClient)ar.AsyncState;

            try
            {
                // Get the results.
                var messages = client.EndGetMessages(ar);

                // Insert into the cache
                HttpRuntime.Cache.Insert(CacheKey, messages, null, DateTime.Now.Add(CacheExpiration), Cache.NoSlidingExpiration);
            }
            finally
            {
                //Close the client
                client.Close();
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "ServiceMessage", Namespace = "http://schemas.datacontract.org/2004/07/Catbert4.Services.Wcf")]
    [System.SerializableAttribute()]
    public partial class ServiceMessage : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged
    {

        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool CriticalField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MessageField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool GlobalField;

        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool Critical
        {
            get
            {
                return this.CriticalField;
            }
            set
            {
                if ((this.CriticalField.Equals(value) != true))
                {
                    this.CriticalField = value;
                    this.RaisePropertyChanged("Critical");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool Global
        {
            get
            {
                return this.GlobalField;
            }
            set
            {
                if ((this.GlobalField.Equals(value) != true))
                {
                    this.GlobalField = value;
                    this.RaisePropertyChanged("Global");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Message
        {
            get
            {
                return this.MessageField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MessageField, value) != true))
                {
                    this.MessageField = value;
                    this.RaisePropertyChanged("Message");
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }


    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace = "https://secure.caes.ucdavis.edu/Catbert4", ConfigurationName = "ServiceReference1.IMessageService")]
    public interface IMessageService
    {

        [System.ServiceModel.OperationContractAttribute(Action = "https://secure.caes.ucdavis.edu/Catbert4/IMessageService/GetMessages", ReplyAction = "https://secure.caes.ucdavis.edu/Catbert4/IMessageService/GetMessagesResponse")]
        ServiceMessage[] GetMessages(string appName);

        [System.ServiceModel.OperationContractAttribute(AsyncPattern = true, Action = "https://secure.caes.ucdavis.edu/Catbert4/IMessageService/GetMessages", ReplyAction = "https://secure.caes.ucdavis.edu/Catbert4/IMessageService/GetMessagesResponse")]
        System.IAsyncResult BeginGetMessages(string appName, System.AsyncCallback callback, object asyncState);

        ServiceMessage[] EndGetMessages(System.IAsyncResult result);

        [System.ServiceModel.OperationContractAttribute(Action = "https://secure.caes.ucdavis.edu/Catbert4/IMessageService/Repeat", ReplyAction = "https://secure.caes.ucdavis.edu/Catbert4/IMessageService/RepeatResponse")]
        string Repeat(string n);

        [System.ServiceModel.OperationContractAttribute(AsyncPattern = true, Action = "https://secure.caes.ucdavis.edu/Catbert4/IMessageService/Repeat", ReplyAction = "https://secure.caes.ucdavis.edu/Catbert4/IMessageService/RepeatResponse")]
        System.IAsyncResult BeginRepeat(string n, System.AsyncCallback callback, object asyncState);

        string EndRepeat(System.IAsyncResult result);
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IMessageServiceChannel : IMessageService, System.ServiceModel.IClientChannel
    {
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class GetMessagesCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        public GetMessagesCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        public ServiceMessage[] Result
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return ((ServiceMessage[])(this.results[0]));
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class RepeatCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        public RepeatCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        public string Result
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class MessageServiceClient : System.ServiceModel.ClientBase<IMessageService>, IMessageService
    {

        private BeginOperationDelegate onBeginGetMessagesDelegate;

        private EndOperationDelegate onEndGetMessagesDelegate;

        private System.Threading.SendOrPostCallback onGetMessagesCompletedDelegate;

        private BeginOperationDelegate onBeginRepeatDelegate;

        private EndOperationDelegate onEndRepeatDelegate;

        private System.Threading.SendOrPostCallback onRepeatCompletedDelegate;

        public MessageServiceClient()
        {
        }

        public MessageServiceClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        public MessageServiceClient(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public MessageServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public MessageServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        public event System.EventHandler<GetMessagesCompletedEventArgs> GetMessagesCompleted;

        public event System.EventHandler<RepeatCompletedEventArgs> RepeatCompleted;

        public ServiceMessage[] GetMessages(string appName)
        {
            return base.Channel.GetMessages(appName);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginGetMessages(string appName, System.AsyncCallback callback, object asyncState)
        {
            return base.Channel.BeginGetMessages(appName, callback, asyncState);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public ServiceMessage[] EndGetMessages(System.IAsyncResult result)
        {
            return base.Channel.EndGetMessages(result);
        }

        private System.IAsyncResult OnBeginGetMessages(object[] inValues, System.AsyncCallback callback, object asyncState)
        {
            string appName = ((string)(inValues[0]));
            return this.BeginGetMessages(appName, callback, asyncState);
        }

        private object[] OnEndGetMessages(System.IAsyncResult result)
        {
            ServiceMessage[] retVal = this.EndGetMessages(result);
            return new object[] {
                    retVal};
        }

        private void OnGetMessagesCompleted(object state)
        {
            if ((this.GetMessagesCompleted != null))
            {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.GetMessagesCompleted(this, new GetMessagesCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }

        public void GetMessagesAsync(string appName)
        {
            this.GetMessagesAsync(appName, null);
        }

        public void GetMessagesAsync(string appName, object userState)
        {
            if ((this.onBeginGetMessagesDelegate == null))
            {
                this.onBeginGetMessagesDelegate = new BeginOperationDelegate(this.OnBeginGetMessages);
            }
            if ((this.onEndGetMessagesDelegate == null))
            {
                this.onEndGetMessagesDelegate = new EndOperationDelegate(this.OnEndGetMessages);
            }
            if ((this.onGetMessagesCompletedDelegate == null))
            {
                this.onGetMessagesCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnGetMessagesCompleted);
            }
            base.InvokeAsync(this.onBeginGetMessagesDelegate, new object[] {
                        appName}, this.onEndGetMessagesDelegate, this.onGetMessagesCompletedDelegate, userState);
        }

        public string Repeat(string n)
        {
            return base.Channel.Repeat(n);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginRepeat(string n, System.AsyncCallback callback, object asyncState)
        {
            return base.Channel.BeginRepeat(n, callback, asyncState);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public string EndRepeat(System.IAsyncResult result)
        {
            return base.Channel.EndRepeat(result);
        }

        private System.IAsyncResult OnBeginRepeat(object[] inValues, System.AsyncCallback callback, object asyncState)
        {
            string n = ((string)(inValues[0]));
            return this.BeginRepeat(n, callback, asyncState);
        }

        private object[] OnEndRepeat(System.IAsyncResult result)
        {
            string retVal = this.EndRepeat(result);
            return new object[] {
                    retVal};
        }

        private void OnRepeatCompleted(object state)
        {
            if ((this.RepeatCompleted != null))
            {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.RepeatCompleted(this, new RepeatCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }

        public void RepeatAsync(string n)
        {
            this.RepeatAsync(n, null);
        }

        public void RepeatAsync(string n, object userState)
        {
            if ((this.onBeginRepeatDelegate == null))
            {
                this.onBeginRepeatDelegate = new BeginOperationDelegate(this.OnBeginRepeat);
            }
            if ((this.onEndRepeatDelegate == null))
            {
                this.onEndRepeatDelegate = new EndOperationDelegate(this.OnEndRepeat);
            }
            if ((this.onRepeatCompletedDelegate == null))
            {
                this.onRepeatCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnRepeatCompleted);
            }
            base.InvokeAsync(this.onBeginRepeatDelegate, new object[] {
                        n}, this.onEndRepeatDelegate, this.onRepeatCompletedDelegate, userState);
        }
    }

}