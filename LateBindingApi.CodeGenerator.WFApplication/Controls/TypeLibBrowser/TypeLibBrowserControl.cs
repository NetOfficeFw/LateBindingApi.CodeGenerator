using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LateBindingApi.CodeGenerator.WFApplication.Controls.TypeLibBrowser
{
    public partial class TypeLibBrowserControl : UserControl
    {
        #region Fields

        private ListViewItemComparer _resultSorter = new ListViewItemComparer(0);
        private List<RegistryKey>    _entriesList  = new List<RegistryKey>();
        
        #endregion

        #region Events

        public event EventHandler SelectedIndexChanged;
        public new event EventHandler Click;
        public new event EventHandler DoubleClick;

        #endregion

        #region Properties

        public ListView.SelectedListViewItemCollection SelectedItems 
        { 
            get
            {
                return listViewTypeLibInfo.SelectedItems; 
            }
        }

        #endregion

        #region Construction

        public TypeLibBrowserControl()
        {
            InitializeComponent();
            SetupListPropertySorter(); 
            ScanTypeLibRegistry();
            ShowResultItems();
        }

        #endregion

        #region Methods

        private void SetupListPropertySorter()
        {
            _resultSorter.AddColumnInfo(new ColumnTypeInfo(0, ColumnType.TypeInteger));
            _resultSorter.AddColumnInfo(new ColumnTypeInfo(1, ColumnType.TypeString));
            _resultSorter.AddColumnInfo(new ColumnTypeInfo(2, ColumnType.TypeString));
            _resultSorter.AddColumnInfo(new ColumnTypeInfo(3, ColumnType.TypeString));
            _resultSorter.AddColumnInfo(new ColumnTypeInfo(4, ColumnType.TypeString));

            listViewTypeLibInfo.ListViewItemSorter = _resultSorter;
        }

        private void ScanTypeLibRegistry()
        {
            _entriesList.Clear();

            foreach (RegistryKey itemKey in TypeLibRegistry.Key.Keys)
            {
                _entriesList.Add(itemKey);
            }
        }
     
        private void ShowResultItems()
        {
            string filterText = textBoxFilter.Text.Trim();
            bool filterEnabled = (filterText != "");

            int i = 1;
            listViewTypeLibInfo.Items.Clear();
            foreach (RegistryKey itemKey in _entriesList)
            {
                foreach (RegistryKey itemSubKey in itemKey.Keys)
                {
                    string version = itemSubKey.Name;
                    string name = "";
                    if (itemSubKey.Entries.Count > 0)
                        name = itemSubKey.Entries[0].Value.ToString();

                    if (true == FilterIsMatched(filterEnabled, name, filterText))
                    {
                        foreach (RegistryKey itemSubSubKey in itemSubKey.Keys)
                        {
                            int iValue = -1;
                            int.TryParse(itemSubSubKey.Name, out iValue);
                            if (itemSubSubKey.Name == iValue.ToString())
                            {
                                foreach (RegistryKey itemSubSubSubKey in itemSubSubKey.Keys)
                                {
                                    ListViewItem listItem = listViewTypeLibInfo.Items.Add(i.ToString());
                                    listItem.SubItems.Add(name);
                                    listItem.SubItems.Add(version);
                                    listItem.SubItems.Add(itemSubSubSubKey.Name);
                                    if (itemSubSubSubKey.Entries.Count > 0)
                                        listItem.SubItems.Add(itemSubSubSubKey.Entries[0].Value.ToString());
                                    else
                                        listItem.SubItems.Add("<Empty>");

                                    i++;
                                }
                            }

                        }
                    }

                }

            }

        }

        private bool FilterIsMatched(bool filterEnabled, string name, string filterText)
        {
            if (filterEnabled == false) 
                return true;
            
            string[] filterArray = filterText.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string  filter in filterArray)
            {
                int stringPosition = name.IndexOf(filter, StringComparison.InvariantCultureIgnoreCase);
                if (stringPosition > -1)
                    return true;
            }

            return false;            
         }

        #endregion

        #region Gui Trigger

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            ShowResultItems();
        }

        private void textBoxFilter_KeyDown(object sender, KeyEventArgs e)
        {
            // alt and number marks and double click the ListViewItem
            if ((e.Modifiers == Keys.Alt) && ( (e.KeyValue >= 49) && (e.KeyValue <= 57) ) )
            {
                int itemNumber = e.KeyValue - 48;
                if(itemNumber <= listViewTypeLibInfo.Items.Count)
                {
                    listViewTypeLibInfo.Items[itemNumber-1].Selected = true;
                    listViewTypeLibInfo_DoubleClick(this, new EventArgs());
                }
            }

            if (e.KeyCode == Keys.Return)
                ShowResultItems();
        }
       
        private void listViewTypeLibInfo_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            _resultSorter.SortingColumn = e.Column;

            if (_resultSorter.SortingType == SortOrder.Ascending)
                _resultSorter.SortingType = SortOrder.Descending;
            else
                _resultSorter.SortingType = SortOrder.Ascending;

            listViewTypeLibInfo.Sort();
        }

        private void listViewTypeLibInfo_Resize(object sender, EventArgs e)
        {
            listViewTypeLibInfo.Columns[0].Width = (this.Size.Width / 100) * 10;
            listViewTypeLibInfo.Columns[1].Width = (this.Size.Width / 100) * 30;
            listViewTypeLibInfo.Columns[2].Width = (this.Size.Width / 100) * 10;
            listViewTypeLibInfo.Columns[3].Width = (this.Size.Width / 100) * 10;
            listViewTypeLibInfo.Columns[4].Width = (this.Size.Width / 100) * 30;
        }

        private void listViewTypeLibInfo_SelectedIndexChanged(object sender, EventArgs e)       
        {
            if (null != SelectedIndexChanged)
                SelectedIndexChanged(sender, e);
        }

        private void listViewTypeLibInfo_Click(object sender, EventArgs e)
        {
            if (null != Click)
                Click(sender, e);
        }

        private void listViewTypeLibInfo_DoubleClick(object sender, EventArgs e)
        {
            if (null != DoubleClick)
                DoubleClick(sender, e);
        }

        private void listViewTypeLibInfo_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyData == Keys.Return) && (listViewTypeLibInfo.SelectedItems.Count > 0)) 
                listViewTypeLibInfo_DoubleClick(this, new EventArgs());
        }

        private void buttonSaveSelection_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "PlainText(.txt)|*.txt";
            DialogResult dr = sfd.ShowDialog(this);
            if (dr == DialogResult.OK)
            {
                string saveFilePaths = "";
                foreach (ListViewItem item in listViewTypeLibInfo.SelectedItems)
                    saveFilePaths += item.SubItems[4].Text + "\r\n";

                if (true == System.IO.File.Exists(sfd.FileName))
                    System.IO.File.Delete(sfd.FileName);

                System.IO.File.AppendAllText(sfd.FileName, saveFilePaths);
            }
        }

        private void buttonLoadSelection_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "PlainText(.txt)|*.txt";
            DialogResult dr = ofd.ShowDialog(this);
            if( (dr == DialogResult.OK) && (true == System.IO.File.Exists(ofd.FileName)) )
            {
                string fileContent = System.IO.File.ReadAllText(ofd.FileName);
                string[] filesToSelect = fileContent.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string  item in filesToSelect)
                {
                    foreach (ListViewItem viewItem in listViewTypeLibInfo.Items)
                    {
                        if (item == viewItem.SubItems[4].Text)
                            viewItem.Selected = true;
                    }
                }
                listViewTypeLibInfo_Click(this, new EventArgs());
            }
        }

        #endregion
    }
}
