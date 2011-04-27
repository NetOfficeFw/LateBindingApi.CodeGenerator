﻿// Generated by LateBindingApi.CodeGenerator
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using ComTypes = System.Runtime.InteropServices.ComTypes;

namespace LateBindingApi.Core
{
    /// <summary>
    /// Sink Helper Base Class for an Event Interface Sink helper class
    /// </summary>
    public abstract class SinkHelper : IDisposable
    {
        private static List<SinkHelper> _pointList = new List<SinkHelper>();

        #region Fields

        private COMObject _eventClass;
        private IConnectionPoint _connectionPoint;
        private int _connectionCookie;
        private Guid _interfaceId;

        #endregion

        #region Construction

        public SinkHelper(COMObject eventClass)
        {
            _eventClass = eventClass;
        }

        #endregion

        #region Static Methods

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public static string GetConnectionPoint(COMObject comProxy, ref IConnectionPoint point, params string[] sinkIds)
        {
            if (null == sinkIds)
                return null;

            IConnectionPointContainer connectionPointContainer = (IConnectionPointContainer)comProxy.UnderlyingObject;
            IEnumConnectionPoints enumPoints = null;
            connectionPointContainer.EnumConnectionPoints(out enumPoints);
            IConnectionPoint[] points = new IConnectionPoint[1];
            while (enumPoints.Next(1, points, IntPtr.Zero) == 0) // S_OK = 0 , S_FALSE = 1
            {
                if (null == points[0])
                    break;

                Guid interfaceGuid;
                points[0].GetConnectionInterface(out interfaceGuid);

                for (int i = sinkIds.Length; i > 0; i--)
                {
                    string id = interfaceGuid.ToString().Replace("{", "").Replace("}", "");
                    if (true == sinkIds[i - 1].Equals(id, StringComparison.InvariantCultureIgnoreCase))
                    {
                        Marshal.ReleaseComObject(enumPoints);
                        point = points[0];
                        return id;
                    }
                }
            }

            Marshal.ReleaseComObject(enumPoints);
            return null;
        }

        /// <summary>
        /// Dispose all active event bridges
        /// </summary>
        public static void DisposeAll()
        {
            foreach (SinkHelper point in _pointList)
                point.RemoveEventBinding(false);
            _pointList.Clear();
        }

        #endregion

        #region Public Methods

        public void SetupEventBinding(IConnectionPoint connectPoint)
        {
            if (true == Settings.EnableEvents)
            {
                connectPoint.GetConnectionInterface(out _interfaceId);
                _connectionPoint = connectPoint;
                _connectionPoint.Advise(this, out _connectionCookie);
                _pointList.Add(this);
            }
        }

        public void RemoveEventBinding()
        {
            RemoveEventBinding(true);
        }

        private void RemoveEventBinding(bool removeFromList)
        {
            if (_connectionCookie != 0)
            {
                try
                {
                    _connectionPoint.Unadvise(_connectionCookie);
                    Marshal.ReleaseComObject(_connectionPoint);
                }
                catch (System.Runtime.InteropServices.COMException loE)
                {
                    if ((uint)loE.ErrorCode != 0x800706BA)
                        throw (loE);
                }

                _connectionPoint = null;
                _connectionCookie = 0;

                if(removeFromList)
                    _pointList.Remove(this);
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            RemoveEventBinding();
        }

        #endregion
    }
}
