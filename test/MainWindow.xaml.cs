using System;
using System.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Opc;
using Opc.Ua;
using OpcUaHelper;
using Opc.Ua.Client;

namespace test
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private TreeViewMode _treeViewRoot = new TreeViewMode();
        public TreeViewMode TreeViewRoot
        {
            get { return _treeViewRoot; }
            set { _treeViewRoot = value; }
        }

        public DataTable dataTable { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            #region treeview数据
            TreeViewMode treeViewModeNx = new TreeViewMode() { Name = "first" };
            TreeViewMode treeViewModeNx2 = new TreeViewMode() { Name = "second" };
            TreeViewMode treeViewModeNx3 = new TreeViewMode() { Name = "third" };

            TreeViewRoot.Children.Add(treeViewModeNx);
            TreeViewRoot.Children.Add(treeViewModeNx);
            TreeViewRoot.Children.Add(treeViewModeNx);
            TreeViewRoot.Children.Add(treeViewModeNx);
            TreeViewRoot.Children.Add(treeViewModeNx);

            treeViewModeNx.Children.Add(treeViewModeNx2);
            treeViewModeNx.Children.Add(treeViewModeNx2);
            treeViewModeNx.Children.Add(treeViewModeNx2);
            treeViewModeNx.Children.Add(treeViewModeNx2);
            treeViewModeNx.Children.Add(treeViewModeNx2);
            treeViewModeNx.Children.Add(treeViewModeNx2);

            treeViewModeNx2.Children.Add(treeViewModeNx3);
            treeViewModeNx2.Children.Add(treeViewModeNx3);
            treeViewModeNx2.Children.Add(treeViewModeNx3);
            treeViewModeNx2.Children.Add(treeViewModeNx3);
            #endregion

            #region DataGrid数据绑定
            dataTable = new DataTable();

            dataTable.Columns.Add(new DataColumn());
            dataTable.Columns.Add(new DataColumn());
            dataTable.Columns.Add(new DataColumn());
            dataTable.Columns.Add(new DataColumn());
            dataTable.Columns.Add(new DataColumn());

            DataRow dr = dataTable.NewRow();
            dr[0] = 1;
            dr[1] = "abc";
            dr[2] = "温度";
            dr[3] = "35.6";

            dataTable.Rows.Add(dr);
            dataTable.Rows.Add();

            //this.dg.ItemsSource = dataTable.AsDataView();
            this.dg.ItemsSource = dataTable.DefaultView;
            #endregion

        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            MessageBox.Show(((e.OriginalSource as TreeView).SelectedItem as TreeViewMode).Name);
        }

        private void dg_Loaded(object sender, RoutedEventArgs e)
        {
            this.dg.Columns[0].IsReadOnly = true;
            this.dg.Columns[2].IsReadOnly = true;
            this.dg.Columns[4].IsReadOnly = true;
        }
        OpcUaClient OpcUaClient = new OpcUaClient();
        public bool ConnectStatus { get { return OpcUaClient.Connected; } }
        /// <summary>
        /// 获取批量节点数据[同步读取]
        /// </summary>
        /// <param name="nodeIdList">节点列表</param>
        /// <returns>返回节点数据字典</returns>
        public Dictionary<string,DataValue> GetBatchNodeDatasOfSync(List<NodeId> nodeIdList)
        {
            Dictionary<string, DataValue> dicNodeInfo = new Dictionary<string, DataValue>();

            if (nodeIdList != null && nodeIdList.Count>0 && ConnectStatus)
            {
                try
                {
                    List<DataValue> dataValues = OpcUaClient.ReadNodes(nodeIdList.ToArray());
                    int count = nodeIdList.Count;
                    for (int i = 0; i < count; i++)
                    {
                        AddInfoToDic(dicNodeInfo, nodeIdList[i].ToString(), dataValues[i]);
                    }
                }
                catch (Exception ex)
                {
                    ClientUtils.HandleException("读取失败！", ex);
                }
            }
            return dicNodeInfo;
        }
        /// <summary>
        /// 获取到当前节点的值[异步读取]
        /// </summary>
        /// <typeparam name="T">节点对应的数据类型</typeparam>
        /// <param name="nodeId">节点</param>
        /// <returns>返回当前节点的值</returns>
        public async Task<T> GetCurrentNodeValueOfAsync<T>(string nodeId)
        {
            T value = default(T);
            if (!string.IsNullOrEmpty(nodeId) && ConnectStatus)
            {
                try
                {
                    value = await OpcUaClient.ReadNodeAsync<T>(nodeId);
                }
                catch (Exception ex)
                {
                    ClientUtils.HandleException("读取失败！", ex);
                }
            }
            return value;
        }
        /// <summary>
        /// 获取批量节点数据[异步读取]
        /// </summary>
        /// <param name="nodeIdList">节点列表</param>
        /// <returns>返回节点数据字典</returns>
        public async Task<Dictionary<string,DataValue>> GetBatchNodeDatasOfAsync(List<NodeId> nodeIdList)
        {
            Dictionary<string, DataValue> dicNodeInfo = new Dictionary<string, DataValue>();
            if (nodeIdList != null && nodeIdList.Count > 0 && ConnectStatus)
            {
                try
                {
                    List<DataValue> dataValues = await OpcUaClient.ReadNodesAsync(nodeIdList.ToArray());
                    int count = nodeIdList.Count;
                    for (int i = 0; i < count; i++)
                    {
                        AddInfoToDic(dicNodeInfo, nodeIdList[i].ToString(), dataValues[i]);
                    }
                }
                catch (Exception ex)
                {
                    ClientUtils.HandleException("读取失败！", ex);
                }
            }
            return dicNodeInfo;
        }
        /// <summary>
        /// 获取到当前节点的关联节点
        /// </summary>
        /// <param name="nodeId">当前节点</param>
        /// <returns>返回当前节点的关联节点</returns>
        public ReferenceDescription[] GetAllRelationNodeOfNodeId(string nodeId)
        {
            ReferenceDescription[] referenceDescriptions = null;
            if (!string.IsNullOrEmpty(nodeId) && ConnectStatus)
            {
                try
                {
                    referenceDescriptions = OpcUaClient.BrowseNodeReference(nodeId);
                }
                catch (Exception ex)
                {
                    string str = "获取当前：" + nodeId + "节点的相关节点失败！";
                    ClientUtils.HandleException(str, ex);
                }
            }
            return referenceDescriptions;
        }
        /// <summary>
        /// 获取当前节点的所有属性
        /// </summary>
        /// <param name="nodeId">当前节点</param>
        /// <returns>返回当前节点对应的所有属性</returns>
        public OpcNodeAttribute[] GetCurrentNodeAttributes(string nodeId)
        {
            OpcNodeAttribute[] opcNodeAttributes = null;
            if (!string.IsNullOrEmpty(nodeId) && ConnectStatus)
            {
                try
                {
                    opcNodeAttributes = OpcUaClient.ReadNoteAttributes(nodeId);
                }
                catch (Exception ex)
                {
                    string str = "读取节点：" + nodeId + "的所有属性失败！";
                    ClientUtils.HandleException(str, ex);
                }
            }
            return opcNodeAttributes;
        }
        /// <summary>
        /// 写入单个节点[同步方式]
        /// </summary>
        /// <typeparam name="T">写入节点值的数据类型</typeparam>
        /// <param name="nodeId">节点</param>
        /// <param name="value">写入成功则返回true，否则返回false</param>
        /// <returns></returns>
        public bool WriteSingleNodeIdOfSync<T>(string nodeId,T value)
        {
            bool success = false;
            if (OpcUaClient != null && ConnectStatus)
            {
                if (!string.IsNullOrEmpty(nodeId))
                {
                    try
                    {
                        success = OpcUaClient.WriteNode(nodeId, value);
                    }
                    catch (Exception ex)
                    {
                        string str = "当前节点：" + nodeId + "写入当前节点";
                        ClientUtils.HandleException(str, ex);
                    }
                }
            }
            return success;
        }
        /// <summary>
        /// 批量写入节点
        /// </summary>
        /// <param name="nodeIdArray">节点数据</param>
        /// <param name="nodeIdValueArray">节点对应数据组</param>
        /// <returns>返回写入结果(true:表示写入成功)</returns>
        public bool BatchWriteNodeIds(string[] nodeIdArray,object[] nodeIdValueArray)
        {
            bool success = false;
            if (nodeIdArray != null && nodeIdArray.Length > 0 &&
                nodeIdValueArray != null && nodeIdValueArray.Length > 0)
            {
                try
                {
                    success = OpcUaClient.WriteNodes(nodeIdArray, nodeIdValueArray);
                }
                catch (Exception ex)
                {
                    ClientUtils.HandleException("批量写入节点失败！", ex);
                }
            }
            return success;
        }
        /// <summary>
        /// 写入单个节点[异步方式]
        /// </summary>
        /// <typeparam name="T">写入节点值的数据类型</typeparam>
        /// <param name="nodeId">节点</param>
        /// <param name="value">节点对应的数据值</param>
        /// <returns>返回写入结果(true:表示写入成功)</returns>
        public async Task<bool> WriteSingleNodeIdOfAsync<T>(string nodeId,T value)
        {
            bool success = false;
            if (OpcUaClient != null && ConnectStatus)
            {
                if (!string.IsNullOrEmpty(nodeId))
                {
                    try
                    {
                        success = await OpcUaClient.WriteNodeAsync(nodeId, value);
                    }
                    catch (Exception ex)
                    {
                        string str = "当前节点：" + nodeId + "写入失败";
                        ClientUtils.HandleException(str, ex);
                    }
                }
            }
            return success;
        }
        /// <summary>
        /// 读取单个节点的历史数据
        /// </summary>
        /// <typeparam name="T">节点的数据类型</typeparam>
        /// <param name="nodeId">节点</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>返回该历史节点的历史数据记录</returns>
        public List<T> ReadSingleNodeIdHistoryDatas<T>(string nodeId,DateTime startTime,DateTime endTime)
        {
            List<T> nodeIdDatas = null;
            if (!string.IsNullOrEmpty(nodeId) && startTime != null &&
                endTime != null && endTime > startTime)
            {
                try
                {
                    nodeIdDatas = OpcUaClient.ReadHistoryRawDataValues<T>(nodeId, startTime, endTime).ToList();
                }
                catch (Exception ex)
                {
                    ClientUtils.HandleException("读取失败", ex);
                }
            }
            return nodeIdDatas;
        }
        /// <summary>
        /// 读取单个节点的历史数据
        /// </summary>
        /// <param name="nodeId">节点</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">节点时间</param>
        /// <returns>返回该节点对应的历史数据记录</returns>
        public List<DataValue> ReadSingleNodeIdHistoryDatas(string nodeId,DateTime startTime,DateTime endTime)
        {
            List<DataValue> nodeIdDatas = null;
            if (!string.IsNullOrEmpty(nodeId) && startTime != null &&
                endTime != null && endTime > startTime)
            {
                if (ConnectStatus)
                {
                    try
                    {
                        nodeIdDatas = OpcUaClient.ReadHistoryRawDataValues(nodeId, startTime, endTime).ToList();
                    }
                    catch (Exception ex)
                    {
                        ClientUtils.HandleException("读取失败", ex);
                    }
                }
            }
            return nodeIdDatas;
        }
        /// <summary>
        /// 单节点数据订阅
        /// </summary>
        /// <param name="key">订阅的关键字(必须唯一)</param>
        /// <param name="nodeId">节点</param>
        /// <param name="callback">数据订阅的回调方法</param>
        public void SingleNodeIdDatasSubscription(string key,string nodeId,
            Action<string,MonitoredItem,MonitoredItemNotificationEventArgs> callback)
        {
            if (ConnectStatus)
            {
                try
                {
                    OpcUaClient.AddSubscription(key, nodeId, callback);
                }
                catch (Exception ex)
                {
                    string str = "订阅节点" + nodeId + "数据失败";
                    ClientUtils.HandleException(str, ex);
                }
            }
        }
        /// <summary>
        /// 取消单节点数据订阅
        /// </summary>
        /// <param name="key">订阅的关键字</param>
        /// <returns></returns>
        public bool CancelSingleNodeIdDatasSubscription(string key)
        {
            bool success = false;
            if (!string.IsNullOrEmpty(key))
            {
                if (ConnectStatus)
                {
                    try
                    {
                        OpcUaClient.RemoveSubscription(key);
                        success = true;
                    }
                    catch (Exception ex)
                    {
                        string str = "取消" + key + "的订阅失败";
                        ClientUtils.HandleException(str, ex);
                    }
                }
            }
            return success;
        }
        /// <summary>
        /// 批量节点数据订阅
        /// </summary>
        /// <param name="key">订阅的关键字(必须唯一)</param>
        /// <param name="nodeIds">节点数组</param>
        /// <param name="callback">数据订阅的回调方法</param>
        public void BatchNodeIdDatasSubscription(string key,string[] nodeIds,
            Action<string,MonitoredItem,MonitoredItemNotificationEventArgs> callback)
        {
            if (!string.IsNullOrEmpty(key) && nodeIds != null && nodeIds.Length > 0)
            {
                if (ConnectStatus)
                {
                    try
                    {
                        OpcUaClient.AddSubscription(key, nodeIds, callback);
                    }
                    catch (Exception ex)
                    {
                        string str = "批量订阅节点数据失败！";
                        ClientUtils.HandleException(str, ex);
                    }
                }
            }
        }
        /// <summary>
        /// 取消所有节点的数据订阅
        /// </summary>
        /// <returns></returns>
        public bool CancelAllNodeIdDatasSubscription()
        {
            bool success = false;
            if (ConnectStatus)
            {
                try
                {
                    OpcUaClient.RemoveAllSubscription();
                    success = true;
                }
                catch (Exception ex)
                {
                    ClientUtils.HandleException("取消所有的节点数据订阅失败！", ex);
                }
            }
            return success;
        }
        private void AddInfoToDic(Dictionary<string,DataValue> dic,string key,DataValue dataValue)
        {
            if (dic != null)
            {
                if (!dic.ContainsKey(key))
                {
                    dic.Add(key, dataValue);
                }
                else
                {
                    dic[key] = dataValue;
                }
            }
        }
    }


    public class TreeViewMode
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        private ObservableCollection<TreeViewMode> _children;

        public ObservableCollection<TreeViewMode> Children
        {
            get { return _children; }
            set { _children = value; }
        }

        public TreeViewMode()
        {
            _children = new ObservableCollection<TreeViewMode>();
        }
    }
}
