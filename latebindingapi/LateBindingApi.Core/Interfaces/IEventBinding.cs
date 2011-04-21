using System;
using System.ComponentModel;
namespace LateBindingApi.Core
{
    public interface IEventBinding
    {
        /// <summary>
        /// returns array of all event listeners
        /// </summary>
        /// <param name="name">name of event</param>
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        Delegate[] GetEventRecipients(string eventName);
        
        /// <summary>
        /// Dispose method
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        void DisposeSinkHelper();
    }
}
