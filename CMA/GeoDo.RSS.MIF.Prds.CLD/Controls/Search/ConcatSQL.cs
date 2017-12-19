using System;
using System.Text;
using System.Windows.Forms;

namespace GeoDo.RSS.MIF.Prds.CLD
{
    public struct DataSetTableMapping
    {
        public string DatabaseTableName;
        public string DataSetIDFieldName;
        public string DataSetNameFieldName;
        public string DataSetLabelFieldName;
        public int DataSetID;
    }

    public class ConcatSQL
    {
        private StringBuilder _sql;     //sql语句
        private string _dataSource;     //产品数据源
        private string _productType;    //产品类型
        private string _productPeriod;  //产品周期
        //private string _dayNight;       //昼夜
        private string _searchType;     //连续、同期查询
        private string _dateTimeBegin;  //开始日期
        private string _dateTimeEnd;    //结束日期

        //////////////////////////////////////
        //文件路径=ImageData；
        //日产品--PeriodTypeID==
        ///查询条件中的数据库表字段
        ////数据来源(dataSource)=SensorType;产品类型(productType)=ProductID;数据集名(DataSet)=DataSetID;日夜(dayNight)=DataSource;
        ///gridview
        ////观测日期、区域、分辨率、数据集、数据有效性、5分钟段个数、5分钟段时间、文件路径;

        //周期产品----PeriodTypeID==
        ///查询条件中的数据库表字段
        ////周期类型=PeriodTypeID;数据来源(dataSource)=SensorType;产品类型(productType)=ProductID;数据集名(DataSet)=DataSetID;日夜(dayNight)=DataSource;
        ///gridview
        ////年、月、旬、区域、分辨率、数据集、文件路径、有效百分比、统计类型；
        //////////////////////////////////////

        public string Sql { get { return _sql.ToString(); } }

        //根据控件组合拼接sql
        public void ConcatSQLByUIControls(UIControls ui)
        {
            _sql = new StringBuilder();
            foreach (Control c in ui.GroupBoxProductDataSource.Controls)
            {
                RadioButton rb = c as RadioButton;
                if (rb != null && rb.Checked)
                {
                    _dataSource = rb.Text;
                    switch (rb.Text)
                    {
                        case "ISCCP D2":
                            {
                                ConcatISCCPSQL(ui);
                                return;
                            }
                        default:
                            break;
                    }                    
                }
            }
            ConcatSQLByProductPeriod(ui);//周期类型
            ConcatSQLByProductDataSource(ui);//数据来源-传感器
            ConcatSQLByProductType(ui);//产品类型
            ConcatSQLByDataSet(ui);//数据集ID
            ConcatSQLByDayNight(ui);//日夜标识
            AppendSQLByProductPeriod(ui);
            ConcatSQLByProductTime(ui);//连续或同期查询
        }

#region ISCCP查询
        private void ConcatISCCPSQL(UIControls ui)
        {
            //_sql.Append("select KeyWords,Resolution, DataYear, DataMonth, Data10Day,ImageData, ValidValuePercent, statTypeID from cp_periodicsynthesis_tb where PeriodTypeID=4 and ");
            _sql.Append("select KeyWord as KeyWords,Resolution, YEAR(ReceiveTime) as DataYear, MONTH(ReceiveTime) as DataMonth,null as Data10Day, ImageData,3 as statTypeID from cp_isccpd2_tb where ReceiveUTCTime=9999 and ");//MONTH(ReceiveTime)=12;");
            RecordSearchType(ui);
            RecordDateTime(ui);
            foreach (Control c in ui.GroupBoxProductPeriod.Controls)
            {
                RadioButton rb = c as RadioButton;
                if (rb != null && rb.Checked)
                    _productPeriod = rb.Text;
            }
            switch (_searchType)
            {
                case "连续查询":
                    {
                        ConcatISCCPSQLByProductTimeContinue();
                        break;
                    }
                case "同期查询":
                    {
                        ConcatISCCPSQLByProductTimeOverTheSamePeriod(ui);
                        break;
                    }
            }

            #region 波段号
            foreach (Control c in ui.GroupBoxDataSet.Controls)
            {
                ucCheckBoxList uccbl = c as ucCheckBoxList;
                if (uccbl != null)
                {
                    foreach (CheckBox cb in uccbl)
                    {
                        if (cb.Checked)
                        {
                            int isccpband = int.Parse(cb.Tag.ToString());
                        }
                    }
                    break;
                }
            }
            #endregion

        }

