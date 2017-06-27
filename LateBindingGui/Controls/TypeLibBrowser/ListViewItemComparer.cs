using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;

namespace LateBindingApi.CodeGenerator.WFApplication.Controls.TypeLibBrowser
{   
    #region Enums

    public enum ColumnType
    {
        TypeString = 0,
        TypeInteger = 1,
        TypeDateTime = 2,
        TypeStringLex = 3
    }

    #endregion

    public class ColumnTypeInfo
    {
        #region Fields

        private int _columnIndex;
        private ColumnType _valueType;

        #endregion

        #region Properties

        public int ColumnIndex
        {
            get
            {
                return _columnIndex;
            }
            set
            {
                _columnIndex = value;
            }
        }

        public ColumnType ValueType
        {
            get
            {
                return _valueType;
            }
            set
            {
                _valueType = value;
            }
        }

        #endregion

        #region Construction

        public ColumnTypeInfo(int index, ColumnType typ)
        {
            ColumnIndex = index;
            ValueType = typ;
        }

        #endregion
    }

    public class ListViewItemComparer : IComparer
    {
        #region Fields

        private int       _sortColumnIndex;
        private SortOrder _sortOrder;

        private CaseInsensitiveComparer _objectCompare = new CaseInsensitiveComparer();
        private List<ColumnTypeInfo> _listColumInfo = new List<ColumnTypeInfo>();

        #endregion

        #region Properties

        public int SortingColumn
        {
            get
            {
                return _sortColumnIndex;
            }
            set
            {
                _sortColumnIndex = value;
            }
        }

        public SortOrder SortingType
        {

            get
            {
                return _sortOrder;
            }
            set
            {
                _sortOrder = value;
            }

        }

        #endregion

        #region Construction

        public ListViewItemComparer(int columnToSort)
        {
            _sortColumnIndex = columnToSort;
        }

        #endregion

        #region Public Methods

        public int AddColumnInfo(ColumnTypeInfo infoColumnType)
        {
            foreach (ColumnTypeInfo columntypeItem in _listColumInfo)
            {
                if (columntypeItem.ColumnIndex == infoColumnType.ColumnIndex)
                {
                    columntypeItem.ValueType = infoColumnType.ValueType;
                    return _listColumInfo.Count;
                }
            }
            _listColumInfo.Add(infoColumnType);
            return _listColumInfo.Count;
        }

        public void RemoveColumnInfos()
        {
            _listColumInfo.Clear();
        }

        #endregion

        #region Private Methods

        private ColumnTypeInfo GetColumnInfo(int index)
        {
            foreach (ColumnTypeInfo ct in _listColumInfo)
            {
                if (ct.ColumnIndex == index)
                    return ct;
            }
            return null;
        }

        private int CompareStringLex(ListViewItem lviX, ListViewItem lviY)
        {
            if ((lviX.SubItems.Count <= _sortColumnIndex) || (lviY.SubItems.Count <= _sortColumnIndex))
                return 0;
            int compResult = _objectCompare.Compare(lviX.SubItems[_sortColumnIndex].Text, lviY.SubItems[_sortColumnIndex].Text);
            return compResult;
        }

        private int CompareString(ListViewItem lviX, ListViewItem lviY)
        {
            if ((lviX.SubItems.Count <= _sortColumnIndex) || (lviY.SubItems.Count <= _sortColumnIndex))
                return 0;
            int compResult = _objectCompare.Compare(lviX.SubItems[_sortColumnIndex].Text, lviY.SubItems[_sortColumnIndex].Text);
            return compResult;
        }

        private int CompareDateTime(ListViewItem lviX, ListViewItem lviY)
        {
            if ((lviX.SubItems.Count <= _sortColumnIndex) || (lviY.SubItems.Count <= _sortColumnIndex))
                return 0;
            DateTime firstDate = DateTime.Parse(lviX.SubItems[_sortColumnIndex].Text);
            DateTime secondDate = DateTime.Parse(lviY.SubItems[_sortColumnIndex].Text);

            int compResult = DateTime.Compare(firstDate, secondDate);
            return compResult;
        }

        private int CompareInteger(ListViewItem lviX, ListViewItem lviY)
        {
            if ((lviX.SubItems.Count <= _sortColumnIndex) || (lviY.SubItems.Count <= _sortColumnIndex))
                return 0;
            decimal firstVal = Decimal.Parse(lviX.SubItems[_sortColumnIndex].Text);
            decimal secondVal = Decimal.Parse(lviY.SubItems[_sortColumnIndex].Text);

            int compResult = Decimal.Compare(firstVal, secondVal);
            return compResult;
        }

        #endregion

        #region IComparer Members

        int IComparer.Compare(object x, object y)
        {

            int compResult = 0;
            ListViewItem lviX, lviY;
            lviX = (ListViewItem)x;
            lviY = (ListViewItem)y;

            ColumnTypeInfo cti = GetColumnInfo(_sortColumnIndex);

            if (_sortOrder == SortOrder.Ascending)
            {
                if (cti == null)
                {
                    compResult = CompareString(lviX, lviY);
                    return compResult;
                }
                if (cti.ValueType == ColumnType.TypeInteger)
                {

                    compResult = CompareInteger(lviX, lviY);
                    return compResult;
                }
                else if (cti.ValueType == ColumnType.TypeStringLex)
                {
                    compResult = CompareStringLex(lviX, lviY);
                    return compResult;
                }
                else
                {
                    compResult = CompareString(lviX, lviY);
                    return compResult;
                }

            }
            else if (_sortOrder == SortOrder.Descending)
            {
                if (cti == null)
                {
                    compResult = CompareString(lviX, lviY);
                    return (-compResult);
                }
                if (cti.ValueType == ColumnType.TypeInteger)
                {

                    compResult = CompareInteger(lviX, lviY);
                    return (-compResult);
                }
                else if (cti.ValueType == ColumnType.TypeDateTime)
                {
                    compResult = CompareDateTime(lviX, lviY);
                    return (-compResult);
                }
                else if (cti.ValueType == ColumnType.TypeDateTime)
                {
                    compResult = CompareDateTime(lviX, lviY);
                    return (-compResult);
                }
                else
                {
                    compResult = CompareString(lviX, lviY);
                    return (-compResult);
                }
            }
            else
            {
                return 0;
            }

        }

        #endregion
    }
}
