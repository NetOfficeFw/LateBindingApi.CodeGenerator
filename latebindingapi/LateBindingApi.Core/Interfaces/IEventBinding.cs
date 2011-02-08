using System;
using System.ComponentModel;
namespace LateBindingApi.Core
{
    public interface IEventBinding
    {
        /// <summary>
        /// Raise an event
        /// </summary>
        /// <param name="name">event name</param>
        /// <param name="paramArray">params as array</param>
        /// <returns>indicates one or more eventlistener has recieved the event</returns>
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        bool CallEvent(string name, object[] paramArray);
        
        /// <summary>
        /// Dispose method
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        void DisposeSinkHelper();
    }
}