        /// <summary>
        /// 拼接连续查询sql
        /// </summary>
        private void ConcatISCCPSQLByProductTimeContinue()
        {
            switch (_productPeriod)
            {
                case "月产品":
                    {
                        string datebegin = _dateTimeBegin + "-01";
                        string dateend = _dateTimeEnd + "-01";
                        _sql.Append("DATE(ReceiveTime)>='").Append(datebegin).Append("' and DATE(ReceiveTime)<='").Append(dateend).Append("';");
                        break;
                    }
                case "年产品":
                    {
                        //_sql.Append("DataYear>=").Append(_dateTimeBegin);
                        //_sql.Append(" and ");
                        //_sql.Append("DataYear<=").Append(_dateTimeEnd).Append(";");
                        break;
                    }
            }
        }

        /// <summary>
        /// 拼接同期查询sql
        /// </summary>
        /// <param name="ui"></param>
        private void ConcatISCCPSQLByProductTimeOverTheSamePeriod(UIControls ui)
        {
            _sql.Append("Year(ReceiveTime)>=").Append(_dateTimeBegin.Split('-')[0]);
            _sql.Append(" and ");
            _sql.Append("Year(ReceiveTime)<=").Append(_dateTimeEnd.Split('-')[0]).Append(" and (");
            switch (_productPeriod)
            {
                case "月产品":
                    {
                        foreach (Control c in ui.GroupBoxProductTime.Controls)
                        {
                            Panel p = c as Panel;
                            if (p != null && p.Name == "panelPrdsTimeMonth")
                            {
                                foreach (Control cc in p.Controls)
                                {
                                    ucMonths ucM = cc as ucMonths;
                                    if (ucM != null)
                                    {
                                        bool checkedPrevious = false;
                                        int month = 0;
                                        foreach (CheckBox cb in ucM)
                                            AppendOr(ref checkedPrevious, cb.Checked, "Month(ReceiveTime)", ++month);
                                        _sql.Append(");");
                                        break;
                                    }
                                }
                                break;
                            }
                        }
                        break;
                    }
            }
        }

#endregion

        //拼接产品周期sql，并记录产品周期
        private void ConcatSQLByProductPeriod(UIControls ui)
        {
            foreach (Control c in ui.GroupBoxProductPeriod.Controls)
            {
                RadioButton rb = c as RadioButton;
                if (rb != null && rb.Checked)
                {
                    _productPeriod = rb.Text;
                    switch (rb.Text)
                    {
                        case "日产品":
                            {
                                _sql.Append("select DATE(DataTime) as DataTime, KeyWords,Resolution,ValidValuePercent, GranuleCounts, GranuleTimes, ImageData from cp_daymergeproducts_tb where ");
                                //_sql.Append("select * from cp_daymergeproducts_tb where ");
                                break;
                            }
                        case "旬产品":
                            {
                                //_sql.Append("select KeyWords,Resolution, DataYear, DataMonth, Data10Day,ImageData, ValidValuePercent, statTypeID from cp_periodicsynthesis_tb where PeriodTypeID=3 and ");
                                _sql.Append("select* from cp_periodicsynthesis_tb where PeriodTypeID=3 and ");
                                break;
                            }
                        case "月产品":
                            {
                                //_sql.Append("select KeyWords,Resolution, DataYear, DataMonth, Data10Day,ImageData, ValidValuePercent, statTypeID from cp_periodicsynthesis_tb where PeriodTypeID=4 and ");
                                _sql.Append("select * from cp_periodicsynthesis_tb where PeriodTypeID=4 and ");
                                break;
                            }
                        case "年产品":
                            {
                                //_sql.Append("select KeyWords,Resolution, DataYear, DataMonth, Data10Day,ImageData, ValidValuePercent, statTypeID from cp_periodicsynthesis_tb where PeriodTypeID=5 and ");
                                _sql.Append("select * from cp_periodicsynthesis_tb where PeriodTypeID=5 and ");
                                break;
                            }
                    }
                    break;
                }
            }
        }

