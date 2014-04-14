using CleanGame.Game.Core.Components.Unit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using CleanGame.Game.Core.Components;
using System.Xml.Serialization;

namespace EntityFramework
{
    public class MonsterComponent : UnitComponent
    {
        public string PlayerWeapon;
    }
}
