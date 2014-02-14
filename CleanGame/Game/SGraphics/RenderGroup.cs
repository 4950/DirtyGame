using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CleanGame.Game.SGraphics
{
    public class RenderGroup
    {
        private List<RenderInstance> instances;
        private List<RenderCommand> commands;

        public RenderGroup()
        {
            instances = new List<RenderInstance>();
            commands = new List<RenderCommand>();
        }

        public IEnumerable<RenderInstance> Instances
        {
            get
            {
                return instances;
            }
        }

        public IEnumerable<RenderCommand> Commands
        {
            get
            {
                return commands;
            }
        } 

        public void AddCommand(RenderCommand command)
        {
            commands.Add(command);
        }       

        public void AddInstance(RenderInstance instance)
        {
            instances.Add(instance);
        }

        public void RemoveInstance(RenderInstance instance)
        {
            if (instances.Contains(instance))
            {
                instances.Remove(instance);    
            }
        }
    }
}