        private void AppendSQLByProductPeriod(UIControls ui)
        {
            foreach (Control c in ui.GroupBoxProductPeriod.Controls)
            {
                RadioButton rb = c as RadioButton;
                if (rb != null && rb.Checked)
                {
                    _productPeriod = rb.Text;
                    switch (rb.Text)
                    {
                        case "日产品":
                            {
                                break;
                            }
                        case "旬产品":
                            {
                                //_sql.Append("select KeyWords,Resolution, DataYear, DataMonth, Data10Day,ImageData, ValidValuePercent, statTypeID from cp_periodicsynthesis_tb where PeriodTypeID=3 and ");
                                _sql.Append(" PeriodTypeID=3  and ");
                                break;
                            }
                        case "月产品":
                            {
                                //_sql.Append("select KeyWords,Resolution, DataYear, DataMonth, Data10Day,ImageData, ValidValuePercent, statTypeID from cp_periodicsynthesis_tb where PeriodTypeID=4 and ");
                                _sql.Append(" PeriodTypeID=4  and ");
                                break;
                            }
                        case "年产品":
                            {
                                //_sql.Append("select KeyWords,Resolution, DataYear, DataMonth, Data10Day,ImageData, ValidValuePercent, statTypeID from cp_periodicsynthesis_tb where PeriodTypeID=5 and ");
                                _sql.Append(" PeriodTypeID=5 and   ");
                                break;
                            }
                    }
                    break;
                }
            }
        }


        //拼接产品数据源sql，并记录产品数据源
        private void ConcatSQLByProductDataSource(UIControls ui)
        {
            foreach (Control c in ui.GroupBoxProductDataSource.Controls)
            {
                RadioButton rb = c as RadioButton;
                if (rb != null && rb.Checked)
                {
                    _dataSource = rb.Text;
                    switch (rb.Text)
                    {
                        case "MOD06":
                            {
                                _sql.Append("SensorType='MODIS' and ");
                                break;
                            }
                        case "AIRS":
                            {
                                _sql.Append("SensorType='AIRS' and ");
                                break;
                            }
                        case "ISCCP D2":
                            {
                                _sql.Append("SensorType='ISCCP' and ");
                                break;
                            }
                    }
                    break;
                }
            }
        }

        //拼接产品类型sql，并记录产品类型
        private void ConcatSQLByProductType(UIControls ui)
        {
            foreach (Control c in ui.GroupBoxProductType.Controls)
            {
                ucRadioBoxList ucrbl = c as ucRadioBoxList;
                if (ucrbl != null)
                {
                    foreach (RadioButton rb in ucrbl)
                    {
                        if (rb.Checked)
                        {
                            _productType = rb.Text;
                            //_sql.Append("ProductID=(select ProductsID from cp_cloudprds_tb where ProductsComments='").Append(rb.Text).Append("') and ");
                            _sql.Append("ProductID='").Append(rb.Tag.ToString()).Append("' and ");
                            break;
                        }
                    }
                    break;
                }
            }
        }

