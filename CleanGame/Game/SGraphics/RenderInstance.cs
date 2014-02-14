using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CleanGame.Game.SGraphics.Commands;

namespace CleanGame.Game.SGraphics
{
    public class RenderInstance : IComparable<RenderInstance>
    {        
        private List<RenderCommand> commands;

        public RenderInstance()
        {
            commands = new List<RenderCommand>();
            SortKey = new SortKey();
        }

        public SortKey SortKey
        {
            get;
            set;
        }

        public DrawCall DrawCall
        {
            get;
            set;
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

       
        public int CompareTo(RenderInstance other)
        {
            return SortKey.CompareTo(other.SortKey);
        }
    }
}
