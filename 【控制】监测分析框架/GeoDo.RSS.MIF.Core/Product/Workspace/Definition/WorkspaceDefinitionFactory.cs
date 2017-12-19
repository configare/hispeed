using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public static class WorkspaceDefinitionFactory
    {
        static WorkspaceDef[] _workspaceDefs;

        static WorkspaceDefinitionFactory()
        {
            _workspaceDefs = (new WorkspaceDefinitionParser()).Parse();
        }

        public static WorkspaceDef[] WorkspaceDefs
        {
            get { return _workspaceDefs; }
        }

        public static WorkspaceDef GetWorkspaceDef(string productIdentify)
        {
            if (_workspaceDefs == null || _workspaceDefs.Length == 0 || string.IsNullOrEmpty(productIdentify))
                return null;
            foreach (WorkspaceDef wks in _workspaceDefs)
                if (wks.Identify == productIdentify)
                    return wks;
            return null;
        }
    }
}