        //拼接数据集sql，并记录数据集
        private void ConcatSQLByDataSet(UIControls ui)
        {
            DataSetTableMapping mapping = GetTableNameByDataSource(ui);
            foreach (Control c in ui.GroupBoxDataSet.Controls)
            {
                ucCheckBoxList uccbl = c as ucCheckBoxList;
                if (uccbl != null)
                {
                    //_sql.Append("(");
                    bool checkedPrevious = false;
                    foreach (CheckBox cb in uccbl)
                    {
                        if (cb.Checked)
                        {
                            StringBuilder sql = new StringBuilder();
                            //sql.Append("DataSetID=(select ").Append(mapping.DataSetIDFieldName)
                            //    .Append(" from ").Append(mapping.DatabaseTableName)
                            //    .Append(" where ").Append(mapping.DataSetLabelFieldName).Append("='").Append(cb.Text).Append("')");
                            sql.Append("DataSetID='").Append(cb.Tag.ToString()).Append("'");
                            AppendOr(ref checkedPrevious, cb.Checked, sql.ToString());
                        }
                    }
                    _sql.Append(" and ");
                    break;
                }
            }
        }

        //拼接昼夜sql，并记录白天、晚上
        private void ConcatSQLByDayNight(UIControls ui)
        {
            foreach (Control c in ui.GroupBoxDayNight.Controls)
            {
                RadioButton rb = c as RadioButton;
                if (rb != null && rb.Checked)
                {
                    switch (rb.Text)
                    {
                        case "白天":
                            {
                                _sql.Append("DataSource='D' and ");
                                break;
                            }
                        case "晚上":
                            {
                                _sql.Append("DataSource='N' and ");
                                break;
                            }
                    }
                }
            }
        }

        //拼接产品时间sql，并记录连续查询、同期查询和开始日期、结束日期
        private void ConcatSQLByProductTime(UIControls ui)
        {
            RecordSearchType(ui);
            RecordDateTime(ui);
            switch (_searchType)
            {
                case "连续查询":
                    {
                        ConcatSQLByProductTimeContinue();
                        break;
                    }
                case "同期查询":
                    {
                        ConcatSQLByProductTimeOverTheSamePeriod(ui);
                        break;
                    }
            }
        }

        //记录连续查询、同期查询
        private void RecordSearchType(UIControls ui)
        {
            foreach (Control c in ui.GroupBoxProductTime.Controls)
            {
                RadioButton rb = c as RadioButton;
                if (rb != null && rb.Checked)
                {
                    _searchType = rb.Text;
                    break;
                }
            }
        }

        //记录开始日期、结束日期
        private void RecordDateTime(UIControls ui)
        {
            foreach (Control c in ui.GroupBoxProductTime.Controls)
            {
                DateTimePicker dtp = c as DateTimePicker;
                if (dtp != null)
                {
                    switch (dtp.Name)
                    {
                        case "dateTimePickerBegin":
                            {
                                _dateTimeBegin = dtp.Text;
                                break;
                            }
                        case "dateTimePickerEnd":
                            {
                                _dateTimeEnd = dtp.Text;
                                break;
                            }
                    }
                }
            }
        }

