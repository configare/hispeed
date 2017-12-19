using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CodeCell.Bricks.Runtime;

namespace CodeCell.AgileMap.Components
{
    public static class CatalogPersist
    {
        private static string ConfigFilename = AppDomain.CurrentDomain.BaseDirectory + "Config\\Catalog.cfg";

        public static CatalogDatabaseConn[] GetCatalogDatabaseConns()
        {
            try
            {
                if (!File.Exists(ConfigFilename))
                    return null;
                string[] lines = File.ReadAllLines(ConfigFilename, Encoding.Default);
                List<CatalogDatabaseConn> items = new List<CatalogDatabaseConn>();
                foreach (string ln in lines)
                {
                    //SPATIAL CONNECTION:name,description,connectionstring
                    if (ln.ToUpper().Contains("SPATIAL CONNECTION@"))
                    {
                        string[] parts = ln.Split('@');
                        parts = parts[1].Split(',');
                        SpatialDatabaseConn conn = new SpatialDatabaseConn(parts[0], parts[1], parts[2]);
                        CatalogDatabaseConn cdc = new CatalogDatabaseConn(parts[0], conn, parts[1]);
                        items.Add(cdc);
                    }
                }
                return items.Count > 0 ? items.ToArray() : null;
            }
            catch(Exception ex) 
            {
                Log.WriterException(ex);
                return null;
            }
        }

        public static CatalogLocal[] GetCatalogLocalFolders()
        {
            try
            {
                if (!File.Exists(ConfigFilename))
                    return null;
                string[] lines = File.ReadAllLines(ConfigFilename,Encoding.Default);
                List<CatalogLocal> items = new List<CatalogLocal>();
                foreach (string ln in lines)
                {
                    //SPATIAL LOCAL FOLDER:name,url
                    if (ln.ToUpper().Contains("SPATIAL LOCAL FOLDER@"))
                    {
                        string[] parts = ln.Split('@');
                        items.Add(new CatalogLocal(parts[1], parts[1], parts[1]));
                    }
                }
                return items.Count > 0 ? items.ToArray() : null;
            }
            catch (Exception ex)
            {
                Log.WriterException(ex);
                return null;
            }
        }

        public static CatalogNetServer[] GetCatalogNetServers()
        {
            try
            {
                if (!File.Exists(ConfigFilename))
                    return null;
                string[] lines = File.ReadAllLines(ConfigFilename, Encoding.Default);
                List<CatalogNetServer> items = new List<CatalogNetServer>();
                foreach (string ln in lines)
                {
                    //SPATIAL LOCAL FOLDER:name,url
                    if (ln.ToUpper().Contains("SPATIAL HTTPSERVER@"))
                    {
                        string[] parts = ln.Split('@');
                        parts = parts[1].Split(',');
                        items.Add(new CatalogNetServer(parts[0], parts[2], parts[1]));
                    }
                }
                return items.Count > 0 ? items.ToArray() : null;
            }
            catch (Exception ex)
            {
                Log.WriterException(ex);
                return null;
            }
        }
    }
}
