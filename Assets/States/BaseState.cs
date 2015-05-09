using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.States
{
    public abstract class BaseState
    {
        public GameStateModel MainState;

        public BaseState(GameStateModel targetState)
        {
            this.MainState = targetState;
        }
    }
}
