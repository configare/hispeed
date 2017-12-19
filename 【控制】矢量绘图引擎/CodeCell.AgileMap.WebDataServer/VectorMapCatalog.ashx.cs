using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using System.IO;
using CodeCell.AgileMap.Core;
using CodeCell.Bricks.Runtime;


namespace CodeCell.AgileMap.WebDataServer
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    [WebService(Namespace = "http://www.budgis.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class VectorMapHelper : IHttpHandler
    {
        private string _cnfgFile = null;

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                context.Response.ContentType = "application/octe-stream";
                HttpRequest req = context.Request;
                _cnfgFile = context.Server.MapPath(HttpCommands.cstServerInstanceCnfgFile);
                string cmd = req.Params[HttpCommands.cstArgNameCommand];
                switch (cmd)
                {
                    case HttpCommands.cstArgCmdGetInstanceList:
                        HandGetInstanceList(context);
                        break;
                    case HttpCommands.cstArgCmdGetCatalogList:
                        HandGetCatalogForInstance(context, req.Params[HttpCommands.cstArgInstanceId]);
                        break;
                    case HttpCommands.cstArgCmdGetFetClassProperty:
                        HandGetFetClassProperty(context,req.Params[HttpCommands.cstArgInstanceId], req.Params[HttpCommands.cstArgFetClassId]);
                        break;
                    case HttpCommands.cstArgCmdReadFeatures:
                        HandReadGrid(context, req);
                        break;
                    case HttpCommands.cstArgCmdBeginRead:
                        HandBeginRead(context, req);
                        break;
                    case HttpCommands.cstArgCmdEndRead:
                        HandEndRead(context, req);
                        break;
                    default:
                        {
                            if (cmd == null)
                                throw new Exception(HttpCommands.cstErrorCommandIsEmpty);
                            else
                                throw new Exception(string.Format(HttpCommands.cstErrorCommandInvalid, cmd));
                        }
                }
            }
            catch (Exception ex)
            {
                Log.WriterException(ex);
                context.Response.StatusDescription = "ERROR";
                RequestIsFailed obj = new RequestIsFailed(ex.Message);
                WriteObjectToResponseStream(obj, context);
            }
        }

        private void HandEndRead(HttpContext context, HttpRequest req)
        {
            string instanceId = req.Params[HttpCommands.cstArgInstanceId];
            string fetclassId = req.Params[HttpCommands.cstArgFetClassId];
            IVectorMapServerEnvironment env = VectorMapServerEnironment.GetInstance(_cnfgFile);
            if (env != null)
            {
                try
                {
                    env.BeginRead(int.Parse(instanceId), fetclassId);
                    WriteObjectToResponseStream(new RequestIsOK(),context); 
                }
                catch(Exception ex) 
                {
                    Log.WriterException(ex);
                    WriteObjectToResponseStream(new RequestIsFailed("尝试结束按网格读取要素失败。"),context);
                }
            }
            else
            {
                WriteObjectToResponseStream(new RequestIsFailed("获取IVectorMapServerEnvironment失败。"),context);
            }
        }

        private void HandBeginRead(HttpContext context, HttpRequest req)
        {
            string instanceId = req.Params[HttpCommands.cstArgInstanceId];
            string fetclassId = req.Params[HttpCommands.cstArgFetClassId];
            IVectorMapServerEnvironment env = VectorMapServerEnironment.GetInstance(_cnfgFile);
            if (env != null)
            {
                try
                {
                    env.EndRead(int.Parse(instanceId), fetclassId);
                    WriteObjectToResponseStream(new RequestIsOK(),context);
                }
                catch (Exception ex)
                {
                    Log.WriterException(ex);
                    WriteObjectToResponseStream(new RequestIsFailed("尝试开始按网格读取要素失败。"),context);
                }
            }
            else
            {
                WriteObjectToResponseStream(new RequestIsFailed("获取IVectorMapServerEnvironment失败。"),context);
            }
        }

        private void HandReadGrid(HttpContext context, HttpRequest req)
        {
            string instanceId = req.Params[HttpCommands.cstArgInstanceId];
            string fetclassId = req.Params[HttpCommands.cstArgFetClassId];
            double minx = double.Parse(req.Params[HttpCommands.cstArgMinX]);
            double maxx = double.Parse(req.Params[HttpCommands.cstArgMaxX]);
            double miny = double.Parse(req.Params[HttpCommands.cstArgMinY]);
            double maxy = double.Parse(req.Params[HttpCommands.cstArgMaxY]);
            IVectorMapServerEnvironment env = VectorMapServerEnironment.GetInstance(_cnfgFile);
            if (env != null)
            {
                Feature[] features = env.ReadFeatures(int.Parse(instanceId), fetclassId, new Envelope(minx, miny, maxx, maxy));
                if (features == null)
                    features = new Feature[] { };
                WriteObjectToResponseStream(features, context);
            }
        }

        private void HandGetFetClassProperty(HttpContext context, string instanceId,string fetclassId)
        {
              IVectorMapServerEnvironment env = VectorMapServerEnironment.GetInstance(_cnfgFile);
              if (env != null)
              {
                  FetClassProperty pro = env.GetFetClassProperty(int.Parse(instanceId), fetclassId);
                  WriteObjectToResponseStream(pro, context);
              }
        }

        private void HandGetCatalogForInstance(HttpContext context, string instanceId)
        {
            if (instanceId == null)
                throw new Exception(HttpCommands.cstErrorArgInstanceIdIsMissing);
            int id = -1;
            if (!int.TryParse(instanceId, out id))
                throw new Exception(string.Format(HttpCommands.cstErrorInstanceIsNotExisted, instanceId));
             IVectorMapServerEnvironment env = VectorMapServerEnironment.GetInstance(_cnfgFile);
             if (env != null)
             {
                 FetDatasetIdentify[] fetdsIds = null;
                 FetClassIdentify[] fetcIds = null;
                 env.GetCatalogsByInstanceId(id, out fetdsIds, out fetcIds);
                 object[] args = new object[] { fetdsIds,fetcIds};
                 WriteObjectToResponseStream(args as object, context);
             }
        }

        private void HandGetInstanceList(HttpContext context)
        {
            IVectorMapServerEnvironment env = VectorMapServerEnironment.GetInstance(_cnfgFile);
            if (env != null)
            {
                InstanceIdentify[] ids = env.GetAllServerInstances();
                WriteObjectToResponseStream(ids as object,context);
            }
        }

        private void WriteObjectToResponseStreamZip(object obj, HttpContext context)
        {
            Stream st = PersistObject.ObjectToStreamWithZIP(obj);
            using (BinaryReader br = new BinaryReader(st))
            {
                byte[] bs = br.ReadBytes((int)st.Length);
                context.Response.OutputStream.Write(bs, 0, bs.Length);
            }
        }

        private void WriteObjectToResponseStream(object obj,HttpContext context)
        {
            Stream st = PersistObject.ObjectToStream(obj);
            using (BinaryReader br = new BinaryReader(st))
            {
                byte[] bs = br.ReadBytes((int)st.Length);
                context.Response.OutputStream.Write(bs, 0, bs.Length);
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
