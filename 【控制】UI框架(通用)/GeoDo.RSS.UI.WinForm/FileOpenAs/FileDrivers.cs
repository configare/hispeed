using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using Telerik.WinControls.UI;
using System.Windows.Forms;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.UI.AddIn.CanvasViewer;

namespace GeoDo.RSS.UI.WinForm
{
    internal class FileDrivers
    {
        private ISmartSession _session;

        //public FileDrivers()
        //{
        //    TryLoadDrivers();
        //}

        public FileDrivers(ISmartSession _session)
        {
            // TODO: Complete member initialization
            this._session = _session;
        }

        public IGeoDataDriver[] TryLoadDrivers()
        {
            string[] driverNames = new string[] { "NOAA_1BD", "LDF", "GDAL", "MVG", "MEM" };
            List<IGeoDataDriver> drivers = new List<IGeoDataDriver>();
            foreach (string name in driverNames)
            {
                IGeoDataDriver driver = GetDriver(name);
                if (driver != null)
                    drivers.Add(driver);
            }
            return drivers.ToArray();
        }

        private IGeoDataDriver GetDriver(string driverName)
        {
            //IRasterDataProvider prd = null;
            //prd = driver.Open(filename, enumDataProviderAccess.ReadOnly) as IRasterDataProvider;
            try
            {
                IGeoDataDriver driver = GeoDataDriver.GetDriverByName(driverName);
                return driver;
            }
            finally
            {
            }
        }

        public RadMenuItem[] LoadMenuItems()
        {
            List<RadMenuItem> items = new List<RadMenuItem>();
            IGeoDataDriver[] drivers = TryLoadDrivers();
            foreach (IGeoDataDriver driver in drivers)
            {
                RadMenuItem item = new RadMenuItem();
                item.Text = driver.FullName;
                item.Tag = driver;
                item.Click += new EventHandler(item_Click);
                items.Add(item);
            }
            return items.ToArray();
        }

        void item_Click(object sender, EventArgs e)
        {
            RadMenuItem item = sender as RadMenuItem;
            if (item == null)
                return;
            IGeoDataDriver driver = item.Tag as IGeoDataDriver;
            if (driver == null)
                return;
            using (OpenFileDialog diag = new OpenFileDialog())
            {
                diag.Title = "文件打开为" + driver.FullName;
                if (diag.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        IGeoDataProvider provider = driver.Open(diag.FileName, enumDataProviderAccess.ReadOnly);
                        if (provider == null)
                            return;
                        OpenDataProvider(provider);
                    }
                    catch (Exception ex)
                    {
                        MsgBox.ShowInfo("无法识别为该驱动的文件" + driver.FullName);
                    }
                }
            }
        }

        private void OpenDataProvider(IGeoDataProvider provider)
        {
            if (provider is IRasterDataProvider)
            {
                string fname = (provider as IRasterDataProvider).fileName;

                CanvasViewer cv = new CanvasViewer(OpenFileFactory.GetTextByFileName(fname), _session);
                cv.Tag = fname;
                _session.SmartWindowManager.DisplayWindow(cv);
                RasterLayerBuilder.CreateAndLoadRasterLayer(_session, cv.Canvas, fname);
            }
        }
    }
}