        /// <summary>
        /// 拼接连续查询sql
        /// </summary>
        private void ConcatSQLByProductTimeContinue()
        {
            switch (_productPeriod)
            {
                case "日产品":
                    {
                        _sql.Append("DATE_FORMAT(DataTime,'%Y-%m-%d')>='").Append(_dateTimeBegin).Append("'");
                        _sql.Append(" and ");
                        _sql.Append("DATE_FORMAT(DataTime,'%Y-%m-%d')<='").Append(_dateTimeEnd).Append("';");
                        break;
                    }
                case "旬产品":
                    {
                        break;
                    }
                case "月产品":
                    {
                        string[] splitsBegin = _dateTimeBegin.Split('-');
                        string[] splitsEnd = _dateTimeEnd.Split('-');
                        int yearBegin = Int32.Parse(splitsBegin[0]);
                        int monthBegin = Int32.Parse(splitsBegin[1]);
                        int yearEnd = Int32.Parse(splitsEnd[0]);
                        int monthEnd = Int32.Parse(splitsEnd[1]);
                        if (yearBegin == yearEnd)
                        {
                            //同年，monthBegin到monthEnd
                            _sql.Append(" DataYear=").Append(yearBegin).Append(" and DataMonth>=").Append(monthBegin).Append(" and DataMonth<=").Append(monthEnd).Append(";");
                        }
                        else if (yearBegin + 1 == yearEnd)
                        {
                            //两年，yearBegin的monthBegin到12月，以及yearEnd的1月到monthEnd
                            //_sql.Append("((DataYear=").Append(yearBegin).Append(" and DataMonth>=").Append(monthBegin).Append(") or ");
                            //_sql.Append("(DataYear=").Append(yearEnd).Append(" and DataMonth<=").Append(monthEnd).Append("));");
                            StringBuilder endSQL = new StringBuilder(_sql.ToString());
                            endSQL.Append("(DataYear=").Append(yearEnd).Append(" and DataMonth<=").Append(monthEnd).Append(");");
                            _sql.Append("(DataYear=").Append(yearBegin).Append(" and DataMonth>=").Append(monthBegin).Append(") UNION ");
                            _sql.Append(endSQL.ToString());

                        }
                        else if (yearBegin + 1 < yearEnd)
                        {
                            //三年（含）以上，yearBegin的monthBegin到12月，中间的整年，以及yearEnd的1月到monthEnd
                            //_sql.Append("((DataYear=").Append(yearBegin).Append(" and DataMonth>=").Append(monthBegin).Append(") or ");
                            //_sql.Append("(DataYear>=").Append(yearBegin + 1).Append(" and ").Append("DataYear<=").Append(yearEnd - 1).Append(") or ");
                            //_sql.Append("(DataYear=").Append(yearEnd).Append(" and DataMonth<=").Append(monthEnd).Append("));");
                            StringBuilder midSQL = new StringBuilder(_sql.ToString());
                            midSQL.Append("(DataYear>=").Append(yearBegin + 1).Append(" and ").Append("DataYear<=").Append(yearEnd - 1).Append(") UNION ");
                            StringBuilder endSQL = new StringBuilder(_sql.ToString());
                            endSQL.Append("(DataYear=").Append(yearEnd).Append(" and DataMonth<=").Append(monthEnd).Append(");");
                            _sql.Append("(DataYear=").Append(yearBegin).Append(" and DataMonth>=").Append(monthBegin).Append(") UNION ");
                            _sql.Append(midSQL.ToString());
                            _sql.Append(endSQL.ToString());
                        }
                        break;
                    }
                case "年产品":
                    {
                        _sql.Append("DataYear>=").Append(_dateTimeBegin);
                        _sql.Append(" and ");
                        _sql.Append("DataYear<=").Append(_dateTimeEnd).Append(";");
                        break;
                    }
            }
        }

