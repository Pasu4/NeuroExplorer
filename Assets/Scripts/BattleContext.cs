using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public class BattleContext
    {
        public BattleUI battleUI;
        public EnemyUI activeEnemy;
        public bool isInit = false;

        public BattleContext SetInit(bool value)
        {
            isInit = value;
            return this;
        }
    }
}
