using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.MEF;

namespace GeoDo.RSS.Core.UI
{
    public class CommandEnvironment : ICommandEnvironment
    {
        private Dictionary<int, ICommand> _commands = new Dictionary<int, ICommand>();

        public CommandEnvironment(ISmartSession session)
        {
            string[] dlls = MefConfigParser.GetAssemblysByCatalog("系统命令");
            using (IComponentLoader<ICommand> loader = new ComponentLoader<ICommand>())
            {
                ICommand[] cmds = loader.LoadComponents(dlls);
                if (cmds != null)
                {
                    foreach (ICommand cmd in cmds)
                    {
                        cmd.Apply(session);
                        _commands.Add(cmd.Id, cmd);
                    }
                }
            }
        }

        public ICommand Get(int id)
        {
            if (_commands.ContainsKey(id))
                return _commands[id];
            return null;
        }

        public ICommand Find(string name)
        {
            foreach (ICommand cmd in _commands.Values)
                if (cmd.Name == name)
                    return cmd;
            return null;
        }

        public ICommand[] Search(string keyword)
        {
            List<ICommand> cmds = new List<ICommand>();
            int tryID = 0;
            if (int.TryParse(keyword, out tryID))
            {
                foreach (ICommand cmd in _commands.Values)
                {
                    if (cmd.Id == tryID)
                        cmds.Add(cmd);
                }
            }
            else
            {
                foreach (ICommand cmd in _commands.Values)
                {
                    if ((!string.IsNullOrWhiteSpace(cmd.Name) && cmd.Name.ToLower().Contains(keyword.ToLower())) ||
                        (!string.IsNullOrWhiteSpace(cmd.Text) && cmd.Text.ToLower().Contains(keyword.ToLower())) ||
                        (!string.IsNullOrWhiteSpace(cmd.ToolTip) && cmd.ToolTip.ToLower().Contains(keyword.ToLower())))
                        cmds.Add(cmd);
                }
            }
            return cmds.Count > 0 ? cmds.ToArray() : null;
        }
    }
}