        /// <summary>
        /// 拼接同期查询sql
        /// </summary>
        /// <param name="ui"></param>
        private void ConcatSQLByProductTimeOverTheSamePeriod(UIControls ui)
        {
            _sql.Append("DataYear>=").Append(_dateTimeBegin);
            _sql.Append(" and ");
            _sql.Append("DataYear<=").Append(_dateTimeEnd).Append(" and (");
            switch (_productPeriod)
            {
                case "旬产品":
                    {
                        foreach (Control c in ui.GroupBoxProductTime.Controls)
                        {
                            Panel p = c as Panel;
                            if (p != null && p.Name == "panelPrdsTimeMonth")
                            {
                                foreach (Control cc in p.Controls)
                                {
                                    ucMonths ucM = cc as ucMonths;
                                    if (ucM != null)
                                    {
                                        bool checkedPrevious = false;
                                        int month = 0;
                                        foreach (CheckBox cb in ucM)
                                            AppendOr(ref checkedPrevious, cb.Checked, "DataMonth", ++month);
                                        _sql.Append(") and (");
                                        break;
                                    }
                                }
                            }
                            else if (p != null && p.Name == "panelPrdsTimeTen")
                            {
                                bool checkedPrevious = false;
                                foreach (Control cc in p.Controls)
                                {
                                    CheckBox cb = cc as CheckBox;
                                    if (cb != null && cb.Checked)
                                    {
                                        switch (cb.Text)
                                        {
                                            case "上旬":
                                                {
                                                    AppendOr(ref checkedPrevious, cb.Checked, "Data10Day", 1);
                                                    break;
                                                }
                                            case "中旬":
                                                {
                                                    AppendOr(ref checkedPrevious, cb.Checked, "Data10Day", 2);
                                                    break;
                                                }
                                            case "下旬":
                                                {
                                                    AppendOr(ref checkedPrevious, cb.Checked, "Data10Day", 3);
                                                    break;
                                                }
                                        }
                                    }
                                }
                                //_sql.Append(");");
                                _sql.Append(");");
                            }
                        }
                        break;
                    }
                case "月产品":
                    {
                        foreach (Control c in ui.GroupBoxProductTime.Controls)
                        {
                            Panel p = c as Panel;
                            if (p != null && p.Name == "panelPrdsTimeMonth")
                            {
                                foreach (Control cc in p.Controls)
                                {
                                    ucMonths ucM = cc as ucMonths;
                                    if (ucM != null)
                                    {
                                        bool checkedPrevious = false;
                                        int month = 0;
                                        foreach (CheckBox cb in ucM)
                                            AppendOr(ref checkedPrevious, cb.Checked, "DataMonth", ++month);
                                        //_sql.Append(");");
                                        _sql.Append(");");                                        break;
                                    }
                                }
                                break;
                            }
                        }
                        break;
                    }
            }
        }

        private DataSetTableMapping GetTableNameByDataSource(UIControls ui)
        {
            DataSetTableMapping mapping = new DataSetTableMapping();
            foreach (Control c in ui.GroupBoxProductDataSource.Controls)
            {
                RadioButton rb = c as RadioButton;
                if (rb != null && rb.Checked)
                {
                    switch (rb.Text)
                    {
                        case "MOD06":
                            {
                                mapping.DatabaseTableName = "cp_cldprds2mod06sets_tb";
                                mapping.DataSetIDFieldName = "Prds2MOD06ID";
                                mapping.DataSetNameFieldName = "MOD06DataSetName";
                                mapping.DataSetLabelFieldName = "MOD06SetsLabel";
                                break;
                            }
                        case "AIRS":
                            {
                                mapping.DatabaseTableName = "cp_cldprds2airssets_tb";
                                mapping.DataSetIDFieldName = "Prds2AIRSID";
                                mapping.DataSetNameFieldName = "AIRSDataSetName";
                                mapping.DataSetLabelFieldName = "AIRSSetsLabel";
                                break;
                            }
                        case "ISCCP D2":
                            {
                                mapping.DatabaseTableName = "cp_cldprds2isccpbands_tb";
                                mapping.DataSetIDFieldName = "Prds2CloudSATID";
                                mapping.DataSetNameFieldName = "CloudSATDataSetName";
                                mapping.DataSetLabelFieldName = "CloudSATDataSetLabel";
                                break;
                            }
                    }
                    break;
                }
            }
            return mapping;
        }

        //sql后追加单个查询条件，之间要用or隔开
        private void AppendOr(ref bool checkedPrevious, bool checkedCurrent, string field, int value)
        {
            if (checkedCurrent)
            {
                if (checkedPrevious)
                    _sql.Append(" or ");
                _sql.Append(field).Append("=").Append(value);
            }
            checkedPrevious = checkedPrevious || checkedCurrent;
        }

        //sql后追加单个查询条件，之间要用or隔开
        private void AppendOr(ref bool checkedPrevious, bool checkedCurrent, string sql)
        {
            if (checkedCurrent)
            {
                if (checkedPrevious)
                    _sql.Append(" or ");
                _sql.Append(sql);
            }
            checkedPrevious = checkedPrevious || checkedCurrent;
        }
    }
}
